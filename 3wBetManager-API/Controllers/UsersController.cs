using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    public class UsersController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                return Ok(await getUserDao().FindAllUser());
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            try
            {
                return Ok(await getUserDao().FindUser(id));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetByEmail(string email)
        {
            try
            {
                return Ok(await getUserDao().FindUserByEmailToList(email));
            }
            catch (AggregateException)
            {
                return Content(HttpStatusCode.BadRequest, "email not found");
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPut]
        public IHttpActionResult Put(string id, [FromBody] User user)
        {
            // TODO refactor this in a function 
            try
            {
                var userByEmail = getUserDao().FindUserByEmailToList(user.Email);
                var userByUsername = getUserDao().FindUserByUsername(user.Username);
                if (userByEmail.Result == null && userByUsername.Result == null)
                {
                    getUserDao().UpdateUser(id, user);
                    return Ok();
                }

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
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        public IHttpActionResult Register([FromBody] User user)
        {
            // TODO refactor this in a function 
            try
            {
                var userByEmail = getUserDao().FindUserByEmailToList(user.Email);
                var userByUsername = getUserDao().FindUserByUsername(user.Username);
                if (userByEmail.Result == null && userByUsername.Result == null)
                {
                    user.Role = "User";
                    getUserDao().AddUser(user);
                    return Created("", user);
                }

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
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

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