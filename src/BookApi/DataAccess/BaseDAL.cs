using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookApi.DataAccess
{
    public class BaseDAL<T> : IBaseDAL<T> where T : Base
    {
        protected readonly string _username;

        protected readonly bool isAdmin;
        protected readonly IMongoCollection<T> _collections;

        public BaseDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client, string collectionName)
        {
            _username = contextAccessor.HttpContext.User.Identity.Name;
            isAdmin = bool.Parse(contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "isAdmin")?.Value);
            _collections = client.Database.GetCollection<T>(collectionName);
        }

        public virtual async Task<bool> Save(T baseObject)
        {
            baseObject.CreatedAt = DateTime.Now;
            baseObject.CreatedBy = _username;

            await _collections.InsertOneAsync(baseObject);
            return true;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            var result = await _collections.FindAsync(c => true);
            return await result.ToListAsync();
        }

        public async Task<T> GetById(string id)
        {
            var objectId = GetObjectId(id);
            return await (await _collections.FindAsync(c => c.Id == objectId.ToString()))?.FirstAsync<T>();
        }

        public virtual async Task<bool> Remove(string id)
        {
            await _collections.DeleteOneAsync(c => c.Id == id);
            return true;
        }

        public virtual async Task<bool> Remove(List<string> ids)
        {
            foreach (var id in ids)
            {
                await Remove(id);
            }
            return true;
        }

        protected ObjectId GetObjectId(string id)
        {
            if (ObjectId.TryParse(id, out ObjectId objectId))
            {
                return objectId;
            }

            throw new InvalidOperationException("Invalid id");
        }
    }
}