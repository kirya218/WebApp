using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Omu.AwesomeMvc;

namespace WebApp.Tools
{
    /// <summary>
    /// Добавляет попап окна.
    /// </summary>
    public static class CrudHelpers
    {
        /// <summary>
        /// Инициализирует попап окна.
        /// </summary>
        /// <typeparam name="T">Тип html.</typeparam>
        /// <param name="html">HTML.</param>
        /// <param name="gridId">Идентификатор таблицы.</param>
        /// <param name="crudController">Названия контролера.</param>
        /// <param name="createPopupHeight">Высота окна.</param>
        /// <param name="maxWidth">Максимальная ширина окна.</param>
        /// <returns>Контент HTML.</returns>
        public static IHtmlContent InitCrudPopupsForGrid<T>(this IHtmlHelper<T> html, string gridId, string crudController, int createPopupHeight = 430, int maxWidth = 0)
        {
            gridId = html.Awe().GetContextPrefix() + gridId;
            var url = GetUrlHelper(html);

            return new HtmlString(
                AddCreatePopupForGrid(html, url, gridId, crudController, createPopupHeight, maxWidth) +
                AddEditPopupForGrid(html, url, gridId, crudController, createPopupHeight, maxWidth) +
                AddDeletePopupForGrid(html, url, gridId, crudController));
        }

        private static string AddCreatePopupForGrid<T>(IHtmlHelper<T> html, IUrlHelper url, string gridId, string crudController, int createPopupHeight = 430, int maxWidth = 0)
        {
            return html.Awe()
                    .InitPopupForm()
                    .Name("create" + gridId)
                    .Group(gridId)
                    .Height(createPopupHeight)
                    .MaxWidth(maxWidth)
                    .Url(url.Action("Create", crudController))
                    .Title("Create item")
                    .Modal()
                    .Success("utils.itemCreated('" + gridId + "')")
                    .Success("utils.refreshGrid('" + gridId + "')")
                    .ToString();
        }

        private static string AddEditPopupForGrid<T>(IHtmlHelper<T> html, IUrlHelper url, string gridId, string crudController, int createPopupHeight = 430, int maxWidth = 0)
        {
            return html.Awe()
                    .InitPopupForm()
                    .Name("edit" + gridId)
                    .Group(gridId)
                    .Height(createPopupHeight)
                    .MaxWidth(maxWidth)
                    .Url(url.Action("Edit", crudController))
                    .Title("Edit item")
                    .Modal()
                    .Success("utils.itemEdited('" + gridId + "')")
                    .Success("utils.refreshGrid('" + gridId + "')")
                    .ToString();
        }

        private static string AddDeletePopupForGrid<T>(IHtmlHelper<T> html, IUrlHelper url, string gridId, string crudController)
        {
            return html.Awe()
                    .InitPopupForm()
                    .Name("delete" + gridId)
                    .Group(gridId)
                    .Url(url.Action("Delete", crudController))
                    .Success("utils.itemDeleted('" + gridId + "')")
                    .OnLoad("utils.delConfirmLoad('" + gridId + "')")
                    .Success("utils.refreshGrid('" + gridId + "')")
                    .Height(200)
                    .Modal()
                    .ToString();
        }

        private static IUrlHelper GetUrlHelper<T>(IHtmlHelper<T> html)
        {
            return ((IUrlHelperFactory)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory))).GetUrlHelper(html.ViewContext);
        }
    }
}