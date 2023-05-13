using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GridLibrary;
using WebApp.Builders;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Enums.Schedule;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.Schedule;
using WebApp.Tools;

namespace WebApp.Controllers.ScheludeControllers
{
    public class ScheduleController : Controller, IController<Schedule, ScheduleAddInput, ScheduleEditInput, DeleteInput>
    {
        private readonly WebAppContext _context;

        public ScheduleController(WebAppContext context)
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
        /// Возвращает записи для грида.
        /// </summary>
        /// <param name="gridParams">Параметры грида.</param>
        /// <param name="date">Выбранная дата.</param>
        /// <param name="viewType">Тип вьюшки.</param>
        /// <param name="hourStep">Режим степа.</param>
        /// <returns>Записи.</returns>
        public ActionResult GridGetItems(GridParams gridParams, DateTime? date, ScheduleViewType? viewType, int hourStep)
        {
            var userId = Guid.Parse(User.Claims.First(x => x.Type == "UserId").Value);
            var roleId = Guid.Parse(User.Claims.First(x => x.Type == "RoleId").Value);

            var items = _context.Schedules
                .Include(x => x.Owner)
                .Include(x => x.Procedure)
                .Include(x => x.Patient)
                .AsQueryable();

            var contactId = GridUtils.GetContactId(userId, _context);

            if (roleId == ConstansCS.Roles.User)
            {
                items = items
                    .Where(x => x.Patient != null && x.Patient.Id == contactId);
            }

            if (roleId == ConstansCS.Roles.Employee)
            {
                items = items
                    .Where(x => x.Owner != null && x.Owner.Id == contactId);
            }

            var model = new ScheduleBuilder(new ScheduleBuilderModel
            {
                GridParams = gridParams,
                DateSelected = date,
                HourStep = hourStep,
                ViewType = viewType ?? ScheduleViewType.Week,
                Schedules = items
            }).Build();

            return Json(model);
        }

        public IActionResult GetViewTypes()
        {
            return Json(ScheduleBuilder.GetViewTypes());
        }

        public IActionResult GetHourSteps()
        {
            return Json(ScheduleBuilder.GetHourSteps());
        }

        [HttpGet]
        public ActionResult Create()
        {
            var start = DateTime.Now.ToLocalTime();
            var end = start.AddMinutes(15);

            var input = new ScheduleAddInput
            {
                StartDate = start,
                StartTime = start,
                IsAllDay = false,
                EndDate = end,
                EndTime = end,
                Color = "#5484ED"
            };

            return PartialView(input);
        }

        [HttpPost]
        public ActionResult Create(ScheduleAddInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(view);
            }

            var start = view.StartDate.Value.Date;
            var end = view.StartDate.Value.Date;

            if (!view.IsAllDay)
            {
                start += view.StartTime.TimeOfDay;
                end += view.EndTime.TimeOfDay;
            }

            if (!view.IsAllDay && end <= start || end < start)
            {
                ModelState.AddModelError("StartDate", ConstansCS.LocalizationStrings.StartDateError);
                return PartialView(view);
            }

            var schedule = new Schedule
            {
                Name = view.Title,
                Description= view.Description,
                StartDate = start,
                EndDate = end,
                IsAllDay = view.IsAllDay,
                Color = view.Color,
                Owner = _context.Contacts.Single(x => x.Id == view.Owner),
                Patient = _context.Contacts.Single(x => x.Id == view.Patient),
                Procedure = _context.Procedures.Single(x => x.Id == view.Procedure),
                CreatedOn = DateTime.Now.ToLocalTime(),
                ModifiedOn = DateTime.Now.ToLocalTime()
            };

            _context.Add(schedule);
            _context.SaveChanges();

            return Json(new { });
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var schelude = _context.Schedules
                .Include(x => x.Owner)
                .Include(x => x.Patient)
                .Include(x => x.Procedure)
                .Single(x => x.Id == id);

            var input = new ScheduleEditInput
            {
                Title = schelude.Name,
                Description = schelude.Description,
                StartDate = schelude.StartDate,
                EndDate = schelude.EndDate,
                StartTime = schelude.StartDate,
                EndTime = schelude.EndDate,
                IsAllDay = schelude.IsAllDay,
                Color = schelude.Color,
                Owner = schelude.Owner.Id,
                Patient = schelude.Patient.Id,
                Procedure = schelude.Procedure.Id,
            };

            return PartialView("Edit", input);
        }

        [HttpPost]
        public ActionResult Edit(ScheduleEditInput view)
        {
            var roleId = User.Claims.First(x => x.Type == "RoleId")?.Value;

            if (string.IsNullOrWhiteSpace(roleId) || Guid.Parse(roleId) == ConstansCS.Roles.User)
            {
                return Json(new { });
            }

            if (!ModelState.IsValid)
            {
                return PartialView("Edit", view);
            }

            var start = view.StartDate.Value.Date;
            var end = view.EndDate.Value.Date;

            if (!view.IsAllDay)
            {
                start += view.StartTime.TimeOfDay;
                end += view.EndTime.TimeOfDay;
            }

            if (!view.IsAllDay && end <= start || end < start)
            {
                ModelState.AddModelError("StartDate", ConstansCS.LocalizationStrings.StartDateError);
                return PartialView("Edit", view);
            }

            var schedule = _context.Schedules.Single(x => x.Id == view.Id);

            schedule.Name = view.Title;
            schedule.Description = view.Description;
            schedule.StartDate = start;
            schedule.EndDate = end;
            schedule.IsAllDay = view.IsAllDay;
            schedule.Color = view.Color;
            schedule.Owner = _context.Contacts.Single(x => x.Id == view.Owner);
            schedule.Patient = _context.Contacts.Single(x => x.Id == view.Patient);
            schedule.Procedure = _context.Procedures.Single(x => x.Id == view.Procedure);
            schedule.ModifiedOn = DateTime.Now.ToLocalTime();

            _context.Update(schedule);
            _context.SaveChanges();

            return Json(new { });
        }

        [HttpGet]
        public ActionResult Delete(Guid id, string gridId)
        {
            var schelude = _context.Schedules.Single(x => x.Id == id);

            return PartialView(new DeleteInput
            {
                Id = id,
                GridId = gridId,
                Message = string.Format(ConstansCS.LocalizationStrings.DeleteString, schelude.Name)
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteInput view)
        {
            var schelude = _context.Schedules.Single(x => x.Id == view.Id);

            _context.Remove(schelude);
            _context.SaveChanges();

            return Json(new { view.Id });
        }
    }
}
