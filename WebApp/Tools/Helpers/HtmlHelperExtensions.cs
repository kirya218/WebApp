using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Html;
using Omu.AwesomeMvc;

namespace WebApp.Tools.Helpers
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// �������������� awesome � aweui, 
        /// ��������� ������, ������� ����� ���������� �������� �� ��������� ��� awem.js � jquery.validate; 
        /// ������ ������ ����, ������ ���� ������, ���������� �����������
        /// </summary>
        /// <returns>������ HTML.</returns>
        public static IHtmlContent Initialization()
        {
            var arg = AweUtil.ConvertTojQueryDateFormat(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            var numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var stringBuilder = new StringBuilder("<script>");
            stringBuilder.AppendFormat("awem.isMobileOrTablet = function() {{ return {0}; }};", 1);
            stringBuilder.AppendFormat("awem.fdw = {0};", (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            stringBuilder.AppendFormat("utils.init('{0}', {1}, '{2}');", arg, 1, numberDecimalSeparator);
            stringBuilder.AppendFormat("if(window.aweui){{aweui.init(); aweui.decimalSep = '{0}';}}", numberDecimalSeparator);
            stringBuilder.Append("awe.mdl = [awem, utils]");
            stringBuilder.Append("</script>");
            return new HtmlString(stringBuilder.ToString());
        }

        /// <summary>
        /// ��� �������� � head, ����� �������� ������ jquery ������ ���� head.
        /// </summary>
        /// <returns>������ HTML.</returns>
        public static IHtmlContent JQueryInitialization()
        {
            var stringBuilder = new StringBuilder("<script>");
            stringBuilder.Append("var awejqfuncs = [];if (!jQuery) {var jQuery = function (fn) { awejqfuncs.push(fn); };var $ = jQuery;}");
            stringBuilder.Append("</script>");
            return new HtmlString(stringBuilder.ToString());
        }
    }
}