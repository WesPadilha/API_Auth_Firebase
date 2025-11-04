using System.Text;
using System.Text.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAuthApi.Models;

namespace FirebaseAuthApi.Services
{
    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;
        private const string FirebaseApiKey = "AIzaSyBerEFCbSdkq7sUyvaLVXsoUdwd3ctvbLA";

        public FirebaseAuthService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<UserRecord> RegisterUserAsync(RegisterRequest request)
        {
            var args = new FirebaseAdmin.Auth.UserRecordArgs
            {
                Email = request.Email,
                Password = request.Password,
            };

            return await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        }
        
        public async Task<string?> LoginUserAsync(LoginRequest request)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseApiKey}";

            var payload = new
            {
                email = request.Email,
                password = request.Password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro no login: {responseBody}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement.GetProperty("idToken").GetString();
        }
    }
}