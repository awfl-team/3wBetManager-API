using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Models;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    public class AuthController : BaseController
    {
        [Route("register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register([FromBody] User user)
        {
            return await HandleError(async () =>
            {
                var userExist = await UserManager.UsernameAndEmailExist(user);
                if (userExist != null)
                {
                    return Content(HttpStatusCode.BadRequest, userExist);
                }

                await GetUserDao().AddUser(user);
                return Created("", user);
            });
        }

        [Route("login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login([FromBody] User user)
        {
            return await HandleError(async () =>
            {
                const string errorMessage = "Wrong login password";
                var fullUser = await GetUserDao().FindUserByEmail(user.Email);

                if (BCrypt.Net.BCrypt.Verify(user.Password, fullUser.Password))
                {
                    return Ok(TokenManager.GenerateToken(fullUser.Email, fullUser.Role,
                        fullUser.Username));
                }

                return Content(HttpStatusCode.BadRequest, errorMessage);
            });
        }
    }
}