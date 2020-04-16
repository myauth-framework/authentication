using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAuth.HeaderAuthentication;
using TestServer.Models;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Request.HttpContext.User.Claims.Select(c => new ClaimModel(c)).ToArray());
        }

        [HttpPost("is-in-role")]
        public IActionResult IsInRole([FromBody]string role)
        {
            return Ok(Request.HttpContext.User.IsInRole(role).ToString());
        }
    }
}
