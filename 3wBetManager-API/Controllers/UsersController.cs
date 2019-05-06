using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Models;

namespace _3wBetManager_API.Controllers
{
    [IsGranted]
    [RoutePrefix("users")]
    public class UsersController : BaseController
    {
        [IsGranted(Models.User.AdminRole)]
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
            return await HandleError(async () =>
            {
                using (var userManager = new UserManager())
                {
                    return Ok(await userManager.GetBestBetters());
                }
            });
        }


        [Route("place")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserPlace()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                using (var userManager = new UserManager())
                {
                    return Ok(await userManager.GetUserPositionAmongSiblings(user));
                }
            });
        }

        [Route("top3")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTop3()
        {
            return await HandleError(async () =>
            {
                using (var userManager = new UserManager())
                {
                    return Ok(await userManager.GetTop3());
                }
            });
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
                using (var userManager = new UserManager())
                {
                    var canUpdate = await userManager.CanUpdate(id, user);
                    if (canUpdate.Length > 0) return Content(HttpStatusCode.BadRequest, canUpdate);
                }

                await GetUserDao().UpdateUser(id, user);
                var fullUser = await GetUserDao().FindUser(id);
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
                if (searchValue.Count == 0) return NotFound();
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
                if (user.Items.FindAll(i => i.Type == Item.Life).Count == 0)
                    return Content(HttpStatusCode.BadRequest, "You already used all your lives");
                using (var userManager = new UserManager())
                {
                    await userManager.ResetUser(user);
                }

                return Ok();
            });
        }

        [Route("paginated/{page}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllUsersPaginated(int page)
        {
            return await HandleError(async () =>
            {
                using (var userManager = new UserManager())
                {
                    return Ok(await userManager.GetAllUsersPaginated(page));
                }
            });
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser([FromBody] User user)
        {
            return await HandleError(async () =>
            {
                using (var userManager = new UserManager())
                {
                    var userExist = await userManager.UsernameAndEmailExist(user);
                    if (userExist.Length > 0) return Content(HttpStatusCode.BadRequest, userExist);
                }

                // TODO change this
                await GetUserDao().AddUser(user, user.Role);
                return Created("", user);
            });
        }
    }
}