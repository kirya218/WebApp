using WebApp.Context;
using WebApp.Controllers.Lookups.BaseMultiLookup;

namespace WebApp.Controllers.Lookups.DerivedMultiLookup
{
    public class PatientsMultiLookupController : ContactsMultiLookupController
    {
        public PatientsMultiLookupController(WebAppContext context) : base(context)
        {
        }
    }
}
