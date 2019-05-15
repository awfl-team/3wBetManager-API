using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class ItemDao : IItemDao
    {
        private readonly IMongoCollection<Item> _collection;

        public ItemDao(IMongoDatabase database, IMongoCollection<Item> collection = null)
        {
            _collection = collection ?? database.GetCollection<Item>("item");
        }

        public async Task<List<Item>> FindAllItems()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<Item>> FindItemsFiltered(string itemType)
        {
            return await _collection.Find(i => i.Type != itemType).ToListAsync();
        }

        public async Task<Item> FindItem(string id)
        {
            var result = await _collection.Find(i => i.Id == ObjectId.Parse(id)).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task AddListItem(List<Item> items)
        {
            await _collection.InsertManyAsync(items);
        }

        public async Task PurgeItemCollection()
        {
            await _collection.DeleteManyAsync(i => i.Id != null);
        }

        public async Task UpdateItem(string id, Item item)
        {
            await _collection.UpdateOneAsync(
                i => i.Id == ObjectId.Parse(id),
                Builders<Item>.Update.Set(i => i.Cost, item.Cost)
                    .Set(i => i.Rarity, item.Rarity)
            );
        }
    }
}