namespace WebApp.Builders.Filters
{
    public class FilterRule<T>
    {
        public Func<IQueryable<T>, IQueryable<T>> Query { get; set; }

        public Func<IQueryable<T>, Task> Data { get; set; }

        public bool SelfQuery { get; set; }
    }
}
