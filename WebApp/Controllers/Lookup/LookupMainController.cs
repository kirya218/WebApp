using GridLibrary;
using Microsoft.AspNetCore.Mvc;
using WebApp.Context;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.Lookup;

namespace WebApp.Controllers.Lookup
{
    public class LookupMainController : Controller, IController<Entities.Lookup, object, LookupMainEditInput, DeleteInput>
    {
        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly WebAppContext _context;

        /// <summary>
        /// Стандартный контсруктор.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public LookupMainController(WebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Стандартная страница для грида.
        /// </summary>
        /// <returns>Результат.</returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridGetItems(GridParams gridParams)
        {
            var items = _context.Lookups;

            return Json(new GridModelBuilder<Entities.Lookup>(items.AsQueryable(), gridParams)
            {
                KeyProp = lookup => lookup.Id,
                GetItem = () => items.Single(x => x.Id == Guid.Parse(gridParams.Key)),
            }.Build());
        }

        [HttpGet]
        public ActionResult Create()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Create(object view)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var lookup = _context.Lookups.Single(x => x.Id == id);
            var gridName = lookup.LookupCode + "Lookup";

            return PartialView("Edit", new LookupMainEditInput
            {
                GridName = gridName,
                LookupName = lookup.Name,
                LookupCode = lookup.LookupCode,
                ControllerName = "LookupEdit",
                Columns = new Column[]
                {
                    new Column { Bind = "Id", Header = "Id",  Hidden = true },
                    new Column { Bind = "Name", Header = "Название" },
                    new Column { Bind = "Description", Header = "Описание" }
                }
            });
        }

        [HttpPost]
        public ActionResult Edit(LookupMainEditInput view)
        {
            return Json(new { });
        }

        [HttpGet]
        public ActionResult Delete(Guid id, string gridId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Delete(DeleteInput view)
        {
            throw new NotImplementedException();
        }
    }
}
