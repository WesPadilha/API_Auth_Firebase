using System.Text;                  // Usado para manipular texto e codificações (UTF8, etc.)
using System.Text.Json;             // Usado para converter objetos em JSON e ler respostas JSON
using FirebaseAdmin;                // Biblioteca oficial do Firebase Admin (para gerenciar usuários e autenticação)
using FirebaseAdmin.Auth;           // Fornece classes e métodos de autenticação do Firebase Admin
using FirebaseAuthApi.Models;       // Importa as classes de modelo (RegisterRequest, LoginRequest, etc.)

namespace FirebaseAuthApi.Services
{
    // Serviço responsável por lidar com autenticação de usuários no Firebase
    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;  // Cliente HTTP usado para enviar requisições REST ao Firebase
        private const string FirebaseApiKey = "AIzaSyBerEFCbSdkq7sUyvaLVXsoUdwd3ctvbLA"; 
        // Sua chave de API do Firebase (usada na autenticação pelo endpoint REST)

        public FirebaseAuthService()
        {
            _httpClient = new HttpClient(); // Inicializa o cliente HTTP
        }

        // Método para registrar (criar) um novo usuário no Firebase
        public async Task<UserRecord> RegisterUserAsync(RegisterRequest request)
        {
            // Cria um objeto de argumentos para o método CreateUserAsync do Firebase Admin
            var args = new FirebaseAdmin.Auth.UserRecordArgs
            {
                Email = request.Email,       // Define o e-mail do novo usuário
                Password = request.Password, // Define a senha do novo usuário
            };

            // Cria o usuário no Firebase Authentication usando o SDK Admin
            return await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        }

        // Método para logar um usuário já existente e retornar o token de autenticação
        public async Task<string?> LoginUserAsync(LoginRequest request)
        {
            // URL da API REST do Firebase responsável pelo login com senha
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseApiKey}";

            // Cria o corpo da requisição com os dados de login
            var payload = new
            {
                email = request.Email,        // E-mail do usuário
                password = request.Password,  // Senha
                returnSecureToken = true      // Faz o Firebase retornar um token JWT válido
            };

            // Converte o objeto acima em JSON e prepara para enviar no corpo da requisição
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Envia a requisição POST para o Firebase
            var response = await _httpClient.PostAsync(url, content);

            // Lê o corpo da resposta (pode conter sucesso ou erro)
            var responseBody = await response.Content.ReadAsStringAsync();

            // Se o status da resposta não for 200 (sucesso), lança uma exceção com o erro retornado
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro no login: {responseBody}");
            }

            // Caso o login tenha dado certo, lê o JSON da resposta
            using var doc = JsonDocument.Parse(responseBody);

            // Pega o valor do "idToken" (token JWT que representa o usuário autenticado)
            return doc.RootElement.GetProperty("idToken").GetString();
        }
        
        public async Task<UserRecord> CreateUserWithRoleAsync(CreateUserRequest request)
        {
            var args = new UserRecordArgs
            {
                Email = request.Email,
                Password = request.Password,
            };

            // Cria o usuário no Auth
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

            // Define o "Claim" de role
            var claims = new Dictionary<string, object>
            {
                { "role", request.Role }
            };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
            
            return userRecord;
        }
    }
}
