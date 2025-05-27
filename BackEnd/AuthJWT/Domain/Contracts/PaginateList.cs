namespace AuthJWT.Domain.Contracts
{
    public class PaginateList<T> where T : class
    {
        public PaginateList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage => (PageIndex > 1);
        public bool HasNextPage => (PageIndex < TotalPages);

        public IEnumerable<T> Items { get; set; }

        public static PaginateList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginateList<T>(items, count, pageIndex, pageSize);
        }
    }
}
