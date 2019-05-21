using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Manager.Interfaces
{
    public interface IItemManager
    {
        Task BuyItemsToUser(List<Item> items, User user);
        Task<List<Item>> AddItemsToUser(User user);
        Task<Item> AddMysteryItemToUser(User user);
        Task<User> UseBomb(string userId);
        Task UseMultiplier(string betId, int multiply, User currentUser);
        Task<List<Item>> GenerateLoot(string itemType);
        Task CreateDefaultItems();
        Task<List<Item>> GetAllItems();
        Task ChangeItem(string id, Item item);
    }
}