using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    public class UsersController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            return Ok(await getUserDao().FindAllUser());
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            return Ok(await getUserDao().FindUser(id));
        }

        [HttpPost]
        public IHttpActionResult Register([FromBody] User user)
        {
            var userByEmail = getUserDao().FindUserByEmail(user.Email);
            var userByUsername = getUserDao().FindUserByUsername(user.Username);
            if (userByEmail.Result == null && userByUsername.Result == null)
            {
                getUserDao().AddUser(user);
                return Ok();
            }
            else
            {
                if (userByEmail.Result != null && userByUsername.Result == null)
                {
                    return Content(HttpStatusCode.BadRequest, "email already taken");
                }

                if (userByUsername.Result != null && userByEmail.Result == null)
                {
                    return Content(HttpStatusCode.BadRequest, "username already taken");
                }

                return Content(HttpStatusCode.BadRequest, "username and email already taken");
            }

        }

        [HttpPost]
        public IHttpActionResult Login([FromBody] User user)
        {
            var fullUser = getUserDao().FindUserByEmail(user.Email);
            if (fullUser.Result != null)
            {
                if (BCrypt.Net.BCrypt.Verify(user.Password, fullUser.Result.Password))
                {
                    return Ok(TokenManager.GenerateToken(fullUser.Result.Email, fullUser.Result.Role,
                        fullUser.Result.Username));
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Wrong login password");
                }
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "email not found");
            }
        }

        private IUserDao getUserDao()
        {
            return Singleton.Instance.SetUserDao(new UserDao());
        }
    }
}