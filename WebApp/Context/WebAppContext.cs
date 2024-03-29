﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<ChamberType> ChamberTypes { get; set; }
        public DbSet<ContactHobby> ContactHobbies { get; set; }
        public DbSet<SystemType> SystemTypes { get; set; }

        public WebAppContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            //LoadData();
        }

        private void LoadData()
        {
            Lookups.Add(new Lookup
            {
                LookupCode = "ContactType",
                Name = "Типы контакта",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                Description = "Для типизации контантов"
            });

            Lookups.Add(new Lookup
            {
                LookupCode = "Procedure",
                Name = "Процедуры",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                Description = "Можно добавить процедуры"
            });

            var contactType = new ContactType()
            {
                Id = new Guid("20adea7c-b9a3-4b1a-9968-a7895c3c6ef3"),
                Name = "Работник",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var contactType2 = new ContactType()
            {
                Id = new Guid("968be231-af10-4f4a-99aa-3799be8be508"),
                Name = "Проживающий",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            ContactTypes.AddRange(contactType, contactType2);

            var gender = new Gender()
            {
                Name = "Женский",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var gender2 = new Gender()
            {
                Name = "Муржской",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var gender3 = new Gender()
            {
                Name = "Не определенный",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            Genders.AddRange(gender, gender2, gender3);

            var chamberType = new ChamberType()
            {
                Name = "Обычный",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var chamberType2 = new ChamberType()
            {
                Name = "Для инвалидов",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var chamberType3 = new ChamberType()
            {
                Name = "С спец. техникой",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            ChamberTypes.AddRange(chamberType, chamberType2, chamberType3);

            var roleEmployee = new Role
            {
                Id = new Guid("432ff91d-40f0-4d4e-8971-ec0570faec65"),
                Name = "Работник",
                Description = "Роль работника",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var roleAdmin = new Role()
            {
                Id = new Guid("09f161d7-8ed6-4eb0-a11d-3338453052d4"),
                Name = "Администратор",
                Description = "Роль администратора",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var roleUser = new Role()
            {
                Id = new Guid("a9458d1d-ac61-4936-8c85-c72a76baeef8"),
                Name = "Пользователь",
                Description = "Роль для всех пользователей",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var contact = new Contact()
            {
                Name = "Чернов К.К.",
                FullName = "Чернов Кирилл Константинович",
                Email = "Kirill@mail.ru",
                Phone = "+79192222222",
                ContactType = contactType,
                Age = 22,
                BirthDate = DateTime.Parse("24.04.2001"),
                MobilePhone = "+43564432426",
                Gender = gender,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var contact2 = new Contact()
            {
                Name = "Шикина Е.А.",
                FullName = "Шикина Евгения Александровна",
                Email = "Shik-310101@mail.ru",
                Phone = "+791921231242",
                ContactType = contactType,
                Age = 22,
                BirthDate = DateTime.Parse("31.01.2001"),
                MobilePhone = "+43564432426",
                Gender = gender2,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            var contact3 = new Contact()
            {
                Name = "Зимаков C.К.",
                FullName = "Зимаков Степан Константинович",
                Email = "stepanZimakov@mail.ru",
                Phone = "+7919536272242",
                ContactType = contactType2,
                Age = 22,
                BirthDate = DateTime.Parse("07.03.2002"),
                MobilePhone = "+4357533572426",
                Gender = gender,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            Contacts.AddRange(contact, contact2, contact3);
            Roles.AddRange(roleAdmin, roleEmployee, roleUser);

            Users.Add(new User()
            {
                UserName = "Kirya",
                Login = "kirill132",
                Contact = contact,
                IsBlocked = false,
                Password = "123456",
                Role = roleAdmin,
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
                Role = roleEmployee,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            Users.Add(new User()
            {
                UserName = "Stepan",
                Login = "stepa123",
                Contact = contact3,
                IsBlocked = false,
                Password = "123456",
                Role = roleUser,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            Settings.Add(new Setting()
            {
                Code = "QuntityRoomsOnFloor",
                Name = "Количество палат на этаже",
                Type = "int",
                SettingValue = new SettingValue()
                {
                    IntegerValue = 9
                }
            });

            Settings.Add(new Setting()
            {
                Code = "QuntityFloors",
                Name = "Количество этажей",
                Type = "int",
                SettingValue = new SettingValue()
                {
                    IntegerValue = 4
                }
            });

            Settings.Add(new Setting()
            {
                Code = "MaxQuantitySeats",
                Name = "Максимальное количество мест",
                Type = "int",
                SettingValue = new SettingValue()
                {
                    IntegerValue = 5
                }
            });

            var systemType1 = new SystemType
            {
                Id = new Guid("112303f1-e11f-489a-bb14-7eab492934d1"),
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                Name = "Детдом"
            };
            
            SystemTypes.Add(systemType1);

            SystemTypes.Add(new SystemType
            {
                Id = new Guid("4a4a11bb-afc0-48e9-8e47-797d57189bb8"),
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                Name = "Дом престарелых"
            });

            Settings.Add(new Setting()
            {
                Code = "SystemType",
                Name = "Тип системы",
                Type = "guid",
                SettingValue = new SettingValue()
                {
                    GuidValue = systemType1.Id
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

            Chambers.Add(new Chamber
            {
                Number = 1,
                Floor = 1,
                QuantitySeats = 3,
                Gender = gender,
                Owner = contact,
                ChamberType = chamberType,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            Chambers.Add(new Chamber
            {
                Number = 2,
                Floor = 1,
                QuantitySeats = 2,
                Gender = gender2,
                Owner = contact2,
                ChamberType = chamberType2,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            Chambers.Add(new Chamber
            {
                Number = 3,
                Floor = 2,
                QuantitySeats = 2,
                Gender = gender3,
                Owner = contact2,
                ChamberType = chamberType3,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            });

            SaveChanges();
        }
    }
}