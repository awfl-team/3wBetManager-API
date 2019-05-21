using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
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
            return await HandleError(async () => Ok(await GetUserManager().GetAllUser()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            return await HandleNotFound(async () => Ok(await GetUserManager().GetUser(id)));
        }

        [Route("top50")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTop50()
        {
            return await HandleError(async () => Ok(await GetUserManager().GetBestBetters()));
        }


        [Route("place")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserPlace()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);

                return Ok(await GetUserManager().GetUserPositionAmongSiblings(user));
            });
        }

        [Route("top3")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTop3()
        {
            return await HandleError(async () => Ok(await GetUserManager().GetTop3()));
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
                var canUpdate = await GetUserManager().CanUpdate(id, user);
                if (canUpdate.Length > 0) return Content(HttpStatusCode.BadRequest, canUpdate);


                await GetUserManager().ChangeUser(id, user);
                var fullUser = await GetUserManager().GetUser(id);
                return Ok(GetTokenManager().GenerateToken(fullUser.Email, fullUser.Role, fullUser.Username));
            });
        }

        [Route("visibility")]
        [HttpPut]
        public async Task<IHttpActionResult> PutIsPrivate([FromBody] User userParam)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                await GetUserManager().ChangeUserIsPrivate(user.Id, userParam.IsPrivate);
                return Ok();
            });
        }

        [Route("{id}/role")]
        [HttpPut]
        public async Task<IHttpActionResult> PutRole(string id, [FromBody] User userParam)
        {
            return await HandleNotFound(async () =>
            {
                await GetUserManager().ChangeUserRole(id, userParam.Role);
                return Ok();
            });
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            return await HandleNotFound(async () =>
            {
                await GetUserManager().DeleteUser(id);
                return Ok();
            });
        }

        [Route("search/{value}")]
        [HttpGet]
        public async Task<IHttpActionResult> Search(string value)
        {
            return await HandleError(async () =>
            {
                var searchValue = await GetUserManager().SearchUser(value);
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

                await GetUserManager().ResetUser(user);


                return Ok();
            });
        }

        [Route("paginated/{page}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllUsersPaginated(int page)
        {
            return await HandleError(async () => Ok(await GetUserManager().GetAllUsersPaginated(page)));
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser([FromBody] User user)
        {
            return await HandleError(async () =>
            {
                var userExist = await GetUserManager().UsernameAndEmailExist(user);
                if (userExist.Length > 0) return Content(HttpStatusCode.BadRequest, userExist);


                // TODO change this
                await GetUserManager().AddUser(user, user.Role);
                return Created("", user);
            });
        }
    }
}