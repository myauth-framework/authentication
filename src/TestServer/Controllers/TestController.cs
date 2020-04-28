using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAuth.Authentication;
using MyLab.ApiClient;
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

        [HttpGet("req-headers")]
        public IActionResult GetWithRequiredHeaders(
            [RequiredClaimHeader("X-Claim-User-Id")] string userId,
            [RequiredClaimHeader("X-Claim-Account-Id")] string accountId)
        {
            return Ok(userId + "-" + accountId);
        }

        [HttpGet("authorized")]
        [Authorize]
        public IActionResult GetAuthorized()
        {
            return Ok();
        }

        [HttpPost("is-in-role")]
        public IActionResult IsInRole([FromBody]string role)
        {
            return Ok(Request.HttpContext.User.IsInRole(role).ToString());
        }
    }
}
