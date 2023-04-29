using GridLibrary;
using Microsoft.AspNetCore.Mvc;
using WebApp.Context;
using WebApp.Interfaces;

namespace WebApp.Controllers.Lookups.BaseMultiLookup
{
    public class HobbiesMultiLookupController : Controller, IMultiLookupController
    {
        private const int PageSize = 7;
        private readonly WebAppContext _context;

        public HobbiesMultiLookupController(WebAppContext context)
        {
            _context = context;
        }

        public virtual ActionResult GetItems(Guid[] v)
        {
            var items = _context.Hobbies
                .Where(x => v != null && v.Contains(x.Id))
                .Select(x => new KeyContent(x.Id, x.Name));

            return Json(items);
        }

        public virtual ActionResult Search(Guid[] selected, int page, string search)
        {
            selected ??= Array.Empty<Guid>();

            var items = _context.Hobbies.Where(x => !selected.Contains(x.Id));

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = items.Where(x => x.Name.ToLower().Contains(search.ToLower().Trim()));
            }

            return Json(new AjaxListResult
            {
                Items = items
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .Select(x => new KeyContent(x.Id, x.Name)),
                More = items.Count() > page * PageSize
            });
        }

        public virtual ActionResult Selected(Guid[] selected)
        {
            return Json(new AjaxListResult
            {
                Items = _context.Hobbies
                    .Where(x => selected != null && selected.Contains(x.Id))
                    .Select(x => new KeyContent(x.Id, x.Name))
            });
        }
    }
}
