namespace api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{

    private readonly IAuthDb _authDb;

    public AuthController(IAuthDb authDb) {
        _authDb = authDb;
    }

    [HttpPost(Name = "Login")]
    public async Task<ActionResult> Login(UserLogin userLogin)
    {
        if (await _authDb.TryAuthenticate(userLogin)) {
            return Ok("user found");
        }
        return Ok("could not find user");
    }

}