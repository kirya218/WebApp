using GridLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Context;
using WebApp.Controllers.Lookups.BaseLookup;
using WebApp.Tools;

namespace WebApp.Controllers.Lookups.DerivedLookup
{
    public class PatientLookupController : ContactLookupController
    {
        public PatientLookupController(WebAppContext context) : base(context)
        {
        }

        public override ActionResult Search(int page, string value = "")
        {
            var contacts = _context.Contacts
                .Include(x => x.ContactType)
                .Where(x => x.ContactType.Id == ConstansCS.ContactType.Resident)
                .Where(x => x.FullName.ToLower().Contains(value.ToLower().Trim()))
                .ToList();

            return Json(new AjaxListResult
            {
                Items = contacts.Skip((page - 1) * PageSize).Take(PageSize).Select(x => new KeyContent(x.Id, x.FullName)),
                More = contacts.Count > page * PageSize
            });
        }
    }
}
