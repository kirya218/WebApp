using GridLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Models.SystemSetting;
using WebApp.Tools;

namespace WebApp.Controllers.SystemSetting
{
    public class SystemSettingController : Controller
    {
        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly WebAppContext _context;

        /// <summary>
        /// Стандартный контсруктор.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public SystemSettingController(WebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Стандартная страница для грида.
        /// </summary>
        /// <returns>Результат.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Мапит в объект и возвращает его в грид.
        /// </summary>
        /// <param name="contact">Контакт.</param>
        /// <returns>Результат.</returns>
        private object MapToGridModel(Setting setting)
        {
            return new
            {
                setting.Id,
                setting.Name,
                setting.Code,
                Value = SettingHelper.GetValue(_context, setting.Code),
                setting.Description,
            };
        }

        public ActionResult GridGetItems(GridParams gridParams)
        {
            var items = _context.Settings.Include(x => x.SettingValue).AsQueryable();

            return Json(new GridModelBuilder<Setting>(items, gridParams)
            {
                KeyProp = contact => contact.Id,
                GetItem = () => items.Single(x => x.Id == Guid.Parse(gridParams.Key)),
                Map = MapToGridModel
            }.Build());
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var systemSetting = _context.Settings.Include(x => x.SettingValue).Single(x => x.Id == id);
            var value = SettingHelper.GetValue(_context, systemSetting.Code);

            return systemSetting.Type switch
            {
                "string" => PartialView("Edit", CreateInput(systemSetting, new SystemSettingEditInput() { StringValue = (string)value })),
                "bool" => PartialView("Edit", CreateInput(systemSetting, new SystemSettingEditInput() { BoolValue = (bool)value })),
                "guid" => PartialView("Edit", CreateInput(systemSetting, new SystemSettingEditInput() { GuidValue = (Guid)value })),
                "datetime" => PartialView("Edit", CreateInput(systemSetting, new SystemSettingEditInput() { DateTimeValue = (DateTime)value })),
                "int" => PartialView("Edit", CreateInput(systemSetting, new SystemSettingEditInput() { IntValue = (int)value })),
                "float" => PartialView("Edit", CreateInput(systemSetting, new SystemSettingEditInput() { FloatValue = (float)value })),
                _ => PartialView(),
            };
        }

        private SystemSettingEditInput CreateInput(Setting systemSetting, SystemSettingEditInput view)
        {
            view.Id = systemSetting.Id;
            view.Name = systemSetting.Name;
            view.Code = systemSetting.Code;
            view.Description = systemSetting.Description;
            return view;
        }

        [HttpPost]
        public ActionResult Edit(SystemSettingEditInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Edit", view);
            }

            var setting = _context.Settings.Include(x => x.SettingValue).Single(x => x.Id == view.Id);
            setting.Description = view.Description ?? string.Empty;

            if (view.IntValue != null)
            {
                setting.SettingValue.IntegerValue = view.IntValue;
            }
            else if (view.GuidValue != null)
            {
                setting.SettingValue.GuidValue = view.GuidValue;
            }
            else if (view.DateTimeValue != null)
            {
                setting.SettingValue.DateTimeValue = view.DateTimeValue;
            }
            else if (view.FloatValue != null)
            {
                setting.SettingValue.FloatValue = view.FloatValue;
            }
            else if (view.BoolValue != null)
            {
                setting.SettingValue.BooleanValue = view.BoolValue;
            }
            else
            {
                setting.SettingValue.TextValue = view.StringValue;
            }

            setting.ModifiedOn = DateTime.Now.ToLocalTime();

            _context.Update(setting);
            _context.SaveChanges();

            return Json(new { setting.Id });
        }
    }
}
