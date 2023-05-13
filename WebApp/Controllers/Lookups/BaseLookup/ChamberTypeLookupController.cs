using Microsoft.AspNetCore.Mvc;
using GridLibrary;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;

namespace WebApp.Controllers.Lookups.BaseLookup
{
    public class ChamberTypeLookupController : Controller, ILookupController
    {
        protected readonly WebAppContext _context;
        protected const int PageSize = 7;

        public ChamberTypeLookupController(WebAppContext context)
        {
            _context = context;
        }

        public virtual ActionResult GetItem(Guid v)
        {
            var contactType = _context.ChamberTypes.Single(x => x.Id == v) ?? new ChamberType();
            return Json(new KeyContent(contactType.Id, contactType.Name));
        }

        public virtual ActionResult Search(int page, string value = "")
        {
            var contacts = _context.ChamberTypes.Where(x => x.Name.ToLower().Contains(value.ToLower().Trim()));

            return Json(new AjaxListResult
            {
                Items = contacts.Skip((page - 1) * PageSize).Take(PageSize).Select(x => new KeyContent(x.Id, x.Name)),
                More = contacts.Count() > page * PageSize
            });
        }
    }
}
