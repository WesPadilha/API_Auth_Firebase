using FirebaseAuthApi.Models;
using FirebaseAuthApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly FirebaseAuthService _firebaseAuthService;

    public AdminController(FirebaseAuthService firebaseAuthService)
    {
        _firebaseAuthService = firebaseAuthService;
    }

    [HttpPost("create-user")]
    //[AuthorizeFirebase("admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _firebaseAuthService.CreateUserWithRoleAsync(request);
            return Ok(new
            {
                user.Uid,
                user.Email,
                Role = request.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}