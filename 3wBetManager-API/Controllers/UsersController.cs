using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
    [Route("api/users")]
    public class UsersController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get(string uid)
        {
            return Ok(await getUserDao().GetUser(uid));
        }

        /*[HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] User user)
        {
            getUserDao().AddUser(user);
            return Ok();
        }
        */

        [HttpPost]
        public IHttpActionResult Login()
        {
            return Ok(TokenManager.GenerateToken("lucas", "admin"));
        }


        private IUserDao getUserDao()
        {
            return Singleton.Instance.SetUserDao(new UserDao());
        }
    }
}