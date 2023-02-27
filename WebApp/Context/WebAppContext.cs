using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Context
{
    public class WebAppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SettingValue> SettingValues { get; set; }
        public DbSet<Chamber> Chambers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactInChamber> ContactInChambers { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public WebAppContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            //LoadData();
        }

        private void LoadData()
        {
            var contactType = new ContactType()
            {
                Name = "Работник",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var contactType2 = new ContactType()
            {
                Name = "Пациент",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            ContactTypes.Add(contactType);
            ContactTypes.Add(contactType2);

            var role = new Role()
            {
                Name = "Supervisor",
                Description = "Роль администратора",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var contact = new Contact()
            {
                Name = "Чернов К.К",
                FullName = "Чернов Кирилл Константинович",
                Email = "Kirill@mail.ru",
                Phone = "+79192222222",
                ContactType = contactType,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var contact2 = new Contact()
            {
                Name = "Шикина Е.А",
                FullName = "Шикина Евгения Александровна",
                Email = "Shik-310101@mail.ru",
                Phone = "+791921231242",
                ContactType = contactType2,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            Contacts.Add(contact);
            Contacts.Add(contact2);
            Roles.Add(role);

            Roles.Add(new Role()
            {
                Name = "User",
                Description = "Роль пользователя",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            Users.Add(new User()
            {
                UserName = "Kirya",
                Login = "kirill132",
                Contact = contact,
                IsBlocked = false,
                Password = "123456",
                Role = role,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            Users.Add(new User()
            {
                UserName = "Genya",
                Login = "genya124",
                Contact = contact2,
                IsBlocked = false,
                Password = "123456",
                Role = role,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            Settings.Add(new Setting()
            {
                Code = "QuntityRoomsOnFloor",
                Name = "Quntity rooms on floor",
                Type = "int",
                SettingValue = new SettingValue()
                {
                    IntegerValue = 9
                }
            });

            Settings.Add(new Setting()
            {
                Code = "QuntityFloors",
                Name = "Quntity floors",
                Type = "int",
                SettingValue = new SettingValue()
                {
                    IntegerValue = 4
                }
            });

            Settings.Add(new Setting()
            {
                Code = "MaxQuantitySeats",
                Name = "Max quntity seats on chamber",
                Type = "int",
                SettingValue = new SettingValue()
                {
                    IntegerValue = 5
                }
            });

            Schedules.Add(new Schedule()
            {
                Name = "Провести процедуру",
                Description = "Для того чтобы",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(2),
                Owner = contact,
                Patient = contact2,
                Color = "Red",
                IsAllDay= true,
                Procedure = new Procedure()
                {
                    Name = "Качаться",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                },
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            SaveChanges();
        }
    }
}