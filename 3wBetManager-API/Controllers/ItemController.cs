using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Hub;
using Manager;
using Models;

namespace _3wBetManager_API.Controllers
{
    [IsGranted]
    [RoutePrefix("items")]
    public class ItemController: BaseController
    {
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> BuyItems(List<Item> items)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                await ItemManager.BuyItemsToUser(items, user);
                return Created("", items);
            });
        }


        [Route("loot")]
        [HttpPost]
        public async Task<IHttpActionResult> AddItems(List<Item> items)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                await ItemManager.AddItemsToUser(items, user);
                return Created("", items);
            });
        }

        [Route("bomb/{userId}")]
        [HttpPut]
        public async Task<IHttpActionResult> UseBomb(string userId)
        {
            return await HandleError(async () =>
            {
                var notificationHub = new NotificationHub();
                var user = await GetUserByToken(Request);
                var sendTo = await ItemManager.UseBomb(userId);
                notificationHub.SendNotification(sendTo.Username, user.Username + " used a bomb on you");
                return Content(HttpStatusCode.NoContent, "");
            });
        }

        [Route("multiplier/{multiply}/{betId}")]
        [HttpPut]
        public async Task<IHttpActionResult> UseMultiplier(int multiply,string betId)
        {
            return await HandleError(async () =>
            {
                await ItemManager.UseMultiplier(betId, multiply);
                return Content(HttpStatusCode.NoContent, "");
            });
        }

        /*[Route("key/{userId}")]
        [HttpGet]
        public async Task<IHttpActionResult> UseKey(int multiply, string betId)
        {
            return await HandleError(async () =>
            {
                await ItemManager.UseMultiplier(betId, multiply);
                return Content(HttpStatusCode.NoContent, "");
            });
        }*/

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            return await HandleError(async () => Ok(await GetItemDao().FindAllItems()));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateItem(string id, [FromBody] Item item)
        {
            return await HandleError(async () =>
            {
                await GetItemDao().UpdateItem(id, item);
                return Ok();
            });
        }
    }
}
