using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAO.Interfaces
{
    public interface IItemDao
    {
        Task<List<Item>> FindAllItems();
        Task<List<Item>> FindItemsFiltered(string itemType);
        Task<Item> FindItem(string id);
        Task AddListItem(List<Item> items);
        Task PurgeItemCollection();
        Task UpdateItem(string id, Item item);
    }
}
