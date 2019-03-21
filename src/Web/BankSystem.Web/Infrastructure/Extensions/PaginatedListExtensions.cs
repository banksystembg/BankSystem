namespace BankSystem.Web.Infrastructure.Extensions
{
    using Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class PaginatedListExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> items, int pageIndex, int itemsCountPerPage)
        {
            pageIndex = Math.Max(1, pageIndex);

            var totalPages = (int)Math.Ceiling(items.Count() / (double)itemsCountPerPage);
            pageIndex = Math.Min(pageIndex, totalPages);

            var itemsToShow = items
                .Skip((pageIndex - 1) * itemsCountPerPage)
                .Take(itemsCountPerPage)
                .ToList();

            return new PaginatedList<T>(itemsToShow, pageIndex, totalPages);
        }
    }
}
