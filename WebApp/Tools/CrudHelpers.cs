using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using GridLibrary;

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
        public static IHtmlContent InitCrudPopupsForGrid<T>(
            this IHtmlHelper<T> html,
            string gridId,
            string crudController,
            int createPopupHeight = 430,
            int maxWidth = 0,
            bool reload = false,
            string area = null)
        {
            var url = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var format = "utils.{0}('" + gridId + "')";

            var createFunc = string.Format(format, reload ? "refreshGrid" : "itemCreated");
            var editFunc = string.Format(format, reload ? "refreshGrid" : "itemEdited");
            var delFunc = string.Format(format, reload ? "refreshGrid" : "itemDeleted");
            var delConfirmFunc = string.Format(format, "delConfirmLoad");

            return new HtmlString(
                html.Awe()
                    .InitPopupForm()
                    .Name("create" + gridId)
                    .Group(gridId)
                    .Height(createPopupHeight)
                    .MaxWidth(maxWidth)
                    .Url(url.Action("Create", crudController, new { area }))
                    .Title("Создание")
                    .Modal()
                    .Success(createFunc)
                    .ToString()
                +
                html.Awe()
                    .InitPopupForm()
                    .Name("edit" + gridId)
                    .Group(gridId)
                    .Height(createPopupHeight)
                    .MaxWidth(maxWidth)
                    .Url(url.Action("Edit", crudController, new { area }))
                    .Title("Редактирование")
                    .Modal()
                    .Success(editFunc)
                +
                html.Awe()
                    .InitPopupForm()
                    .Name("delete" + gridId)
                    .Group(gridId)
                    .Url(url.Action("Delete", crudController, new { area }))
                    .Title("Удаление")
                    .Success(delFunc)
                    .OnLoad(delConfirmFunc)
                    .Height(200)
                    .Modal()
             );
        }

        private static IUrlHelper GetUrlHelper<T>(IHtmlHelper<T> html)
        {
            return ((IUrlHelperFactory)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory))).GetUrlHelper(html.ViewContext);
        }
    }
}