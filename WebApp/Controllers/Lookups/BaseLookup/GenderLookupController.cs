using GridLibrary;
using Microsoft.AspNetCore.Mvc;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;

namespace WebApp.Controllers.Lookups.BaseLookup
{
    public class GenderLookupController : Controller, ILookupController
    {
        private readonly WebAppContext _context;
        private const int PageSize = 7;

        public GenderLookupController(WebAppContext context)
        {
            _context = context;
        }

        public virtual ActionResult GetItem(Guid v)
        {
            var contactType = _context.Genders.Single(x => x.Id == v) ?? new Gender();
            return Json(new KeyContent(contactType.Id, contactType.Name));
        }

        public virtual ActionResult Search(int page, string value = "")
        {
            var contacts = _context.Genders.Where(x => x.Name.ToLower().Contains(value.ToLower().Trim()));

            return Json(new AjaxListResult
            {
                Items = contacts.Skip((page - 1) * PageSize).Take(PageSize).Select(x => new KeyContent(x.Id, x.Name)),
                More = contacts.Count() > page * PageSize
            });
        }
    }
}
