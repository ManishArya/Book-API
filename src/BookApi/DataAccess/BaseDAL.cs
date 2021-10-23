using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookApi.models;
using MongoDB.Driver;

namespace BookApi.DataAccess
{
    public class BaseDAL<T> : IBaseDAL<T> where T : Base
    {
        protected readonly IMongoCollection<T> _collections;
        public BaseDAL(IDatabaseClient client, string collectionName)
        {
            _collections = client.Database.GetCollection<T>(collectionName);
        }

        public async Task<string> Save(T baseObject)
        {
            baseObject.CreatedAt = DateTime.Now;
            baseObject.CreatedBy = "";

            await _collections.InsertOneAsync(baseObject);
            return "saved successfully";
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            var result = await _collections.FindAsync(c => true);
            return await result.ToListAsync();
        }

        public async Task<T> GetById(string id) => await (await _collections.FindAsync(c => c.Id == id))?.FirstAsync<T>();

        public async Task<string> Remove(string id)
        {
            await _collections.DeleteOneAsync(c => c.Id == id);
            return "";
        }
    }
}