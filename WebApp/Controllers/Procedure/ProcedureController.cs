using Microsoft.AspNetCore.Mvc;
using GridLibrary;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.Procedure;

namespace WebApp.Controllers.ProcedureControllers
{
    public class ProcedureController : Controller, IController<Procedure, ProcedureAddInput, ProcedureEditInput, DeleteInput>
    {
        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly WebAppContext _context;

        /// <summary>
        /// Стандартный контсруктор.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public ProcedureController(WebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Мапит в объект и возвращает его в грид.
        /// </summary>
        /// <param name="procedure">Контакт.</param>
        /// <returns>Результат.</returns>
        private object MapToGridModel(Procedure procedure)
        {
            return new
            {
                procedure.Id,
                procedure.Name,
                procedure.Description
            };
        }

        public ActionResult GridGetItems(GridParams gridParams)
        {
            var items = _context.Procedures;

            return Json(new GridModelBuilder<Procedure>(items, gridParams)
            {
                KeyProp = contact => contact.Id,
                GetItem = () => items.Single(x => x.Id == Guid.Parse(gridParams.Key)),
                Map = MapToGridModel
            }.Build());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ProcedureAddInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(view);
            }

            var procedure = new Procedure
            {
                Name = view.Name,
                Description = view.Description,
                CreatedOn = DateTime.Now.ToLocalTime(),
                ModifiedOn = DateTime.Now.ToLocalTime()
            };

            _context.Add(procedure);
            _context.SaveChanges();

            return Json(MapToGridModel(procedure));
        }

        [HttpGet]
        public ActionResult Delete(Guid id, string gridId)
        {
            var procedure = _context.Procedures.Single(x => x.Id == id);

            return PartialView(new DeleteInput
            {
                Id = id,
                GridId = gridId,
                Message = string.Format("Вы действительно хотете удалить данную процедуру <b>{0}</b> ?", procedure.Name)
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteInput view)
        {
            var procedure = _context.Procedures.Single(x => x.Id == view.Id);

            _context.Remove(procedure);
            _context.SaveChanges();

            return Json(new { view.Id });
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var procedure = _context.Procedures.Single(x => x.Id == id);

            var input = new ProcedureEditInput
            {
                Id = procedure.Id,
                Name = procedure.Name,
                Description = procedure.Description ?? string.Empty
            };

            return PartialView("Edit", input);
        }

        [HttpPost]
        public ActionResult Edit(ProcedureEditInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Edit", view);
            }

            var procedure = _context.Procedures.Single(x => x.Id == view.Id);

            procedure.Name = view.Name ?? string.Empty;
            procedure.Description = view.Description;
            procedure.ModifiedOn = DateTime.Now.ToLocalTime();

            _context.Update(procedure);
            _context.SaveChanges();

            return Json(new { procedure.Id });
        }
    }
}
