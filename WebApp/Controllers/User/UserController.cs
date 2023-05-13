using GridLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Context;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Models.User;
using WebApp.Tools;

namespace WebApp.Controllers.User
{
    public class UserController : Controller, IController<Entities.User, UserAddInput, UserEditInput, DeleteInput>
    {
        private readonly WebAppContext _context;

        public UserController(WebAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        private object MapToGridModel(Entities.User user)
        {
            return
                new
                {
                    user.Id,
                    user.UserName,
                    Contact = user.Contact.FullName,
                    Role = user.Role.Name,
                    IsBlocked = user.IsBlocked ? "Да" : "Нет"
                };
        }

        public async Task<ActionResult> GridGetItems(GridParams gridParams)
        {
            var items = _context.Users
                .Include(x => x.Role)
                .Include(x => x.Contact);

            return Json(await new GridModelBuilder<Entities.User>(items, gridParams)
            {
                KeyProp = user => user.Id,
                GetItem = () => items.Single(x => x.Id == Guid.Parse(gridParams.Key)),
                Map = MapToGridModel
            }.BuildAsync());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(UserAddInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(view);
            }

            var user = new Entities.User
            {
                UserName = view.UserName,
                Login = view.Login,
                Password = view.Password,
                Contact = _context.Contacts.Find(view.Contact),
                Role = _context.Roles.Find(view.Role),
                IsBlocked = view.IsBlocked,
                CreatedOn = DateTime.Now.ToLocalTime(),
                ModifiedOn = DateTime.Now.ToLocalTime()
            };

            _context.Add(user);
            _context.SaveChanges();

            return Json(MapToGridModel(user));
        }

        [HttpGet]
        public ActionResult Delete(Guid id, string gridId)
        {
            var user = _context.Users.Single(x => x.Id == id);

            return PartialView(new DeleteInput
            {
                Id = id,
                GridId = gridId,
                Message = string.Format(ConstansCS.LocalizationStrings.DeleteString, user.UserName)
            });
        }

        [HttpPost]
        public ActionResult Delete(DeleteInput view)
        {
            var user = _context.Users.Single(x => x.Id == view.Id);

            _context.Remove(user);
            _context.SaveChanges();

            return Json(new { view.Id });
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var user = _context.Users
                .Include(x => x.Role)
                .Include(x => x.Contact)
                .Single(x => x.Id == id);

            var input = new UserEditInput
            {
                Id = user.Id,
                UserName = user.UserName,
                Login = user.Login,
                Password = user.Password,
                ConfirmPassword = user.Password,
                Contact = user.Contact.Id,
                Role = user.Role.Id,
                IsBlocked = user.IsBlocked
            };

            return PartialView("Edit", input);
        }

        [HttpPost]
        public ActionResult Edit(UserEditInput view)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Edit", view);
            }

            var user = _context.Users.Single(x => x.Id == view.Id);

            user.UserName = view.UserName;
            user.Login = view.Login;
            user.Password = view.Password;
            user.Contact = _context.Contacts.Single(x => x.Id == view.Contact);
            user.Role = _context.Roles.Single(x => x.Id == view.Role);
            user.IsBlocked = view.IsBlocked;
            user.ModifiedOn = DateTime.Now.ToLocalTime();

            _context.Update(user);
            _context.SaveChanges();

            return Json(new { user.Id });
        }
    }
}
