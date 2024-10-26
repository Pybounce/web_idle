namespace api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{

    [HttpPost(Name = "PostTest")]
    public ActionResult PostTest(object a)
    {
        Console.WriteLine(a);
        Console.WriteLine("boop");
        return Ok("ok");
    }

}