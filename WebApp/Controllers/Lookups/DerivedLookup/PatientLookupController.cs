using WebApp.Context;
using WebApp.Controllers.Lookups.BaseLookup;

namespace WebApp.Controllers.Lookups.DerivedLookup
{
    public class PatientLookupController : ContactLookupController
    {
        public PatientLookupController(WebAppContext context) : base(context)
        {
        }
    }
}
