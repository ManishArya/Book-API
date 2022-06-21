using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using BookApi.enums;

namespace BookApi.DataAccess
{
    public class BaseDAL<T> : IBaseDAL<T> where T : Base
    {
        protected readonly string _username;

        protected readonly bool _isAdmin;

        protected readonly IMongoCollection<T> _collections;

        protected readonly RolePermission[] _permissions;

        public BaseDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client, string collectionName)
        {
            var user = contextAccessor.HttpContext.User;
            _username = user.Identity.Name;
            bool.TryParse(user.FindFirst(c => c.Type == "isAdmin")?.Value, out bool result);
            _isAdmin = result;
            _permissions = user.FindAll(u => u.Type == "perms").
                       Select(c => new { convert = Enum.TryParse<RolePermission>(c.Value, out var result), result }).
                       Where(c => c.convert).Select(c => c.result)
                       .ToArray();

            _collections = client.Database.GetCollection<T>(collectionName);
        }

        protected FilterDefinitionBuilder<T> FilterBuilder => Builders<T>.Filter;

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

        public virtual Task<bool> Remove(string id) => throw new NotImplementedException();

        public virtual Task<bool> Remove(IEnumerable<string> ids) => throw new NotImplementedException();

        protected async Task<bool> RemoveMany(FilterDefinition<T> filterDefinition)
        {
            await _collections.DeleteManyAsync(filterDefinition);
            return true;
        }

        protected async Task<bool> RemoveOne(FilterDefinition<T> filterDefinition)
        {
            await _collections.DeleteOneAsync(filterDefinition);
            return true;
        }

        protected async Task<long> CountDocumentsAsync(FilterDefinition<T> filterDefinition) => await _collections.CountDocumentsAsync(filterDefinition);

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