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

    public interface IFileCollection
    {
        Task<FileDataModel> GetFileById(string id);

        Task<List<FileDataModel>> GetFileListByFolder(string folderId, bool ignorePages);

        Task InsertNewFile(FileDataModel model);

        Task UpdateFileData(FileDataModel model);

        Task DeleteFile(string id);
    }


    public class FileCollection : IFileCollection
    {
        private readonly IMongoCollection<FileDataModel> _collection;

        public FileCollection(IMongoCollection<FileDataModel> collection)
        {
            _collection = collection;
        }

        public async Task DeleteFile(string id)
        {
            var filter = Builders<FileDataModel>.Filter.Eq(m => m.Id, id);
            var action = await _collection.DeleteOneAsync(filter).ConfigureAwait(false);
        }

        public async Task<FileDataModel> GetFileById(string id)
        {
            var filter = Builders<FileDataModel>.Filter.Eq(m => m.Id, id);
            var action = await _collection.FindAsync(filter).ConfigureAwait(false);
            var result = await action.FirstOrDefaultAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<List<FileDataModel>> GetFileListByFolder(string folderId, bool ignorePages)
        {
            var filter = Builders<FileDataModel>.Filter.Eq(m => m.FolderId, folderId);
            if (ignorePages)
            {
                var projection = Builders<FileDataModel>.Projection.Exclude(m => m.Pages);
                FindOptions<FileDataModel, FileDataModel> opt = new FindOptions<FileDataModel, FileDataModel>()
                {
                    Projection = projection
                };
                var action = await _collection.FindAsync(filter, opt).ConfigureAwait(false);
                var result = await action.ToListAsync().ConfigureAwait(false);
                return result;
            }
            else
            {
                var action = await _collection.FindAsync(filter).ConfigureAwait(false);
                var result = await action.ToListAsync().ConfigureAwait(false);
                return result;
            }

        }

        public async Task InsertNewFile(FileDataModel data)
        {
            await _collection.InsertOneAsync(data).ConfigureAwait(false);
        }

        public async Task UpdateFileData(FileDataModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Id))
            {
                var filter = Builders<FileDataModel>.Filter.Eq(m => m.Id, model.Id);
                await _collection.ReplaceOneAsync(filter, model).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("Model must contain id to update file data");
            }

        }
    }

    public class FileDataModel
    {
        public FileDataModel()
        {
            Pages = new List<PageItemDataModel>();
        }


        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FolderId { get; set; }

        public Guid FileId { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public long Size { get; set; }

        public IconFileType IconFileType { get; set; }

        public string Extension { get; set; }

        public int PageCount { get; set; }

        public List<PageItemDataModel> Pages { get; set; }

    }

    public class PageItemDataModel
    {
        public PageItemDataModel()
        {
            WordItems = new List<WordItemDataModel>();
        }

        public int PageNumber { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<WordItemDataModel> WordItems { get; set; }
    }

    public class WordItemDataModel
    {

        public string FontStyle { get; set; }

        public string Text { get; set; }

        public Guid Id { get; set; }

        public decimal XAxis { get; set; }

        public decimal YAxis { get; set; }

        public List<VisionVerticie> Vertices { get; set; }

        public int TextSize { get; set; }

    }

}
