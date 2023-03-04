using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GridLibrary;
using System;
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


            switch (systemSetting.Type)
            {
                case "string":
                    return PartialView("Edit", CreateInput(systemSetting, (string)value));
                case "bool":
                    return PartialView("Edit", CreateInput(systemSetting, (bool)value));
                case "guid":
                    return PartialView("Edit", CreateInput(systemSetting, (Guid)value));
                case "datetime":
                    return PartialView("Edit", CreateInput(systemSetting, (DateTime)value));
                case "int":
                    return PartialView("EditInteger", CreateInput(systemSetting, (int)value));
                case "float":
                    return PartialView("Edit", CreateInput(systemSetting, (float)value));
                default: 
                    return PartialView();
            }
        }

        private static SystemSettingEditInput<T> CreateInput<T>(Setting systemSetting, T value)
        {
            return new SystemSettingEditInput<T>
            {
                Id = systemSetting.Id,
                Name = systemSetting.Name,
                Code = systemSetting.Code,
                Description = systemSetting.Description,
                Value = value
            };
        }

        [HttpPost]
        public ActionResult Edit(SystemSettingEditInput<int> view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("EditInteger", view);
            }

            var setting = _context.Settings.Include(x => x.SettingValue).Single(x => x.Id == view.Id);

            setting.Description = view.Description ?? string.Empty;
            setting.SettingValue.IntegerValue = view.Value;
            setting.ModifiedOn = DateTime.Now.ToLocalTime();


            _context.Update(setting);
            _context.SaveChanges();

            return Json(new { setting.Id });
        }
    }
}
