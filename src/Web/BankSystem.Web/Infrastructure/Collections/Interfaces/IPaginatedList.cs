namespace BankSystem.Web.Infrastructure.Collections.Interfaces
{
    public interface IPaginatedList
    {
        int PageIndex { get; }

        int TotalPages { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }

        int SurroundingPagesCount { get; }
    }
}