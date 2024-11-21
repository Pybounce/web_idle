namespace api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{

    [HttpPost(Name = "Login")]
    public ActionResult Login(UserLogin userLogin)
    {
        Console.WriteLine("username: " + userLogin.Username);
        Console.WriteLine("password " + userLogin.Password);
        return Ok("ok");
    }

}