using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
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
                await ItemManager.AddListItemsToUser(items, user);
                return Created("", items);
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
        public async Task<IHttpActionResult> UpdateCost(string id, [FromBody] Item item)
        {
            return await HandleError(async () =>
            {
                await GetItemDao().UpdateCostItem(id, item);
                return Ok();
            });
        }
    }
}
