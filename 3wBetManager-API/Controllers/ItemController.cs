using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
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
        [HttpGet]
        public async Task<IHttpActionResult> AddItems()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                var items = await ItemManager.AddItemsToUser(user);
                await Singleton.Instance.UserDao.RemoveUserItem(await GetUserByToken(Request), Item.LootBox);
                return Created("", items);
            });
        }

        [Route("mystery")]
        [HttpGet]
        public async Task<IHttpActionResult> AddMysteryItem()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                var item = await ItemManager.AddMysteryItemToUser(user);
                await Singleton.Instance.UserDao.RemoveUserItem(await GetUserByToken(Request), Item.Mystery);
                return Created("", item);
            });
        }

        [Route("bomb/{userId}")]
        [HttpPut]
        public async Task<IHttpActionResult> UseBomb(string userId)
        {
            return await HandleError(async () =>
            {
                await ItemManager.UseBomb(userId);
                await Singleton.Instance.UserDao.RemoveUserItem(await GetUserByToken(Request), Item.Bomb);
                return Content(HttpStatusCode.NoContent, "");
            });
        }

        [Route("multiplier/{multiply}/{betId}")]
        [HttpPut]
        public async Task<IHttpActionResult> UseMultiplier(int multiply,string betId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                var itemToUse = "";
                switch (multiply)
                {
                    case 2:
                        itemToUse = Item.MultiplyByTwo;
                        break;
                    case 5:
                        itemToUse = Item.MultiplyByFive;
                        break;
                    case 10:
                        itemToUse = Item.MultiplyByTen;
                        break;
                    default:
                        return Content(HttpStatusCode.BadRequest, "");
                }
                await ItemManager.UseMultiplier(betId, multiply, user);
             
                await Singleton.Instance.UserDao.RemoveUserItem(await GetUserByToken(Request), itemToUse);
                return Content(HttpStatusCode.NoContent, "");
            });
        }

        [Route("key/{userId}")]
        [HttpGet]
        public async Task<IHttpActionResult> UseKey(string userId)
        {
            return await HandleError(async () =>
            { 
                await Singleton.Instance.UserDao.RemoveUserItem(await GetUserByToken(Request), Item.Key);
                return await HandleNotFound(async () => Ok(await GetUserDao().FindUser(userId)));
            });
        }

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
