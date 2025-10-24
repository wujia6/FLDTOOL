using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLDTOOL.ErpOaSync.Controllers
{
    [ApiController]
    [Route("api/security")]
    public class SecurityController : ControllerBase
    {
        [HttpGet("oaLogin")]
        public dynamic OaLogin(string username, string password)
        {
            return new { username, password };
        }
        
        [HttpPost("signIn")]
        public Task<dynamic> SignInAsync([FromBody] (string username, string password) paramJson)
        {
            return Task.FromResult<dynamic>(new
            {
                Username = paramJson.username,
                Password = paramJson.password
            });
        }
    }
}
