using WebApp.Context;
using WebApp.Controllers.Lookups.BaseLookup;

namespace WebApp.Controllers.Lookups.DerivedLookup
{
    public class OwnerLookupController : ContactLookupController
    {
        public OwnerLookupController(WebAppContext context) : base(context)
        {
        }
    }
}
