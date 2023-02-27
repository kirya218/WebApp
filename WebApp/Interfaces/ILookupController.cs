using Microsoft.AspNetCore.Mvc;

namespace WebApp.Interfaces
{
    public interface ILookupController
    {
        /// <summary>
        /// Получить запись по Id.
        /// </summary>
        /// <param name="id">Идентификатор записи.</param>
        ActionResult GetItem(Guid id);

        /// <summary>
        /// Ищет все совпадаения по @value.
        /// </summary>
        /// <param name="value">Строка поиска.</param>
        /// <param name="page">Старница в lookup.</param>
        ActionResult Search(int page, string value = "");
    }
}
