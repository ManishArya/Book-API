using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using BookApi.enums;

namespace BookApi.DataAccess
{
    public class BookDAL : BaseDAL<Book>
    {
        public BookDAL(IHttpContextAccessor contextAccessor, IDatabaseClient client) : base(contextAccessor, client, "books")
        {

        }

        public override Task<bool> Save(Book book)
        {
            if (_isAdmin || _permissions.Contains(RolePermission.AddBook))
            {
                return base.Save(book);
            }

            throw new SecurityException("Permission denied");
        }

        public override Task<bool> Remove(IEnumerable<string> ids)
        {
            if (_isAdmin || _permissions.Contains(RolePermission.DeleteBook))
            {
                var filterDefinition = FilterBuilder.In(f => f.Id, ids);
                return RemoveMany(filterDefinition);
            }

            throw new SecurityException("Permission denied");
        }
    }
}