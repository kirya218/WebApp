using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Norbit.Srm.SeverMinerals.GenerateExcelFromXml;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace WebApp.Tools
{
    public class ExcelHelper
    {
        private readonly Dictionary<string, string> _systemProperties = new Dictionary<string, string>()
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
        public List<T> ImportExcel<T>(Type classType, Stream file) where T : class
        {
            var result = new List<T>();
            var document = SpreadsheetDocument.Open(file, false);
            var excel = new Excel(document);
            var sheets = excel.GetSheets();
            excel.SelectCurrentWorksheet(sheets.First().Key);

            var countElement = GetCountRows(excel);
            var startRow = 2U;

            while (countElement != 0)
            {
                if (!string.IsNullOrEmpty(excel.GetCellData($"A{startRow}")))
                {
                    result.Add(CreateItem<T>(excel, classType, startRow, false));
                }
                else if (!string.IsNullOrEmpty(excel.GetCellData($"D{startRow}")))
                {
                    result.Add(CreateItem<T>(excel, classType, startRow, true));
                }

                startRow++;
                countElement--;
            }

            return result;
        }

        /// <summary>
        /// Создание элемента из Excel.
        /// </summary>
        /// <typeparam name="T">Класс парсинга.</typeparam>
        /// <param name="excel">Excel.</param>
        /// <param name="classType">Название класса парсинга.</param>
        /// <param name="row">Строка.</param>
        /// <param name="isNew">Обозначение нового или старого значения.</param>
        /// <returns>Объект.</returns>
        private T CreateItem<T>(Excel excel, Type classType, uint row, bool isNew) where T : class
        {
            var obj = Activator.CreateInstance(classType) as T;
            var properties = GetNameProperties(obj);

            for (var j = 0; j < properties.Count; j++)
            {
                var key = properties.ElementAt(j).Key;
                var property = obj.GetType().GetProperty(key);

                if (property == null)
                {
                    continue;
                }

                if (isNew && _systemProperties.ContainsKey(key))
                {
                    if (property.PropertyType == typeof(Guid))
                    {
                        property.SetValue(obj, Guid.Empty);
                    }

                    if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(obj, DateTime.Now);
                    }

                    continue;
                }

                var value = GetValue(excel, properties.ElementAt(j), row);

                if (property.PropertyType == typeof(Guid))
                {
                    property.SetValue(obj, Guid.Parse(value));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    if (DateTime.TryParse(value, out var dateTime))
                    {
                        property.SetValue(obj, dateTime);
                    }
                    else
                    {
                        property.SetValue(obj, DateTime.FromOADate(double.Parse(value)));
                    }
                }
                else if (property.PropertyType == typeof(int))
                {
                    property.SetValue(obj, int.Parse(value));
                }
                else if (property.PropertyType == typeof(double))
                {
                    property.SetValue(obj, double.Parse(value));
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    property.SetValue(obj, decimal.Parse(value));
                }
                else if (property.PropertyType == typeof(byte[]))
                {
                    property.SetValue(obj, Encoding.Default.GetBytes(value));
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(obj, value);
                }
                else
                {
                    var classObjType = Type.GetType(property.PropertyType.FullName);
                    var classObj = Activator.CreateInstance(classObjType);
                    var classPropertyId = classObj.GetType().GetProperty("Id");
                    var excelValue = GetValue(excel, properties.ElementAt(j), row);

                    if (string.IsNullOrEmpty(excelValue))
                    {
                        continue;
                    }

                    classPropertyId?.SetValue(classObj, Guid.Parse(excelValue));
                    property?.SetValue(obj, classObj);
                }
            }

            return obj;
        }

        /// <summary>
        /// Возвращает кол-во строк в Excel.
        /// </summary>
        /// <param name="excel">Excel.</param>
        /// <returns>Кол-во строк.</returns>
        private uint GetCountRows(Excel excel)
        {
            uint count = 2U;

            while (!string.IsNullOrEmpty(excel.GetCellData($"A{count}")) || 
                   !string.IsNullOrEmpty(excel.GetCellData($"D{count}")))
            {
                count++;
            };

            return count;
        }

        /// <summary>
        /// Получение значения из Excel по свойствам.
        /// </summary>
        /// <param name="excel">Excel.</param>
        /// <param name="property">Значение свойства класса.</param>
        /// <param name="row">Строка.</param>
        /// <returns>Возвращает значение из Excel.</returns>
        private string GetValue(Excel excel, KeyValuePair<string, string> property, uint row)
        {
            var column = 1U;

            while (excel.GetCellData(Excel.GetCellPosition(1U, column)) != property.Value ||
                   string.IsNullOrWhiteSpace(excel.GetCellData(Excel.GetCellPosition(1U, column)))) 
            {
                column++;
            }

            return excel.GetCellData(Excel.GetCellPosition(row, column));
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
