using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using Models;
using MongoDB.Bson;

namespace _3wBetManager_API.Controllers
{
    [Route("api/users")]
    public class UsersController : ApiController
    {
        // GET api/values
        public async Task<IHttpActionResult> Get(string uid)
        {
            var id = ObjectId.Parse(uid);
            using (UserDao UserDao = new UserDao())
            {
                return Ok(await UserDao.GetUser(id));
            }

        }

        // POST api/values 
        public async Task<IHttpActionResult> Post ([FromBody] User user)
        {
            using (UserDao UserDao = new UserDao())
            {
               UserDao.AddUser(user);
            }

            return Ok();

        }

    }
}
