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


        [Route("top50")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                return Ok(await getUserDao().FindBestBetters());
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
                return Ok(await TokenManager.GetUserByToken(Request));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
        
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> Put(string id, [FromBody] User user)
        {
            try
            {
                var canUpdate  = getUserDao().CanUpdate(id, user, out var errorMessage);
                if (canUpdate == false)
                {
                    return Content(HttpStatusCode.BadRequest, errorMessage);
                }
                getUserDao().UpdateUser(id, user);
                var fullUser = await Singleton.Instance.UserDao.FindUser(id);
                return Ok(TokenManager.GenerateToken(fullUser.Email, fullUser.Role, fullUser.Username));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("visibility")]
        [HttpPut]
        public async Task<IHttpActionResult> PutIsPrivate([FromBody] User userParam)
        {
            try
            {
                var user = await TokenManager.GetUserByToken(Request);
                Singleton.Instance.UserDao.UpdateUserIsPrivate(user.Id, userParam.IsPrivate);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("role")]
        [HttpPut]
        public async Task<IHttpActionResult> PutRole([FromBody] User userParam)
        {
            try
            {
                var user = await TokenManager.GetUserByToken(Request);
                Singleton.Instance.UserDao.UpdateUserRole(user.Id, userParam.Role);
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
        
        [Route("reset")]
        [HttpPut]
        public async Task<IHttpActionResult> Put()
        {
            try
            {
                var user = await TokenManager.GetUserByToken(Request);
                if (user.Life == 0)
                {
                    return Content(HttpStatusCode.BadRequest, "You already used all your lives");
                }
                getUserDao().ResetUser(user);
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