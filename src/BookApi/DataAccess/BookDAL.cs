using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using BookApi.enums;

namespace BookApi.DataAccess
{
    public class BookDAL : BaseDAL<Book>, IBookDAL
    {
        public BookDAL(IHttpContextAccessor contextAccessor, IBookDBContext context) : base(contextAccessor, context) { }

        public override async Task SaveAsync(Book book)
        {
            if (!HasPermission(RolePermission.AddBook))
            {
                throw new SecurityException("Permission denied");
            }

            await base.SaveAsync(book);
        }

        public async Task<bool> RemoveAsync(IEnumerable<string> ids)
        {
            if (!HasPermission(RolePermission.DeleteBook))
            {
                throw new SecurityException("Permission denied");
            }

            var filterDefinition = FilterBuilder.In(f => f.Id, ids);
            return await RemoveMany(filterDefinition);

        }

        private bool HasPermission(RolePermission permission) => _isAdmin || _permissions.Contains(permission);
    }
}