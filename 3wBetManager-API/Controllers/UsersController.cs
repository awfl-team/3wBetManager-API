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
        [Route("/users")]
        public async Task<IHttpActionResult> GetAll()
        {
            return Ok(await getUserDao().FindAllUser());
        }

        [HttpGet]
        [Route("/users/{userId}")]
        public async Task<IHttpActionResult> Get(string id)
        {
            return Ok(await getUserDao().FindUser(id));
        }

        /*[HttpPost]
        [Route("/register")]
        public IHttpActionResult Register([FromBody] User user)
        {
            var fullUserEmail = getUserDao().FindUserByEmail(user.Email);
            var fullUserPseudo = getUserDao().FindUserByPseudo(user.Pseudo);

            if (fullUserEmail == null && fullUserPseudo == null)
            {
                getUserDao().AddUser(user);
                return Ok();
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "MDR le c#");
            }
        }*/

        [HttpPost]
        [Route("/login")]
        public IHttpActionResult Post([FromBody] User user)
        {
            try
            {
                var fullUser = getUserDao().FindUserByEmail(user.Email);
                if (BCrypt.Net.BCrypt.Verify(user.Password, fullUser.Result.Password))
                {
                    return Ok(TokenManager.GenerateToken(fullUser.Result.Email, fullUser.Result.Role,
                        fullUser.Result.Pseudo));
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "Wrong login password");
                }
            }
            catch (AggregateException)
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