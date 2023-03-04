using Microsoft.AspNetCore.Mvc;
using WebApp.Context;
using WebApp.Tools;

namespace WebApp.Controllers.ChamberControllers
{
    public class ChamberValidateController : Controller
    {
        private readonly WebAppContext _context;

        public ChamberValidateController(WebAppContext context)
        {
           _context = context;
        }

        public IActionResult VerifyNumber(int number, int floor)
        {
            if (HasProblemQuntityRooms(number))
            {
                return Json(false);
            }

            if (_context.Chambers.Any(c => c.Number == number && c.Floor == floor))
            {
                return Json(false);
            }

            return Json(true);
        }

        public IActionResult VerifyNumberEdit(int number, int floor)
        {
            if (HasProblemQuntityRooms(number))
            {
                return Json(false);
            }

            if (_context.Chambers.Count(c => c.Number == number && c.Floor == floor) > 1)
            {
                return Json(false);
            }

            return Json(true);
        }

        private bool HasProblemQuntityRooms(int number)
        {
            return !SettingHelper.TryGetValue<int>(_context, "QuntityRoomsOnFloor", out var quantityRoomsOnFloor) || quantityRoomsOnFloor < number || number <= 0;
        }
    }
}
