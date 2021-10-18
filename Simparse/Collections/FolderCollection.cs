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

    public interface IFolderCollection
    {
        Task<List<FolderDataModel>> GetFolderList(string userId);

        Task<FolderDataModel> GetFolderData(string folderId);

        Task CreateFolder(string userId, string name);

        Task SaveFolderData(FolderDataModel folder);

        Task DeleteFolder(string folderId);

    }

    public class FolderCollection : IFolderCollection
    {
        private readonly IMongoCollection<FolderDataModel> _collection;

        public FolderCollection(IMongoCollection<FolderDataModel> collection)
        {
            _collection = collection;
        }

        public async Task CreateFolder(string userId, string name)
        {
            var newRecord = new FolderDataModel()
            {
                DateCreated = DateTime.UtcNow,
                Name = name,
                UserId = userId
            };

            await _collection.InsertOneAsync(newRecord).ConfigureAwait(false);
        }

        public async Task DeleteFolder(string folderId)
        {
            var filter = Builders<FolderDataModel>.Filter.Eq(m => m.Id, folderId);
            var action = await _collection.DeleteOneAsync(filter).ConfigureAwait(false);
        }

        public async Task<FolderDataModel> GetFolderData(string folderId)
        {
            var filter = Builders<FolderDataModel>.Filter.Eq(m => m.Id, folderId);
            var action = await _collection.FindAsync(filter).ConfigureAwait(false);
            var result = await action.FirstOrDefaultAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<List<FolderDataModel>> GetFolderList(string userId)
        {
            var filter = Builders<FolderDataModel>.Filter.Eq(m => m.UserId, userId);
            var action = await _collection.FindAsync(filter).ConfigureAwait(false);
            var result = await action.ToListAsync().ConfigureAwait(false);
            return result;
        }

        public async Task SaveFolderData(FolderDataModel folder)
        {
            if (!string.IsNullOrWhiteSpace(folder.Id))
            {
                var previousFolderData = await GetFolderData(folder.Id).ConfigureAwait(false);
                previousFolderData.Name = folder.Name;
                var filter = Builders<FolderDataModel>.Filter.Eq(m => m.Id, folder.Id);
                await _collection.FindOneAndReplaceAsync(filter, previousFolderData).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("Cannot save a folder data object without providing a mongo id.");
            }
        }
    }

    public class FolderDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
    }


}
