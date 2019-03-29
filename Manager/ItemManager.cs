using System.Collections.Generic;
using System.Threading.Tasks;
using DAO;
using Models;

namespace Manager
{
    public class ItemManager
    {
        public static async Task AddListItemsToUser(List<Item> items, User user)
        {
            var totalCost = 0;
            foreach (var item in items)
            {
                await Singleton.Instance.UserDao.AddUserItem(item, user);
                totalCost += item.Cost;
            }

            await Singleton.Instance.UserDao.UpdateUserPoints(user, (user.Point - totalCost),
                user.TotalPointsUsedToBet);
        }
    }
}