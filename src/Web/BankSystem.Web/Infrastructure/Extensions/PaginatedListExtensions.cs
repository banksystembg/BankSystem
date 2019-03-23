namespace BankSystem.Web.Infrastructure.Extensions
{
    using Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class PaginatedListExtensions
    {
        private const int DefaultSurroundingPageCount = 3;

        public static PaginatedList<T> ToPaginatedList<T>(
            this IEnumerable<T> items,
            int pageIndex,
            int itemsCountPerPage,
            int surroundingPageCount = DefaultSurroundingPageCount)
        {
            pageIndex = Math.Max(1, pageIndex);

            var itemsArray = items as T[] ?? items.ToArray();

            var totalPages = (int) Math.Ceiling(itemsArray.Length / (double) itemsCountPerPage);
            pageIndex = Math.Min(pageIndex, totalPages);

            var itemsToShow = itemsArray
                .Skip((pageIndex - 1) * itemsCountPerPage)
                .Take(itemsCountPerPage)
                .ToList();

            return new PaginatedList<T>(itemsToShow, pageIndex, totalPages, surroundingPageCount);
        }
    }
}
