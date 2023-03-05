using Microsoft.EntityFrameworkCore;
using PagedList;

namespace GondolinWeb.Areas.Application.Models
{
    /// <summary>
    /// The class representing the paginated list.
    /// </summary>
    /// <typeparam name="T">the type of list.</typeparam>
    public class PaginatedList<T> : List<T>, IPagedList
    {
        /// <summary>
        /// Initializes a new instance of the PaginatedList class.
        /// </summary>
        /// <param name="items">list of items.</param>
        /// <param name="count">the count.</param>
        /// <param name="pageIndex">page index.</param>
        /// <param name="pagSize">page size.</param>
        public PaginatedList(List<T> items, int count, int pageIndex, int pagSize)
        {
            this.PageIndex = pageIndex;
            this.TotalPages = (int)Math.Ceiling(count / (double)pagSize);
            this.AddRange(items);
        }

        /// <summary>
        /// Gets the page index.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets the total pages.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it has previous page.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Gets a value indicating whether it has next page.
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        public int PageCount => throw new NotImplementedException();

        public int TotalItemCount => throw new NotImplementedException();

        public int PageNumber => throw new NotImplementedException();

        public int PageSize => throw new NotImplementedException();

        public bool IsFirstPage => throw new NotImplementedException();

        public bool IsLastPage => throw new NotImplementedException();

        public int FirstItemOnPage => throw new NotImplementedException();

        public int LastItemOnPage => throw new NotImplementedException();

        /// <summary>
        /// Creates a new page list.
        /// </summary>
        /// <param name="source">the item source.</param>
        /// <param name="pageIndex">the page index.</param>
        /// <param name="pagSize">the page size.</param>
        /// <returns>a new page list.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pagSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pagSize).Take(pagSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pagSize);
        }
    }
}