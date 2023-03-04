using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GridLibrary;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.Chamber;
using WebApp.Tools;

namespace WebApp.Controllers.ChamberControllers
{
    public class ChamberController : Controller, IController<Chamber, ChamberAddImput, ChamberEditImput, DeleteInput>
    {
        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly WebAppContext _context;

        /// <summary>
        /// Стандартный контсруктор.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public ChamberController(WebAppContext context)
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
        /// <param name="chamber">Палата.</param>
        /// <returns>Результат.</returns>
        private object MapToGridModel(Chamber chamber)
        {
            var patientNames = _context.ContactInChambers
                    .Include(x => x.Chamber)
                    .Include(x => x.Contact)
                    .Where(x => x.Chamber != null && x.Contact != null && x.Chamber.Id == chamber.Id)
                    .Select(x => x.Contact.FullName)
                    .ToList();

            return
                new
                {
                    chamber.Id,
                    chamber.Number,
                    chamber.Floor,
                    chamber.QuantitySeats,
                    OwnerName = chamber.Owner.FullName,
                    PatientNames = string.Join(",", patientNames),
                    CreatedOn = chamber.CreatedOn.ToString("F"),
                    ModifiedOn = chamber.ModifiedOn.ToString("F")
                };
        }

        public ActionResult GridGetItems(GridParams gridParams)
        {
            var items = _context.Chambers.Include(x => x.Owner).AsQueryable();

            return Json(new GridModelBuilder<Chamber>(items, gridParams)
            {
                KeyProp = chamber => chamber.Id,
                GetItem = () => items.Single(x => x.Id == Guid.Parse(gridParams.Key)),
                Map = MapToGridModel
            }.Build());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ChamberAddImput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(view);
            }

            var chamber = new Chamber
            {
                Number = view.Number,
                Floor = view.Floor,
                QuantitySeats = view.QuantitySeats,
                Owner = _context.Contacts.Single(x => x.Id == view.Owner),
                CreatedOn = DateTime.Now.ToLocalTime(),
                ModifiedOn = DateTime.Now.ToLocalTime()
            };

            _context.Add(chamber);
            AddPatientsInChamber(chamber, view.Patients);
            _context.SaveChanges();

            return Json(MapToGridModel(chamber));
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var chamber = _context.Chambers.Include(x => x.Owner).Single(x => x.Id == id);
            var patients = _context.ContactInChambers
                .Include(x => x.Contact)
                .Include(x => x.Chamber)
                .Where(x => x.Contact != null && x.Chamber.Id == id)
                .ToList();

            var input = new ChamberEditImput
            {
                Id = chamber.Id,
                Number = chamber.Number,
                Floor = chamber.Floor,
                Owner = chamber.Owner.Id,
                Patients = patients.Select(x => x.Contact.Id),
                QuantitySeats = chamber.QuantitySeats
            };

            return PartialView("Edit", input);
        }

        [HttpPost]
        public ActionResult Edit(ChamberEditImput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Edit", view);
            }

            var chamber = _context.Chambers.First(x => x.Id == view.Id);

            chamber.Number = view.Number;
            chamber.Floor = view.Floor;
            chamber.Owner = _context.Contacts.Single(x => x.Id == view.Owner);
            chamber.QuantitySeats = view.QuantitySeats;
            chamber.ModifiedOn = DateTime.Now.ToLocalTime();

            RemovePatientsInChamber(chamber);
            AddPatientsInChamber(chamber, view.Patients);
            _context.Update(chamber);
            _context.SaveChanges();

            return Json(new { chamber.Id });
        }

        [HttpGet]
        public ActionResult Delete(Guid id, string gridId)
        {
            var chamber = _context.Chambers.Single(x => x.Id == id);

            return PartialView(new DeleteInput
            {
                Id = id,
                GridId = gridId,
                Message = string.Format("Are you sure you want to delete the chamber number <b>{0}</b> ?", chamber.Number)
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteInput view)
        {
            var chamber = _context.Chambers.Single(x => x.Id == view.Id);

            RemovePatientsInChamber(chamber);
            _context.Remove(chamber);
            _context.SaveChanges();

            return Json(new { view.Id });
        }

        /// <summary>
        /// Добавляет пациентов.
        /// </summary>
        /// <param name="chamber">Палата.</param>
        /// <param name="patientsId">Иденфикатор пациентов.</param>
        private void AddPatientsInChamber(Chamber chamber, IEnumerable<Guid> patientsId)
        {
            if (patientsId == null || !patientsId.Any())
            {
                return;
            }

            var patients = new List<ContactInChamber>();

            foreach (var patientId in patientsId)
            {
                patients.Add(new ContactInChamber
                {
                    Chamber = chamber,
                    Contact = _context.Contacts.Single(x => x.Id == patientId)
                });
            }

            if (patients.Count > 0)
            {
                _context.AddRange(patients);
            }
        }

        /// <summary>
        /// Удаляет пациентов из базы.
        /// </summary>
        /// <param name="chamber">Палата.</param>
        /// <param name="patientsId">Индентификатор пациентов.</param>
        private void RemovePatientsInChamber(Chamber chamber)
        {
            var patients = _context.ContactInChambers
                .Include(x => x.Chamber)
                .Include(x => x.Contact)
                .Where(x => x.Contact != null && x.Chamber != null && x.Chamber.Id == chamber.Id);

            _context.RemoveRange(patients);
        }

        /// <summary>
        /// Возвращает список доступных мест в палате.
        /// </summary>
        /// <returns>Список доступных мест в палате.</returns>
        public ActionResult GetSeatsList()
        {
            if (!SettingHelper.TryGetValue<int>(_context, "MaxQuantitySeats", out var quantity))
            {
                return Json(false);
            }

            return Json(Enumerable.Range(1, quantity).Select(x => new KeyContent(x, x.ToString())));
        }

        /// <summary>
        /// Возврщает список допустимых этажей.
        /// </summary>
        /// <returns>Список доступных этажей.</returns>
        public ActionResult GetFloorsList()
        {
            if (!SettingHelper.TryGetValue<int>(_context, "QuntityFloors", out var quantity))
            {
                return Json(false);
            }

            return Json(Enumerable.Range(1, quantity).Select(x => new KeyContent(x, x.ToString())));
        }
    }
}
