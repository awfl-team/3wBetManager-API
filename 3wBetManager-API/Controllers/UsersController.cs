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
            return await HandleError(async () => Ok(await GetUserDao().FindAllUser()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            return await HandleNotFound(async () => Ok(await GetUserDao().FindUser(id)));
        }


        [Route("top50")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTop50()
        {
            return await HandleError(async () => Ok(await UserManager.GetBestBetters()));
      
        }


        [Route("place")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserPlace()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await UserManager.GetUserPlace(user));
            });

        }

        [Route("top3")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTop3()
        {
            return await HandleError(async () => Ok(await UserManager.GetTop3()));

        }

        [Route("token")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserFromToken()
        {
            return await HandleError(async () => Ok(await GetUserByToken(Request)));
        }
        
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> Put(string id, [FromBody] User user)
        {
            return await HandleNotFound(async () =>
            {
                var canUpdate = await UserManager.CanUpdate(id, user);
                if (canUpdate != null)
                {
                    return Content(HttpStatusCode.BadRequest, canUpdate);
                }
                await GetUserDao().UpdateUser(id, user);
                var fullUser = await Singleton.Instance.UserDao.FindUser(id);
                return Ok(TokenManager.GenerateToken(fullUser.Email, fullUser.Role, fullUser.Username));
            });
        }

        [Route("visibility")]
        [HttpPut]
        public async Task<IHttpActionResult> PutIsPrivate([FromBody] User userParam)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                await GetUserDao().UpdateUserIsPrivate(user.Id, userParam.IsPrivate);
                return Ok();
            });
        }

        [Route("{id}/role")]
        [HttpPut]
        public async Task<IHttpActionResult> PutRole(string id, [FromBody] User userParam)
        {
            return await HandleNotFound(async () =>
            {
                await GetUserDao().UpdateUserRole(id, userParam.Role);
                return Ok();
            });
      
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            return await HandleNotFound(async () =>
            {
                await GetUserDao().DeleteUser(id);
                return Ok();
            });
        }

        [Route("search/{value}")]
        [HttpGet]
        public async Task<IHttpActionResult> Search(string value)
        {
            return await HandleError(async () =>
            {
                var searchValue = await GetUserDao().SearchUser(value);
                if (searchValue.Count == 0)
                {
                    return NotFound();
                }
                return Ok(searchValue);
            });
        }

        [Route("reset")]
        [HttpPut]
        public async Task<IHttpActionResult> Put()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                if (user.Life == 0)
                {
                    return Content(HttpStatusCode.BadRequest, "You already used all your lives");
                }
                await UserManager.ResetUser(user);
                return Ok();
            });
        }

        [Route("paginated/{page}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllUsersPaginated(int page)
        {
            return await HandleError(async () => Ok(await UserManager.GetAllUsersPaginated(page)));
        }

        [Route("new")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser([FromBody] User user)
        {
            return await HandleError(async () =>
            {
                var userExist = await UserManager.UsernameAndEmailExist(user);
                if (userExist != null)
                {
                    return Content(HttpStatusCode.BadRequest, userExist);
                }

                await GetUserDao().AddUser(user);
                return Created("", user);
            });
        }

        [Route("stats/coins")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserCoinStats()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await UserManager.GetUserCoinStats(user));
            });
        }
    }
}