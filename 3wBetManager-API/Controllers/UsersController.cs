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
            try
            {
                var isExist = getUserDao().UsernameAndEmailExist(user, out var errorMessage);
                if (isExist == false)
                {
                    return Content(HttpStatusCode.BadRequest, errorMessage);
                }
                getUserDao().UpdateUser(id, user);
                return Ok();
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