using Microsoft.AspNetCore.Mvc;

namespace WebApp.Interfaces
{
    public interface IMultiLookupController
    {
        /// <summary>
        /// Получить все записи по идентификаторам.
        /// </summary>
        /// <param name="v">Индетификаторы.</param>
        /// <returns>Все записи.</returns>
        public ActionResult GetItems(Guid[] v);

        /// <summary>
        /// Поиск постраничный.
        /// </summary>
        /// <param name="selected">Выделенные записи.</param>
        /// <param name="page">Текущая страница.</param>
        /// <param name="search">Поисковой запрос.</param>
        /// <returns>Результат.</returns>
        public ActionResult Search(Guid[] selected, int page, string search = "");

        /// <summary>
        /// Установка выделенных записей.
        /// </summary>
        /// <param name="selected">Выделенные записи.</param>
        /// <returns>Результат.</returns>
        public ActionResult Selected(Guid[] selected);
    }
}
