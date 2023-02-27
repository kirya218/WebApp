namespace WebApp.Tools.Helpers
{
    /// <summary>
    /// validation rules builder
    /// </summary>
    public class VldRulesBld
    {
        readonly IDictionary<string, IDictionary<string,string>> additRules = new Dictionary<string, IDictionary<string, string>>();

        IDictionary<string, List<string>> related = new Dictionary<string, List<string>>();

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, IDictionary<string, string>> AdditRules
        {
            get { return additRules; }
        }

        /// <summary>
        /// Get related rules
        /// </summary>
        public IDictionary<string, List<string>> Related
        {
            get { return related; }
        }

        /// <summary>
        /// add validation rules for property
        /// </summary>
        public VldRulesBld Prop(string name, IDictionary<string, string> ruleNamesMsgs)
        {
            AdditRules.Add(name, ruleNamesMsgs);
            return this;
        }

        /// <summary>
        /// add validation rules for property
        /// </summary>
        public VldRulesBld Prop(string name, string ruleName, string message)
        {
            if (!additRules.ContainsKey(name))
            {
                additRules.Add(name, new Dictionary<string, string>());
            }

            var rules = additRules[name];

            rules.Add(ruleName, message);

            return this;
        }

        /// <summary>
        /// Add related property
        /// </summary>
        public VldRulesBld Relate(string name, string prop)
        {
            if (!related.ContainsKey(name))
            {
                related.Add(name, new List<string>());
            }

            var lst = related[name];

            lst.Add(prop);

            return this;
        }
    }
}