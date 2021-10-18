using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Collections
{

    public interface IContactCollection
    {
        Task SaveContact(ContactDataItem item);
    }

    public class ContactCollection : IContactCollection
    {
        private IMongoCollection<ContactDataItem> _collection;

        public ContactCollection(IMongoCollection<ContactDataItem> collection)
        {
            _collection = collection;
        }

        public async Task SaveContact(ContactDataItem item)
        {
            item.DateCreated = DateTime.UtcNow;
            await _collection.InsertOneAsync(item);
        }

    }


    public class ContactDataItem
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string ContactInfo { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
