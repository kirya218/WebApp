using Microsoft.AspNetCore.Mvc;
using GridLibrary;

namespace WebApp.Interfaces
{
    public interface IController<T, TAddImput, TEditImput, TDeleteImput>
    {
        /// <summary>
        /// Метод для создания записи. Пустая заглушка.
        /// </summary>
        /// <returns>Результат.</returns>
        [HttpGet]
        public ActionResult Create();

        /// <summary>
        /// Метод для создания записи, уже с заполнеными данными. Добавление в базу.
        /// </summary>
        /// <param name="view">Вьюшка с заполнеными данными.</param>
        /// <returns>Результат.</returns>
        [HttpPost]
        public ActionResult Create(TAddImput view);

        /// <summary>
        /// Возвращает запись с редактируемыми полями.
        /// </summary>
        /// <param name="id">Идентификато записи.</param>
        /// <returns>Результат.</returns>
        [HttpGet]
        public ActionResult Edit(Guid id);

        /// <summary>
        /// Изменят данные в базе. Данные приходят с вьюшки.
        /// </summary>
        /// <param name="view">Вьюшка с заполнеными данными.</param>
        /// <returns>Результат.</returns>
        [HttpPost]
        public ActionResult Edit(TEditImput view);

        /// <summary>
        /// Попап окно для предупреждения удаления.
        /// </summary>
        /// <param name="id">Иденитфиктор записи.</param>
        /// <param name="gridId">Идентификатор грида.</param>
        /// <returns>Результат.</returns>
        [HttpGet]
        public ActionResult Delete(Guid id, string gridId);

        /// <summary>
        /// Удаляет запись из базы.
        /// </summary>
        /// <param name="input">Вьшка для удаления.</param>
        /// <returns>Результат.</returns>
        [HttpPost]
        public ActionResult Delete(TDeleteImput view);
    }
}
