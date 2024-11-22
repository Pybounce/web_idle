namespace api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IUserSystem _userSystem;
    private readonly IAuthSystem _authSystem;

    public UserController(IUserSystem userSystem, IAuthSystem authSystem) {
        _userSystem = userSystem;
        _authSystem = authSystem;
    }
    // TODO: Input sanitisation and error handling
    [HttpPost(Name = "Login")]
    public async Task<ActionResult> Login(UserLogin userLogin)
    {
        if (await _authSystem.TryAuthenticate(userLogin)) {
            return Ok("user found");
        }
        return Ok("could not find user");
    }

    [HttpPost(Name = "CreateAccount")]
    public async Task<ActionResult> CreateAccount(UserCreate userCreate)
    {
        if (await _userSystem.TryCreate(userCreate)) {
            return Ok("user created");
        }
        return Ok("could not create user");
    }

}