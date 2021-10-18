using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Simparse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simparse.Collections
{

    public interface IUserCollection
    {
        Task<ApplicationUser> GetUserById(string id);

        Task<ApplicationUser> GetUserByIdentityId(string id);

        Task<ApplicationUser> CreateUser(ApplicationUser user);

        Task UpdateUser(ApplicationUser newUserInstance);

    }

    public class UserCollection : IUserCollection
    {
        private IMongoCollection<ApplicationUser> _collection;

        public UserCollection(IMongoCollection<ApplicationUser> collection)
        {
            _collection = collection;
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq(m => m.Id, id);
            var result = await _collection.FindAsync(filter).ConfigureAwait(false);
            var user = await result.FirstOrDefaultAsync().ConfigureAwait(false);
            return user;
        }

        public async Task<ApplicationUser> GetUserByIdentityId(string id)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq(m => m.IdentityId, id);
            var result = await _collection.FindAsync(filter).ConfigureAwait(false);
            var user = await result.FirstOrDefaultAsync().ConfigureAwait(false);
            return user;
        }

        public async Task<ApplicationUser> CreateUser(ApplicationUser user)
        {
            await _collection.InsertOneAsync(user).ConfigureAwait(false);
            return user;
        }

        public async Task UpdateUser(ApplicationUser newUserInstance)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq(m => m.Id, newUserInstance.Id);
            await _collection.ReplaceOneAsync(filter, newUserInstance);
        }


    }

    public class ApplicationUser
    {

        public ApplicationUser()
        {

        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string IdentityId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLineOne { get; set; }

        public string AddressLineTwo { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
