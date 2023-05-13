using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using GridLibrary;
using System.Reflection;
using WebApp.Context;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Tools
{
    public static class GridUtils
    {
        private static string GetPopupName<T>(this IHtmlHelper<T> html, string action, string gridId)
        {
            return action + html.Awe().GetContextPrefix() + gridId;
        }

        public static IHtmlContent CreateButtonForGrid<T>(this IHtmlHelper<T> html, string gridId, string text = "Create")
        {
            return html.Awe().Button()
                .Text(text)
                .OnClick(html.Awe().OpenPopup(html.GetPopupName("create", gridId)));
        }

        public static string EditFormatForGrid<T>(this IHtmlHelper<T> html, string gridId, string popapName = "edit", string key = "Id", bool setId = false, bool nofocus = false, int? height = null)
        {
            var popupName = html.GetPopupName(popapName, gridId);

            var click = html.Awe().OpenPopup(popupName).Params(new { id = $".({key})" });
            if (height.HasValue)
            {
                click.Height(height.Value);
            }

            var button = html.Awe().Button()
                .CssClass("awe-nonselect editbtn")
                .HtmlAttributes(new { aria_label = popapName })
                .Text("<span class='ico-crud ico-edit'></span>")
                .OnClick(click);

            var attrdict = new Dictionary<string, object>();

            if (setId)
            {
                attrdict.Add("id", $"gbtn{popupName}.{key}");
            }

            if (nofocus)
            {
                attrdict.Add("tabindex", "-1");
            }

            button.HtmlAttributes(attrdict);

            return button.ToString();
        }

        public static string DeleteFormatForGrid<T>(this IHtmlHelper<T> html, string gridId, string key = "Id", bool nofocus = false)
        {
            gridId = html.Awe().GetContextPrefix() + gridId;

            return DeleteFormat("delete" + gridId, key, null, null, nofocus);
        }

        public static string DeleteFormat(string popupName, string key = "Id", string deleteContent = null, string btnClass = null, bool nofocus = false)
        {
            deleteContent ??= "<span class='ico-crud ico-del'></span>";

            var tabindex = nofocus ? "tabindex = \"-1\"" : string.Empty;

            return string.Format("<button aria-label=\"delete\" type=\"button\" class=\"awe-btn awe-nonselect delbtn {3}\"" +
                                 " {4} onclick=\"awe.open('{0}', {{ params:{{ id: '.({1})' }} }}, event)\">{2}</button>",
                popupName, key, deleteContent, btnClass, tabindex);
        }

        public static IReadOnlyCollection<Type> GetTypes(Type baseType)
        {
            return Assembly
                .GetAssembly(baseType)
                .GetTypes()
                .Where(type => type.IsSubclassOf(baseType))
                .ToList();
        }

        public static Guid GetContactId(Guid userId, WebAppContext context) => context.Users.Include(x => x.Contact)?.FirstOrDefault(x => x.Id == userId)?.Contact?.Id ?? Guid.Empty;
    }
}