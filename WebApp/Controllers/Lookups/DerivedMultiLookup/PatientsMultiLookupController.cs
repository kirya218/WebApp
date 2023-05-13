using GridLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Context;
using WebApp.Controllers.Lookups.BaseMultiLookup;
using WebApp.Tools;

namespace WebApp.Controllers.Lookups.DerivedMultiLookup
{
    public class PatientsMultiLookupController : ContactsMultiLookupController
    {
        public PatientsMultiLookupController(WebAppContext context) : base(context)
        {
        }
        public override ActionResult Search(Guid[] selected, int page, string search)
        {
            selected ??= Array.Empty<Guid>();

            var items = _context.Contacts
                .Include(x => x.ContactType)
                .Where(x => x.ContactType.Id == ConstansCS.ContactType.Resident)
                .Where(x => !selected.Contains(x.Id));

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = items.Where(x => x.FullName.ToLower().Contains(search.ToLower().Trim()));
            }

            return Json(new AjaxListResult
            {
                Items = items
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .Select(x => new KeyContent(x.Id, x.FullName)),
                More = items.Count() > page * PageSize
            });
        }
    }
}
