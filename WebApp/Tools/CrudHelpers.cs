﻿using GridLibrary;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

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

            var refreshFunc = string.Format(format, "refreshGrid");
            var createFunc = string.Format(format, "itemCreated");
            var editFunc = string.Format(format, "itemEdited");
            var delFunc = string.Format(format, "itemDeleted");
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
                    .Success(refreshFunc)
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
                    .Success(refreshFunc)
                +
                html.Awe()
                    .InitPopupForm()
                    .Name("delete" + gridId)
                    .Group(gridId)
                    .Url(url.Action("Delete", crudController, new { area }))
                    .Title("Удаление")
                    .Success(delFunc)
                    .Success(refreshFunc)
                    .OnLoad(delConfirmFunc)
                    .Height(200)
                    .Modal()
             );
        }

        /// <summary>
        /// Инициализирует окно выбора фильтров.
        /// </summary>
        /// <typeparam name="T">Тип html.</typeparam>
        /// <param name="html">HTML.</param>
        /// <param name="gridId">Идентификатор таблицы.</param>
        /// <param name="crudController">Названия контролера.</param>
        /// <param name="createPopupHeight">Высота окна.</param>
        /// <param name="maxWidth">Максимальная ширина окна.</param>
        /// <returns>Контент HTML.</returns>
        public static IHtmlContent InitChoiseChamber<T>(
            this IHtmlHelper<T> html,
            string gridId,
            string crudController,
            int createPopupHeight = 430,
            int maxWidth = 284,
            string area = null)
        {
            var url = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var reloadFunc = "utils.refreshGrid('" + gridId + "')";

            return new HtmlString(
                html.Awe()
                    .InitPopupForm()
                    .Name("choice" + gridId)
                    .Group(gridId)
                    .Height(createPopupHeight)
                    .MaxWidth(maxWidth)
                    .Url(url.Action("Choice", crudController, new { area }))
                    .Title("Подбор палаты")
                    .Modal()
                    .Success(reloadFunc)
                    .ToString()
             );
        }

        /// <summary>
        /// Инициализирует окно выбора фильтров.
        /// </summary>
        /// <typeparam name="T">Тип html.</typeparam>
        /// <param name="html">HTML.</param>
        /// <param name="gridId">Идентификатор таблицы.</param>
        /// <param name="crudController">Названия контролера.</param>
        /// <param name="createPopupHeight">Высота окна.</param>
        /// <param name="maxWidth">Максимальная ширина окна.</param>
        /// <returns>Контент HTML.</returns>
        public static IHtmlContent InitImportFile<T>(
            this IHtmlHelper<T> html,
            string gridId,
            string crudController,
            int createPopupHeight = 200,
            int maxWidth = 0,
            string area = null)
        {
            var url = GetUrlHelper(html);
            gridId = html.Awe().GetContextPrefix() + gridId;

            var reloadFunc = "utils.refreshGrid('" + gridId + "')";

            return new HtmlString(
                html.Awe()
                    .InitPopupForm()
                    .Name("importexcel" + gridId)
                    .Group(gridId)
                    .Height(createPopupHeight)
                    .MaxWidth(maxWidth)
                    .Url(url.Action("ImportExcel", crudController, new { area }))
                    .Title("Выбор файла")
                    .Modal()
                    .Success(reloadFunc)
                    .ToString()
             );
        }

        private static IUrlHelper GetUrlHelper<T>(IHtmlHelper<T> html)
        {
            return ((IUrlHelperFactory)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory))).GetUrlHelper(html.ViewContext);
        }
    }
}