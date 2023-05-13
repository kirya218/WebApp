using Microsoft.AspNetCore.Mvc;
using GridLibrary;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.LookupEdit;
using WebApp.Models.Lookup;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using WebApp.Tools;

namespace WebApp.Controllers.ProcedureControllers
{
    public class LookupEditController : Controller
    {
        private static string _lookupCode;

        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly WebAppContext _context;

        /// <summary>
        /// Стандартный контсруктор.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public LookupEditController(WebAppContext context)
        {
            _context = context;
        }

        public IActionResult GridGetItems(GridParams gridParams, LookupMainEditInput model)
        {
            _lookupCode = model.LookupCode;
            var items = (IQueryable<object>)_context.GetType()?.GetProperty($"{model.LookupCode}s")?.GetValue(_context);
            return Json(new GridModelBuilder<object>(items, gridParams).Build());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult Create(LookupEditAddInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(view);
            }

            var obj = Activator.CreateInstance(Type.GetType($"WebApp.Entities.{_lookupCode}"));

            obj.GetType()?.GetProperty("Name")?.SetValue(obj, view.Name);
            obj.GetType()?.GetProperty("Description")?.SetValue(obj, view.Description);
            obj.GetType()?.GetProperty("CreatedOn")?.SetValue(obj, DateTime.Now.ToLocalTime());
            obj.GetType()?.GetProperty("ModifiedOn")?.SetValue(obj, DateTime.Now.ToLocalTime());

            if (obj != null)
            {
                _context.Add(obj);
                _context.SaveChanges();
            }

            return Json(obj);
        }

        [HttpGet]
        public IActionResult Delete(Guid id, string gridId)
        {
            var obj = ((IEnumerable<object>)_context.GetType()?.GetProperty($"{_lookupCode}s")?.GetValue(_context))
                .Single(x => (Guid)x.GetType().GetProperty("Id").GetValue(x) == id);

            return PartialView(new DeleteInput
            {
                Id = id,
                GridId = gridId,
                Message = string.Format(ConstansCS.LocalizationStrings.DeleteString, obj.GetType().GetProperty("Name").GetValue(obj))
            });
        }

        [HttpPost]
        public IActionResult Delete(DeleteInput view)
        {
            var obj = ((IEnumerable<object>)(_context.GetType()?.GetProperty($"{_lookupCode}s")?.GetValue(_context)))
                 .Single(x => (Guid)x.GetType().GetProperty("Id").GetValue(x) == view.Id);

            _context.Remove(obj);
            _context.SaveChanges();

            return Json(new { view.Id });
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var obj = ((IEnumerable<object>)_context.GetType()?.GetProperty($"{_lookupCode}s")?.GetValue(_context))
                .Single(x => (Guid)x.GetType().GetProperty("Id").GetValue(x) == id);

            var input = new LookupEditEditInput
            {
                Id = (Guid)obj.GetType().GetProperty("Id").GetValue(obj),
                Name = (string)obj.GetType().GetProperty("Name").GetValue(obj),
                Description = (string)obj.GetType().GetProperty("Description").GetValue(obj) ?? string.Empty
            };

            return PartialView("Edit", input);
        }

        [HttpPost]
        public IActionResult Edit(LookupEditEditInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Edit", view);
            }

            var obj = ((IEnumerable<object>)(_context.GetType()?.GetProperty($"{_lookupCode}s")?.GetValue(_context)))
                 .Single(x => (Guid)x.GetType().GetProperty("Id").GetValue(x) == view.Id);

            obj.GetType()?.GetProperty("Name")?.SetValue(obj, view.Name);
            obj.GetType()?.GetProperty("Description")?.SetValue(obj, view.Description);
            obj.GetType()?.GetProperty("ModifiedOn")?.SetValue(obj, DateTime.Now.ToLocalTime());

            _context.Update(obj);
            _context.SaveChanges();

            return Json(new { Id = (Guid)obj.GetType().GetProperty("Id").GetValue(obj) });
        }
    }
}
