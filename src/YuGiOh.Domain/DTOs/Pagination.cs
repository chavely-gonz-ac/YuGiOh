namespace YuGiOh.Domain.DTOs
{
    /// <summary>
    /// Represents pagination parameters for querying paged data.
    /// </summary>
    public class Pagination
    {
        private const int MaxPageSize = 100;

        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public Pagination(int page = 1, int pageSize = 10)
        {
            Page = page < 1 ? 1 : page;
            PageSize = pageSize > MaxPageSize ? MaxPageSize : pageSize;
        }

        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
    }
}
