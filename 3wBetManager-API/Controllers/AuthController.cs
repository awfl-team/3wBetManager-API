using System;
using System.Net;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    public class AuthController: ApiController
    {
        [Route("register")]
        [HttpPost]
        public IHttpActionResult Register([FromBody] User user)
        {
         
            try
            {
                var isExist = getUserDao().UsernameAndEmailExist(user, out var errorMessage);
                if (isExist == false)
                {
                    return Content(HttpStatusCode.BadRequest, errorMessage);
                }

                // TODO create a constant in MODEL.USER
                user.Role = "USER";
                getUserDao().AddUser(user);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] User user)
        {
            const string errorMessage = "Wrong login password";
            try
            {
                var fullUser = getUserDao().FindUserByEmailSingle(user.Email);

                if (BCrypt.Net.BCrypt.Verify(user.Password, fullUser.Result.Password))
                {
                    return Ok(TokenManager.GenerateToken(fullUser.Result.Email, fullUser.Result.Role,
                        fullUser.Result.Username));
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, errorMessage);
                }
            }
            catch (AggregateException)
            {
                return Content(HttpStatusCode.BadRequest, errorMessage);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        private IUserDao getUserDao()
        {
            return Singleton.Instance.UserDao;
        }
    }
}
