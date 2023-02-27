using Microsoft.AspNetCore.Mvc;
using Omu.AwesomeMvc;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;

namespace WebApp.Controllers.Lookups.BaseLookup
{
    public class ContactTypeLookupController : Controller, ILookupController
    {
        private readonly WebAppContext _context;
        private const int PageSize = 7;

        public ContactTypeLookupController(WebAppContext context)
        {
            _context = context;
        }

        public virtual ActionResult GetItem(Guid v)
        {
            var contactType = _context.ContactTypes.Single(x => x.Id == v) ?? new ContactType();
            return Json(new KeyContent(contactType.Id, contactType.Name));
        }

        public virtual ActionResult Search(int page, string value = "")
        {
            var contacts = _context.ContactTypes.Where(x => x.Name.ToLower().Contains(value.ToLower().Trim()));

            return Json(new AjaxListResult
            {
                Items = contacts.Skip((page - 1) * PageSize).Take(PageSize).Select(x => new KeyContent(x.Id, x.Name)),
                More = contacts.Count() > page * PageSize
            });
        }
    }
}
