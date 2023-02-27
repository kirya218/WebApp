using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace WebApp.Tools.Helpers
{
    /// <summary>
    /// ovld.js client validation helpers
    /// </summary>
    public static class OVldHelpers
    {
        /// <summary>
        /// set this action to add additional attributes to ovld rules transformation code
        /// </summary>
        public static Action<IDictionary<string, string>, IDictionary<string, string>> attrToRules;

        /// <summary>
        /// add js script for ovld.bind client validation
        /// </summary>
        /// <returns></returns>
        public static IHtmlContent BindOVld<TModel>(this IHtmlHelper html, string selector, Action<VldRulesBld> addRules = null)
        {
            var result = string.Empty;
            var cattributes = GetVldAttrs<TModel>(html);
            var related = string.Empty;

            foreach (var prop in typeof(TModel).GetProperties())
            {
                if (!cattributes.ContainsKey(prop.Name)) continue;

                var attributes = cattributes[prop.Name];

                var rules = new Dictionary<string, string>();

                var kreq = "data-val-required";
                var klen = "data-val-length";
                var klenmax = "data-val-length-max";
                var klenmin = "data-val-length-min";

                if (attributes.ContainsKey(kreq))
                {
                    var msg = attributes[kreq];
                    rules.Add("ovld.rules.req", msg);
                }

                if (attributes.ContainsKey(klen))
                {
                    var msg = attributes[klen];
                    var max = attributes.ContainsKey(klenmax) ? attributes[klenmax] : string.Empty;
                    var min = attributes.ContainsKey(klenmin) ? attributes[klenmin] : string.Empty;
                    rules.Add(string.Format("ovld.rules.len({0}, {1})", max, min), msg);
                }

                if (addRules!= null)
                {
                    var bld = new VldRulesBld();
                    addRules(bld);
                    var additRules = bld.AdditRules;
                    
                    if (additRules.ContainsKey(prop.Name))
                    {
                        foreach (var aditRule in additRules[prop.Name])
                        {
                            rules.Add(aditRule.Key, aditRule.Value);
                        }
                    }

                    related = JsonConvert.SerializeObject(bld.Related);
                }

                if (attrToRules != null)
                {
                    attrToRules(attributes, rules);
                }

                if (!rules.Any()) continue;

                result += string.Format("'{0}': [", prop.Name);

                result = rules.Select(kv => "chk:" + kv.Key + ", msg:'" + JsEncode(kv.Value) + "'").Aggregate(result, (current, s) => current + "{" + s + "},");

                result += "],";
            }

            return new HtmlString("<script>$(function(){ovld.bind({ " +
                                     "subev: 'aweinlinebfsave', " +
                                     string.Format("sel: '{0}', rules:{{{1}}}, ", selector, result) +
                                     (related != null ? "related: " + related + ", " : string.Empty) + 
                                     "msgCont: function(o){ return o.inp.closest('.awe-row').find('[vld-for=' + o.name + ']'); } " +
                                     "}); });</script>");
        }

        internal static string JsEncode(string s)
        {
            if (s == null) return null;
            return s.Replace("'", "\\'").Replace("\"", "&quot;");
        }

        private static IDictionary<string, IDictionary<string, string>> GetVldAttrs<TModel>(IHtmlHelper html)
        {
            var res = new Dictionary<string, IDictionary<string, string>>();

            var attributeProvider = html.ViewContext.HttpContext.RequestServices.GetRequiredService<ValidationHtmlAttributeProvider>();
            var modelExplorer = html.MetadataProvider.GetModelExplorerForType(typeof(TModel), null);

            foreach (var prop in typeof(TModel).GetProperties())
            {
                var clientAttributes = new Dictionary<string, string>();

                attributeProvider.AddValidationAttributes(html.ViewContext, modelExplorer.GetExplorerForProperty(prop.Name), clientAttributes);

                if (clientAttributes.Count > 0)
                {
                    res.Add(prop.Name, clientAttributes);
                }
            }

            return res;
        }
    }
}