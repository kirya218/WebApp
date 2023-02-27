using Microsoft.AspNetCore.Mvc;
using Omu.AwesomeMvc;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;

namespace WebApp.Controllers.Lookups.BaseLookup
{
    public class ContactLookupController : Controller, ILookupController
    {
        private readonly WebAppContext _context;
        private const int PageSize = 7;

        public ContactLookupController(WebAppContext context)
        {
            _context = context;
        }

        public virtual ActionResult GetItem(Guid v)
        {
            var owner = _context.Contacts.FirstOrDefault(x => x.Id == v) ?? new Contact();
            return Json(new KeyContent(owner.Id, owner.FullName));
        }

        public virtual ActionResult Search(int page, string value = "")
        {
            var contacts = _context.Contacts.Where(x => x.FullName.ToLower().Contains(value.ToLower().Trim()));

            return Json(new AjaxListResult
            {
                Items = contacts.Skip((page - 1) * PageSize).Take(PageSize).Select(x => new KeyContent(x.Id, x.FullName)),
                More = contacts.Count() > page * PageSize
            });
        }
    }
}