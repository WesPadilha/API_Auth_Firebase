using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("profile")]
    [AuthorizeFirebase]
    public IActionResult Profile()
    {
        return Ok(new { message = "Acesso autorizado!" });
    }
}