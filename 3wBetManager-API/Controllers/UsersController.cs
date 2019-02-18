using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using Manager;
using Models;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : BaseController
    {
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                return Ok(await GetUserDao().FindAllUser());
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
                return Ok(await GetUserDao().FindUser(id));
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
                return Ok(await UserManager.GetBestBetters());
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
                return Ok(await GetUserByToken(Request));
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
                var canUpdate  = await UserManager.CanUpdate(id, user);
                if (canUpdate != null)
                {
                    return Content(HttpStatusCode.BadRequest, canUpdate);
                }
                GetUserDao().UpdateUser(id, user);
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
                var user = await GetUserByToken(Request);
                GetUserDao().UpdateUserIsPrivate(user.Id, userParam.IsPrivate);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{id}/role")]
        [HttpPut]
        public IHttpActionResult PutRole(string id, [FromBody] User userParam)
        {
            try
            {
                GetUserDao().UpdateUserRole(id, userParam.Role);
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
                GetUserDao().DeleteUser(id);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("search/{value}")]
        [HttpGet]
        public async Task<IHttpActionResult> Search(string value)
        {
            try
            {
                var searchValue = await GetUserDao().SearchUser(value);
                if (searchValue.Count == 0)
                {
                    return NotFound();
                }
                return Ok(searchValue);
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
                var user = await GetUserByToken(Request);
                if (user.Life == 0)
                {
                    return Content(HttpStatusCode.BadRequest, "You already used all your lives");
                }
                GetUserDao().ResetUser(user);
                return Ok();
                
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

    }
}