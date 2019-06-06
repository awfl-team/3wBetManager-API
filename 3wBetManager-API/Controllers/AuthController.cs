using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
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
                var userExist = await GetUserManager().UsernameAndEmailExist(user);
                if (userExist.Length > 0) return Content(HttpStatusCode.BadRequest, userExist);


                await GetUserManager().AddUser(user, Models.User.UserRole);

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
                var fullUser = await GetUserManager().GetUserByEmail(user.Email);
                if (fullUser == null) return Content(HttpStatusCode.BadRequest, errorMessage);
                if (BCrypt.Net.BCrypt.Verify(user.Password, fullUser.Password))
                    return Ok(GetTokenManager().GenerateToken(fullUser.Email, fullUser.Role,
                        fullUser.Username));
                return Content(HttpStatusCode.BadRequest, errorMessage);
            });
        }

        [Route("forgot_password")]
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword([FromBody] User userParam)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserManager().GetUserByEmail(userParam.Email);
                if (user == null) return NotFound();

                GetEmailManager().SendResetPasswordEmail(user);

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
                if (user == null) return BadRequest();

                user.Password = userParam.Password;
                await GetUserManager().ChangePassword(user);

                return Content(HttpStatusCode.NoContent, "");
            });
        }

        [Route("verify_account")]
        [HttpPut]
        public async Task<IHttpActionResult> VerifyAccount([FromBody] User userParam)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                if (user == null) return BadRequest();

                await GetUserManager().ChangeIsEnabled(user.Id, true);

                return Content(HttpStatusCode.NoContent, "");
            });
        }
    }
}