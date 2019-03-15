using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Models;


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
                if (userExist.Length > 0)
                {
                    return Content(HttpStatusCode.BadRequest, userExist);
                }
                
                await GetUserDao().AddUser(user, Models.User.AdminRole);
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

        [Route("forgot_password")]
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword([FromBody] User userParam)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserDao().FindUserByEmail(userParam.Email);
                if (user == null)
                {
                    return NotFound();
                }

                using (var emailManager = new EmailManager())
                {
                    emailManager.SendResetPasswordEmail(user);
                }
                return Ok();
            });
        }

        [Route("reset_password")]
        [HttpPut]
        public async Task<IHttpActionResult> ResetPassword([FromBody] User userParam)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                if (user == null)
                {
                    return BadRequest();
                }

                user.Password = userParam.Password;
                await GetUserDao().UpdateUserPassword(user);
             
                return Content(HttpStatusCode.NoContent, "");
            });
        }
    }
}
