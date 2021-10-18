using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Simparse.Enums;
using Simparse.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Collections
{
    public interface IFileMappingCollection
    {
        Task Insert(FileMappingItem item);

        Task Delete(string id);

        Task<List<FileMappingItem>> GetByFolderId(string folderId);

        Task<List<FileMappingItem>> GetByFolderId(string folderId, int pageNumber);

        Task UpdateFileMappingItem(FileMappingItem item);
    }

    public class FileMappingCollection : IFileMappingCollection
    {
        private readonly IMongoCollection<FileMappingItem> _collection;

        public FileMappingCollection(IMongoCollection<FileMappingItem> collection)
        {
            _collection = collection;
        }

        public async Task Insert(FileMappingItem item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task Delete(string id)
        {
            var filter = Builders<FileMappingItem>.Filter.Eq(m => m.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<List<FileMappingItem>> GetByFolderId(string folderId)
        {
            var builder = Builders<FileMappingItem>.Filter;
            var filter = builder.Eq(m => m.FolderId, folderId);
            var action = await _collection.FindAsync(filter);
            var result = await action.ToListAsync();
            return result;
        }


        public async Task<List<FileMappingItem>> GetByFolderId(string folderId, int pageNumber)
        {
            var builder = Builders<FileMappingItem>.Filter;
            var filter = builder.Eq(m => m.FolderId, folderId) & builder.Eq(m => m.PageNumber, pageNumber);
            var action = await _collection.FindAsync(filter);
            var result = await action.ToListAsync();
            return result;
        }

        public async Task UpdateFileMappingItem(FileMappingItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.Id))
            {
                var filter = Builders<FileMappingItem>.Filter.Eq(m => m.Id, item.Id);
                var updateName = Builders<FileMappingItem>.Update.Set(m => m.Name, item.Name);
                await _collection.UpdateOneAsync(filter, updateName);
                var updateType = Builders<FileMappingItem>.Update.Set(m => m.Type, item.Type);
                await _collection.UpdateOneAsync(filter, updateType);
            }
        }
    }

    public class FileMappingItem
    {

        public FileMappingItem()
        {

        }

        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string FolderId { get; set; }

        public int PageNumber { get; set; }

        public List<VisionVerticie> Items { get; set; }

        public FieldValueType Type { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
