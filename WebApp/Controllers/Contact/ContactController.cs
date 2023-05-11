using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GridLibrary;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.Contact;
using WebApp.Tools;
using System.Reflection;
using System;
using WebApp.Builders.Filters;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WebApp.Controllers.ContactControllers
{
    public class ContactController : Controller, IController<Contact, ContactAddInput, ContactEditInput, DeleteInput>
    {
        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly WebAppContext _context;

        /// <summary>
        /// Стандартный контсруктор.
        /// </summary>
        /// <param name="context">Контекст.</param>
        public ContactController(WebAppContext context)
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
        private object MapToGridModel(Contact contact)
        {
            return
                new
                {
                    contact.Id,
                    contact.Name,
                    contact.FullName,
                    contact.Email,
                    contact.Age,
                    BirthDate = contact.BirthDate.ToShortDateString(),
                    contact.Phone,
                    contact.MobilePhone,
                    ContactType = contact.ContactType.Name
                };
        }

        public async Task<ActionResult> GridGetItems(GridParams gridParams, string[] forder, string fullName, Guid? contactType)
        {
            forder ??= Array.Empty<string>();
            var items = _context.Contacts.Include(x => x.ContactType).AsQueryable();
            var frow = new Frow();

            var filterBuilder = new FilterBuilder<Contact>()
                .Add("FullName",
                    x => !string.IsNullOrWhiteSpace(fullName)
                        ? x.Where(y => y.FullName.Contains(fullName))
                        : x)
                .Add("ContactType", new FilterRule<Contact>
                {
                    Query = x => contactType.HasValue && contactType != Guid.Empty ? x.Where(y => y.ContactType.Id == contactType) : x,
                    Data = async x =>
                    {
                        frow.ContactType = await x.Select(x => x.ContactType)
                            .Distinct()
                            .Select(x => new KeyContent(x.Id, x.Name))
                            .ToArrayAsync();
                    }
                });

            items = await filterBuilder.ApplyAsync(items, forder);


            return Json(await new GridModelBuilder<Contact>(items, gridParams)
            {
                KeyProp = contact => contact.Id,
                //GetItem = () => items.Single(x => x.Id == Guid.Parse(gridParams.Key)),
                Map = MapToGridModel,
                Tag = new { frow }
            }.BuildAsync());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(ContactAddInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(view);
            }

            var contact = new Contact
            {
                Name = view.Name ?? string.Empty,
                FullName = view.FullName,
                Email = view.Email ?? string.Empty,
                Age = view.Age,
                BirthDate = view.BirthDate,
                Phone = view.Phone ?? string.Empty,
                MobilePhone = view.MobilePhone ?? string.Empty,
                ContactType = _context.ContactTypes.Single(x => x.Id == view.ContactType),
                CreatedOn = DateTime.Now.ToLocalTime(),
                ModifiedOn = DateTime.Now.ToLocalTime()
            };

            _context.Add(contact);
            _context.SaveChanges();

            return Json(MapToGridModel(contact));
        }

        [HttpGet]
        public ActionResult Delete(Guid id, string gridId)
        {
            var contact = _context.Contacts.Single(x => x.Id == id);

            return PartialView(new DeleteInput
            {
                Id = id,
                GridId = gridId,
                Message = string.Format("Are you sure you want to delete the contact <b>{0}</b> ?", contact.FullName)
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteInput view)
        {
            var contact = _context.Contacts.Single(x => x.Id == view.Id);

            _context.Remove(contact);
            _context.SaveChanges();

            return Json(new { view.Id });
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var contact = _context.Contacts.Include(x => x.ContactType).Single(x => x.Id == id);

            var input = new ContactEditInput
            {
                Id = contact.Id,
                Name = contact.Name,
                FullName = contact.FullName,
                Age = contact.Age,
                Email = contact.Email,
                BirthDate = contact.BirthDate,
                MobilePhone = contact.MobilePhone,
                Phone = contact.Phone
            };

            return PartialView("Edit", input);
        }

        [HttpPost]
        public ActionResult Edit(ContactEditInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Edit", view);
            }

            var contact = _context.Contacts.Single(x => x.Id == view.Id);

            contact.Name = view.Name ?? string.Empty;
            contact.FullName = view.FullName;
            contact.Email = view.Email ?? string.Empty;
            contact.Age = view.Age;
            contact.BirthDate = view.BirthDate.ToLocalTime();
            contact.Phone = view.Phone ?? string.Empty;
            contact.MobilePhone = view.MobilePhone ?? string.Empty;
            contact.ModifiedOn = DateTime.Now.ToLocalTime();

            _context.Update(contact);
            _context.SaveChanges();

            return Json(new { contact.Id });
        }

        [HttpGet]
        public ActionResult ExportToExcel()
        {
            var excelHelper = new ExcelHelper();
            var excel = excelHelper.ExportExcel(_context.Contacts.Include(x => x.ContactType).ToList());
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpPost]
        public IActionResult ImportExcel(IFormFile file)
        {
            var excelHelper = new ExcelHelper();
            var items = excelHelper.ImportExcel<Contact>(typeof(Contact), file.OpenReadStream());

            foreach (var item in items)
            {
                var contact = _context.Contacts.FirstOrDefault(x => x.Id == item.Id);

                if (contact == null)
                {
                    var newContact = new Contact()
                    {
                        Id = item.Id,
                        CreatedOn = DateTime.Now.ToLocalTime(),
                        ModifiedOn = DateTime.Now.ToLocalTime(),
                        MobilePhone = item.MobilePhone,
                        Phone = item.Phone,
                        FullName = item.FullName,
                        Age = item.Age,
                        BirthDate = item.BirthDate,
                        ContactType = _context.ContactTypes.Find(item?.ContactType?.Id),
                        Email = item.Email,
                        Gender = _context.Genders.Find(item?.Gender?.Id) ?? null,
                        Image = item.Image,
                        Name = item.Name
                    };

                    _context.Contacts.Add(item);
                }
                else
                {
                    contact.BirthDate = item.BirthDate;
                    contact.Name = item.Name;
                    contact.Email = item.Email;
                    contact.MobilePhone = item.MobilePhone;
                    contact.Gender = _context.Genders.Find(item?.Gender?.Id) ?? null;
                    contact.ContactType = _context.ContactTypes.Find(item?.ContactType?.Id);
                    contact.Age = item.Age;
                    contact.Phone = item.Phone;
                    contact.ModifiedOn = DateTime.Now;
                    _context.Contacts.Update(contact);
                }
            }

            _context.SaveChanges();
            return Json(items);
        }

        private class Frow
        {
            public KeyContent[] ContactType { get; set; }
        }
    }
}
