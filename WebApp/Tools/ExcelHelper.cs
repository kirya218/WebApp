using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Norbit.Srm.SeverMinerals.GenerateExcelFromXml;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebApp.Tools
{
    public class ExcelHelper
    {
        private readonly Dictionary<string, string> _systemProperties = new()
        {
            { "Id", "Id" },
            { "CreatedOn", "Дата создания" },
            { "ModifiedOn", "Дата изменения" }
        };

        /// <summary>
        /// Экпорт в excel.
        /// </summary>
        /// <typeparam name="T">Class.</typeparam>
        /// <param name="entities">Сущности.</param>
        public byte[] ExportExcel<T>(List<T> entities) where T : class
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentException("Пустой параметр", nameof(entities));
            }

            var stream = new MemoryStream();

            using (var package = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                //var stylesPart = new ExcelWorkbookStylePart();
                var name = entities.First().GetType().Name;

                var excel = new Excel(package);
                excel.CreateExcel($"{name} {DateTime.Now.ToShortDateString()}");
                excel.CreateWorkbook();
                //_excel.CreateStyles(stylesPart);
                excel.CreateSheet(name);

                var properties = GetNameProperties(entities.First());

                for (var i = 0; i < properties.Count; i++)
                {
                    excel.Append(1U, (uint)i + 1, properties.ElementAt(i).Value, CellValues.SharedString);
                }

                for (var i = 0; i < entities.Count; i++)
                {
                    for (var j = 0; j < properties.Count; j++)
                    {
                        var property = entities[i].GetType().GetProperty(properties.ElementAt(j).Key);
                        var propertyValue = property?.GetValue(entities[i]);

                        if (propertyValue != null && propertyValue.GetType().IsClass)
                        {
                            propertyValue = propertyValue.GetType().GetProperty("Id")?.GetValue(propertyValue) ?? propertyValue;
                        }

                        excel.Append((uint)i + 2, (uint)j + 1, propertyValue?.ToString() ?? string.Empty, CellValues.SharedString);
                    }
                }
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Возвращает список полученных из excel файла значений и маппит их в T.
        /// </summary>
        /// <typeparam name="T">Класс в который нужно смапить.</typeparam>
        /// <param name="file">Файл.</param>
        /// <returns>Возвращает список смаппитых сущностей.</returns>
        public List<T> ImportExcel<T>(Stream file) where T : class
        {
            var result = new List<T>();



            return result;
        }

        /// <summary>
        /// Получаем все наименования свойств сущности.
        /// </summary>
        /// <typeparam name="T">Параметр.</typeparam>
        /// <param name="entity">Сущность.</param>
        /// <returns>Наименования свойств.</returns>
        private Dictionary<string, string> GetNameProperties<T>(T entity)
        {
            var result = _systemProperties;
            var properties = entity.GetType().GetProperties().Where(x => !result.ContainsKey(x.Name));

            foreach (var property in properties)
            {
                result.Add(property.Name, property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name);
            }

            return result;
        }
    }
}
