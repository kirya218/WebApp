namespace WebApp.Builders.Filters
{
    public class FilterBuilder<T>
    {
        private readonly IDictionary<string, FilterRule<T>> frules = new Dictionary<string, FilterRule<T>>();

        public FilterBuilder<T> Add(string key, FilterRule<T> rule)
        {
            frules.Add(key, rule);
            return this;
        }

        public FilterBuilder<T> Add(string key, Func<IQueryable<T>, IQueryable<T>> filter)
        {
            frules.Add(key, new FilterRule<T> { Query = filter });
            return this;
        }

        public async Task<IQueryable<T>> ApplyAsync(IQueryable<T> query, string[] forder)
        {
            foreach (var prop in forder)
            {
                if (!frules.ContainsKey(prop)) continue;
                var rule = frules[prop];
                var nquery = rule.Query(query);

                if (rule.Data != null)
                {
                    await rule.Data(rule.SelfQuery ? nquery : query);
                }

                query = nquery;
            }

            foreach (var pair in frules.Where(o => !forder.Contains(o.Key)))
            {
                var rule = pair.Value;
                if (rule.Data != null)
                {
                    await rule.Data(query);
                }
            }

            return query;
        }
    }
}
