﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Hub;
using Models;

namespace _3wBetManager_API.Controllers
{
    [IsGranted]
    [RoutePrefix("items")]
    public class ItemController : BaseController
    {
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> BuyItems(List<Item> items)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);

                await GetItemManager().BuyItemsToUser(items, user);


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

                var items = await GetItemManager().AddItemsToUser(user);
                await GetUserManager().DeleteUserItem(await GetUserByToken(Request), Item.LootBox);
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

                var item = await GetItemManager().AddMysteryItemToUser(user);
                await GetUserManager().DeleteUserItem(await GetUserByToken(Request), Item.Mystery);
                return Created("", item);
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


                var sendTo = await GetItemManager().UseBomb(userId);
                notificationHub.SendNotification(sendTo.Username, user.Username + " used a bomb on you");


                await GetUserManager().DeleteUserItem(user, Item.Bomb);
                return Content(HttpStatusCode.NoContent, "");
            });
        }

        [Route("multiplier/{multiply}/{betId}")]
        [HttpPut]
        public async Task<IHttpActionResult> UseMultiplier(int multiply, string betId)
        {
            string itemToUse;
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
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


                await GetItemManager().UseMultiplier(betId, multiply, user);

                await GetUserManager().DeleteUserItem(await GetUserByToken(Request), itemToUse);
                return Content(HttpStatusCode.NoContent, "");
            });
        }

        [Route("key/{userId}")]
        [HttpGet]
        public async Task<IHttpActionResult> UseKey(string userId)
        {
            return await HandleNotFound(async () =>
            {
                var sendTo = await GetUserManager().GetUser(userId);
                var user = await GetUserByToken(Request);
                var notificationHub = new NotificationHub();
                notificationHub.SendNotification(sendTo.Username, user.Username + " used a key on you");
                await GetUserManager().DeleteUserItem(await GetUserByToken(Request), Item.Key);
                return Ok(sendTo);
            });
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            return await HandleError(async () => Ok(await GetItemManager().GetAllItems()));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateItem(string id, [FromBody] Item item)
        {
            return await HandleError(async () =>
            {
                await GetItemManager().ChangeItem(id, item);
                return Ok();
            });
        }
    }
}