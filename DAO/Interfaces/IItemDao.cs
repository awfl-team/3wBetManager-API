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
        Task<Item> FindItem(string id);
        Task UpdateCostItem(string id, Item item);
        Task AddListItem(List<Item> items);
        Task PurgeItemCollection();
    }
}
