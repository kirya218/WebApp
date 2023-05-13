namespace WebApp.Tools
{
    public static class ConstansCS
    {
        public static class LocalizationStrings
        {
            public static readonly string DeleteString = "Вы действительно хотите удалить данную запись <b>{0}</b> ?";
            public static readonly string StartDateError = "Дата и время окончания должны быть больше даты и времени начала";
        }

        public static class Roles
        {
            public static readonly Guid Supervisor = new Guid("09f161d7-8ed6-4eb0-a11d-3338453052d4");
            public static readonly Guid User = new Guid("a9458d1d-ac61-4936-8c85-c72a76baeef8");
            public static readonly Guid Employee = new Guid("432ff91d-40f0-4d4e-8971-ec0570faec65");
        }

        public static class Type
        {
            public static readonly Guid Orphanage = new Guid("112303f1-e11f-489a-bb14-7eab492934d1");
            public static readonly Guid NursingHome = new Guid("4a4a11bb-afc0-48e9-8e47-797d57189bb8");
        }

        public static class ContactType
        {
            public static readonly Guid Employee = new Guid("20adea7c-b9a3-4b1a-9968-a7895c3c6ef3");
            public static readonly Guid Resident = new Guid("968be231-af10-4f4a-99aa-3799be8be508");
        }
    }
}
