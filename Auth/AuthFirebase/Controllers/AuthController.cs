using Microsoft.AspNetCore.Mvc;
using FirebaseAuthApi.Services;
using FirebaseAuthApi.Models;

namespace FirebaseAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Chama o banco de dados e crai uma variárvel
        private readonly FirebaseAuthService _firebaseAuthService;

        // Método construtor
        public AuthController(FirebaseAuthService firebaseAuthService)
        {
            // Irá registrar o valor recebido dentro da variável
            _firebaseAuthService = firebaseAuthService;
        }

        // Método HTTP para Criar um usuário
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var userRecord = await _firebaseAuthService.RegisterUserAsync(request);
                return Ok(new
                {
                    Uid = userRecord.Uid,
                    Email = userRecord.Email
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }

        /*[HttpPost("register")]
        public async Task<IActionResult> Auth([FromBody] LoginRequest request)
        {
            
        }*/
    }
}