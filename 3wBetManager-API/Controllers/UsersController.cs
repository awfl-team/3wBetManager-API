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
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        [Route("")]
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

        [Route("{id}")]
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

        [Route("token")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserFromToken()
        {
            try
            {
                var token = TokenManager.GetTokenFromRequest(Request);
                var user = TokenManager.ValidateToken(token);
                return Ok(await getUserDao().FindUserByEmailToList(user["email"]));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult Put(string id, [FromBody] User user)
        {
            try
            {
                var canUpdate  = getUserDao().CanUpdate(id, user, out var errorMessage);
                if (canUpdate == false)
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

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                getUserDao().DeleteUser(id);
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