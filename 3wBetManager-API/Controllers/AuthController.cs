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
            try
            {
                var userExist = await UserManager.UsernameAndEmailExist(user);
                if (userExist != null)
                {
                    return Content(HttpStatusCode.BadRequest, userExist);
                }

                GetUserDao().AddUser(user);
                return Created("", user);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login([FromBody] User user)
        {
            const string errorMessage = "Wrong login password";
            try
            {
                var fullUser = await GetUserDao().FindUserByEmail(user.Email);

                if (BCrypt.Net.BCrypt.Verify(user.Password, fullUser.Password))
                {
                    return Ok(TokenManager.GenerateToken(fullUser.Email, fullUser.Role,
                        fullUser.Username));
                }

                return Content(HttpStatusCode.BadRequest, errorMessage);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}