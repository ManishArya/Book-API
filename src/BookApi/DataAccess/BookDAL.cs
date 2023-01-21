using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using BookApi.models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using BookApi.enums;
using MongoDB.Driver;
using System;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using BookApi.Utils;

namespace BookApi.DataAccess
{
    public class BookDAL : BaseDAL<Book>, IBookDAL
    {
        public BookDAL(IHttpContextAccessor contextAccessor, IBookDBContext context) : base(contextAccessor, context) { }

        public async Task<(IEnumerable<Book>, long)> GetBooksAsync(BookCriteria criteria)
        {
            var filters = FilterBuilder.Empty;
            var searchText = criteria.SearchText;

            if (!string.IsNullOrEmpty(searchText))
            {
                var exp = new BsonRegularExpression(new Regex(searchText, RegexOptions.IgnoreCase));
                filters &= FilterBuilder.Or(FilterBuilder.Regex(f => f.Name, exp), FilterBuilder.Regex(f => f.Author, exp));
            }

            var filtersCriteria = criteria.FiltersCriteria;
            if (filtersCriteria != null)
            {
                foreach (var filterCriteria in filtersCriteria)
                {
                    var value1 = filterCriteria.Value1;
                    var value2 = filterCriteria.Value2;
                    var expression = ExpressionBuilder.GetExpression<Book, object>(filterCriteria.Column);

                    switch (filterCriteria.Operator)
                    {
                        case FilterOperator.Between:
                            filters &= FilterBuilder.And(FilterBuilder.Gte(expression, value1), FilterBuilder.Lte(expression, value2));
                            break;
                        case FilterOperator.GreaterThan:
                            filters &= FilterBuilder.Gt(expression, value1);
                            break;
                        default:
                            throw new InvalidOperationException("Invalid Operator !!!");
                    }
                }
            }

            var pageSize = criteria.PageSize;
            var findOptions = new FindOptions<Book>()
            {
                Limit = pageSize,
                Skip = (criteria.CurrentPage - 1) * pageSize
            };

            var sortCriteria = criteria.SortCriteria;
            if (sortCriteria?.ascending != null)
            {
                var expression = ExpressionBuilder.GetExpression<Book, object>(sortCriteria.column);
                var isAscending = sortCriteria.ascending.Value;
                findOptions.Sort = isAscending ? SortDefinitionBuilder.Ascending(expression) : SortDefinitionBuilder.Descending(expression);
            }

            var countTask = CountDocumentsAsync(filters);
            var bookTask = GetDocumentsAsync(filters, findOptions);
            await Task.WhenAll(countTask, bookTask);

            return (bookTask.Result, countTask.Result);
        }

        public override async Task SaveDocumentAsync(Book book)
        {
            if (!HasPermission(RolePermission.AddBook))
            {
                throw new SecurityException("Permission denied");
            }

            await base.SaveDocumentAsync(book);
        }

        public async Task<bool> DeleteBookAsync(IEnumerable<string> ids)
        {
            if (!HasPermission(RolePermission.DeleteBook))
            {
                throw new SecurityException("Permission denied");
            }

            var filterDefinition = FilterBuilder.In(f => f.Id, ids);
            return await DeleteManyDocumentsAsync(filterDefinition);

        }

        private bool HasPermission(RolePermission permission) => _isAdmin || _permissions.Contains(permission);
    }
}