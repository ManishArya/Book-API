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
    public abstract class BaseDAL<TDocument> : IBaseDAL<TDocument> where TDocument : Base
    {
        protected readonly string _userId;

        protected readonly bool _isAdmin;

        protected readonly IMongoCollection<TDocument> _collections;

        protected readonly RolePermission[] _permissions;

        protected FilterDefinitionBuilder<TDocument> FilterBuilder => Builders<TDocument>.Filter;

        protected UpdateDefinitionBuilder<TDocument> UpdateBuilder => Builders<TDocument>.Update;


        public BaseDAL(IHttpContextAccessor contextAccessor, IBookDBContext context)
        {
            var user = contextAccessor.HttpContext.User;
            _userId = user.Identity.Name;
            bool.TryParse(user.FindFirst(c => c.Type == "isAdmin")?.Value, out this._isAdmin);
            _permissions = user.FindAll(u => u.Type == "perms")
                               .Select(c => new { convert = Enum.TryParse<RolePermission>(c.Value, out var result), result })
                               .Where(c => c.convert)
                               .Select(c => c.result)
                               .ToArray();

            _collections = context.Get<TDocument>("books");
        }

        public virtual async Task SaveAsync(TDocument baseDocument)
        {
            baseDocument.CreatedAt = DateTime.Now;
            baseDocument.CreatedBy = _userId;
            await _collections.InsertOneAsync(baseDocument);
        }

        public virtual async Task<bool> UpdateOne(FilterDefinition<TDocument> filterDefinition, UpdateDefinition<TDocument> updateDefinition)
        {
            updateDefinition = updateDefinition.Set(u => u.LastModifiedAt, DateTime.Now)
                                               .Set(u => u.UpdatedBy, _userId);
            var result = await _collections.UpdateOneAsync(filterDefinition, updateDefinition);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public virtual async Task<IEnumerable<TDocument>> GetAsync()
        {
            var result = await _collections.FindAsync(c => true);
            return await result.ToListAsync();
        }

        public virtual async Task<TDocument> Get(FilterDefinition<TDocument> filterDefinition) => await (await _collections.FindAsync(filterDefinition))?.FirstOrDefaultAsync<TDocument>();

        public virtual async Task<TDocument> GetByIdAsync(string id)
        {
            var objectId = GetObjectId(id);
            var filterDefinition = FilterBuilder.Where(f => f.Id == objectId.ToString());
            return await Get(filterDefinition);
        }

        public virtual async Task<bool> RemoveAllAsync()
        {
            var filterDefinition = FilterBuilder.Where(f => true);
            return await RemoveMany(filterDefinition);
        }

        protected virtual async Task<bool> RemoveMany(FilterDefinition<TDocument> filterDefinition)
        {
            var result = await _collections.DeleteManyAsync(filterDefinition);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        protected virtual async Task<bool> RemoveOne(FilterDefinition<TDocument> filterDefinition)
        {
            var result = await _collections.DeleteOneAsync(filterDefinition);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        protected virtual async Task<long> CountDocumentsAsync(FilterDefinition<TDocument> filterDefinition) => await _collections.CountDocumentsAsync(filterDefinition);

        protected virtual ObjectId GetObjectId(string id)
        {
            if (ObjectId.TryParse(id, out ObjectId objectId))
            {
                return objectId;
            }

            throw new InvalidOperationException("Invalid id");
        }
    }
}