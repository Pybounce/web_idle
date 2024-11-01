namespace api.Controllers;

using System.Net.WebSockets;
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