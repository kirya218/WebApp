using System;
using GridLibrary;

namespace WebApp.Tools.Helpers
{
    /// <summary>
    /// awesome mod helpers
    /// </summary>
    public static class AwesomeModHelpers
    {
        /// <summary>
        /// Odropdown Awesome Mod 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ahtml"></param>
        /// <param name="prop"></param>
        /// <param name="setCfg"></param>
        /// <returns></returns>
        public static AjaxRadioList<T> Odropdown<T>(this AwesomeHtmlHelper<T> ahtml, string prop, Action<OddCfg> setCfg = null)
        {
            var res = ahtml.AjaxRadioList(prop).Mod("awem.odropdown");
            var odcfg = new OddCfg();

            if (setCfg != null)
            {
                setCfg(odcfg);
                res.Tag(odcfg.ToTag());
            }

            return res;
        }
    }
}