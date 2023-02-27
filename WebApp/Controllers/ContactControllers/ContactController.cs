﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Omu.AwesomeMvc;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.Contact;

namespace WebApp.Controllers.ContactControllers
{
    public class ContactController : Controller, IController<Contact, ContactAddInput, ContactEditInput, DeleteImput>
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
                    BirthDate = contact.BirthDate.ToString("F"),
                    contact.Phone,
                    contact.MobilePhone,
                    ContactType = contact.ContactType.Name
                };
        }

        public ActionResult GridGetItems(GridParams gridParams)
        {
            var items = _context.Contacts.Include(x => x.ContactType).AsQueryable();

            return Json(new GridModelBuilder<Contact>(items, gridParams)
            {
                KeyProp = contact => contact.Id,
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

            return PartialView(new DeleteImput
            {
                Id = id,
                GridId = gridId,
                Message = string.Format("Are you sure you want to delete the contact <b>{0}</b> ?", contact.FullName)
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteImput view)
        {
            var chamber = _context.Chambers.Single(x => x.Id == view.Id);

            _context.Remove(chamber);
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
                BirthDate= contact.BirthDate,
                MobilePhone = contact.MobilePhone,
                Phone= contact.Phone
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
    }
}