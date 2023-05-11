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
                var stylesPart = new ExcelWorkbookStylesPart();
                var name = entities.First().GetType().Name;

                var excel = new Excel(package);
                excel.CreateExcel($"{name} {DateTime.Now.ToShortDateString()}");
                excel.CreateWorkbook();
                excel.CreateStyles(stylesPart);
                excel.CreateSheet(name);

                excel.SetColumnsWidth(new Dictionary<uint, double>()
                {
                    { 1U, 40.42578125D }, { 2U, 24.28515625D }, { 3U, 26.42578125D },
                    { 4U, 21.140625D }, { 5U, 36.7109375D }, { 6U, 31.140625D },
                    { 7U, 16.7109375D }, { 8U, 27D }, { 9U, 26.140625D },
                    { 10U, 27.7109375D }, { 11U, 11.140625D }, { 12U, 40.140625D },
                    { 13U, 14.5703125D }
                });

                var properties = GetNameProperties(entities.First());

                for (var i = 0; i < properties.Count; i++)
                {
                    excel.Append(1U, (uint)i + 1, properties.ElementAt(i).Value, CellValues.SharedString, 2U);
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

                        if (_systemProperties.ContainsKey(properties.ElementAt(j).Key))
                        {
                            excel.Append((uint)i + 2, (uint)j + 1, propertyValue?.ToString() ?? string.Empty, CellValues.SharedString, 1U);
                        }
                        else
                        {
                            excel.Append((uint)i + 2, (uint)j + 1, propertyValue?.ToString() ?? string.Empty, CellValues.SharedString, 3U);
                        }
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

    public class ExcelWorkbookStylesPart : IWorkbookStylesPart
    {
        public virtual void GenerateWorkbookStylesPart(SpreadsheetDocument Document)
        {
            var workbookPart = Document.WorkbookPart;
            var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();

            var stylesheet1 = new Stylesheet()
            {
                MCAttributes = new MarkupCompatibilityAttributes()
                {
                    Ignorable = "x14ac x16r2 xr"
                }
            };

            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            stylesheet1.AddNamespaceDeclaration("x16r2", "http://schemas.microsoft.com/office/spreadsheetml/2015/02/main");
            stylesheet1.AddNamespaceDeclaration("xr", "http://schemas.microsoft.com/office/spreadsheetml/2014/revision");

            var fonts1 = new Fonts() { Count = (UInt32Value)19U, KnownFonts = true };

            var font1 = new Font();
            var fontSize1 = new FontSize() { Val = 11D };
            var color1 = new Color() { Theme = (UInt32Value)1U };
            var fontName1 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet1 = new FontCharSet() { Val = 204 };
            var fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontCharSet1);
            font1.Append(fontScheme1);

            var font2 = new Font();
            var fontSize2 = new FontSize() { Val = 11D };
            var color2 = new Color() { Theme = (UInt32Value)1U };
            var fontName2 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet2 = new FontCharSet() { Val = 204 };
            var fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontCharSet2);
            font2.Append(fontScheme2);

            var font3 = new Font();
            var fontSize3 = new FontSize() { Val = 18D };
            var color3 = new Color() { Theme = (UInt32Value)3U };
            var fontName3 = new FontName() { Val = "Calibri Light" };
            var fontFamilyNumbering3 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet3 = new FontCharSet() { Val = 204 };
            var fontScheme3 = new FontScheme() { Val = FontSchemeValues.Major };

            font3.Append(fontSize3);
            font3.Append(color3);
            font3.Append(fontName3);
            font3.Append(fontFamilyNumbering3);
            font3.Append(fontCharSet3);
            font3.Append(fontScheme3);

            var font4 = new Font();
            var bold1 = new Bold();
            var fontSize4 = new FontSize() { Val = 15D };
            var color4 = new Color() { Theme = (UInt32Value)3U };
            var fontName4 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering4 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet4 = new FontCharSet() { Val = 204 };
            var fontScheme4 = new FontScheme() { Val = FontSchemeValues.Minor };

            font4.Append(bold1);
            font4.Append(fontSize4);
            font4.Append(color4);
            font4.Append(fontName4);
            font4.Append(fontFamilyNumbering4);
            font4.Append(fontCharSet4);
            font4.Append(fontScheme4);

            var font5 = new Font();
            var bold2 = new Bold();
            var fontSize5 = new FontSize() { Val = 13D };
            var color5 = new Color() { Theme = (UInt32Value)3U };
            var fontName5 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering5 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet5 = new FontCharSet() { Val = 204 };
            var fontScheme5 = new FontScheme() { Val = FontSchemeValues.Minor };

            font5.Append(bold2);
            font5.Append(fontSize5);
            font5.Append(color5);
            font5.Append(fontName5);
            font5.Append(fontFamilyNumbering5);
            font5.Append(fontCharSet5);
            font5.Append(fontScheme5);

            var font6 = new Font();
            var bold3 = new Bold();
            var fontSize6 = new FontSize() { Val = 11D };
            var color6 = new Color() { Theme = (UInt32Value)3U };
            var fontName6 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering6 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet6 = new FontCharSet() { Val = 204 };
            var fontScheme6 = new FontScheme() { Val = FontSchemeValues.Minor };

            font6.Append(bold3);
            font6.Append(fontSize6);
            font6.Append(color6);
            font6.Append(fontName6);
            font6.Append(fontFamilyNumbering6);
            font6.Append(fontCharSet6);
            font6.Append(fontScheme6);

            var font7 = new Font();
            var fontSize7 = new FontSize() { Val = 11D };
            var color7 = new Color() { Rgb = "FF006100" };
            var fontName7 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering7 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet7 = new FontCharSet() { Val = 204 };
            var fontScheme7 = new FontScheme() { Val = FontSchemeValues.Minor };

            font7.Append(fontSize7);
            font7.Append(color7);
            font7.Append(fontName7);
            font7.Append(fontFamilyNumbering7);
            font7.Append(fontCharSet7);
            font7.Append(fontScheme7);

            var font8 = new Font();
            var fontSize8 = new FontSize() { Val = 11D };
            var color8 = new Color() { Rgb = "FF9C0006" };
            var fontName8 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering8 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet8 = new FontCharSet() { Val = 204 };
            var fontScheme8 = new FontScheme() { Val = FontSchemeValues.Minor };

            font8.Append(fontSize8);
            font8.Append(color8);
            font8.Append(fontName8);
            font8.Append(fontFamilyNumbering8);
            font8.Append(fontCharSet8);
            font8.Append(fontScheme8);

            var font9 = new Font();
            var fontSize9 = new FontSize() { Val = 11D };
            var color9 = new Color() { Rgb = "FF9C5700" };
            var fontName9 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering9 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet9 = new FontCharSet() { Val = 204 };
            var fontScheme9 = new FontScheme() { Val = FontSchemeValues.Minor };

            font9.Append(fontSize9);
            font9.Append(color9);
            font9.Append(fontName9);
            font9.Append(fontFamilyNumbering9);
            font9.Append(fontCharSet9);
            font9.Append(fontScheme9);

            var font10 = new Font();
            var fontSize10 = new FontSize() { Val = 11D };
            var color10 = new Color() { Rgb = "FF3F3F76" };
            var fontName10 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering10 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet10 = new FontCharSet() { Val = 204 };
            var fontScheme10 = new FontScheme() { Val = FontSchemeValues.Minor };

            font10.Append(fontSize10);
            font10.Append(color10);
            font10.Append(fontName10);
            font10.Append(fontFamilyNumbering10);
            font10.Append(fontCharSet10);
            font10.Append(fontScheme10);

            var font11 = new Font();
            var bold4 = new Bold();
            var fontSize11 = new FontSize() { Val = 11D };
            var color11 = new Color() { Rgb = "FF3F3F3F" };
            var fontName11 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering11 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet11 = new FontCharSet() { Val = 204 };
            var fontScheme11 = new FontScheme() { Val = FontSchemeValues.Minor };

            font11.Append(bold4);
            font11.Append(fontSize11);
            font11.Append(color11);
            font11.Append(fontName11);
            font11.Append(fontFamilyNumbering11);
            font11.Append(fontCharSet11);
            font11.Append(fontScheme11);

            var font12 = new Font();
            var bold5 = new Bold();
            var fontSize12 = new FontSize() { Val = 11D };
            var color12 = new Color() { Rgb = "FFFA7D00" };
            var fontName12 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering12 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet12 = new FontCharSet() { Val = 204 };
            var fontScheme12 = new FontScheme() { Val = FontSchemeValues.Minor };

            font12.Append(bold5);
            font12.Append(fontSize12);
            font12.Append(color12);
            font12.Append(fontName12);
            font12.Append(fontFamilyNumbering12);
            font12.Append(fontCharSet12);
            font12.Append(fontScheme12);

            var font13 = new Font();
            var fontSize13 = new FontSize() { Val = 11D };
            var color13 = new Color() { Rgb = "FFFA7D00" };
            var fontName13 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering13 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet13 = new FontCharSet() { Val = 204 };
            var fontScheme13 = new FontScheme() { Val = FontSchemeValues.Minor };

            font13.Append(fontSize13);
            font13.Append(color13);
            font13.Append(fontName13);
            font13.Append(fontFamilyNumbering13);
            font13.Append(fontCharSet13);
            font13.Append(fontScheme13);

            var font14 = new Font();
            var bold6 = new Bold();
            var fontSize14 = new FontSize() { Val = 11D };
            var color14 = new Color() { Theme = (UInt32Value)0U };
            var fontName14 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering14 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet14 = new FontCharSet() { Val = 204 };
            var fontScheme14 = new FontScheme() { Val = FontSchemeValues.Minor };

            font14.Append(bold6);
            font14.Append(fontSize14);
            font14.Append(color14);
            font14.Append(fontName14);
            font14.Append(fontFamilyNumbering14);
            font14.Append(fontCharSet14);
            font14.Append(fontScheme14);

            var font15 = new Font();
            var fontSize15 = new FontSize() { Val = 11D };
            var color15 = new Color() { Rgb = "FFFF0000" };
            var fontName15 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering15 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet15 = new FontCharSet() { Val = 204 };
            var fontScheme15 = new FontScheme() { Val = FontSchemeValues.Minor };

            font15.Append(fontSize15);
            font15.Append(color15);
            font15.Append(fontName15);
            font15.Append(fontFamilyNumbering15);
            font15.Append(fontCharSet15);
            font15.Append(fontScheme15);

            var font16 = new Font();
            var italic1 = new Italic();
            var fontSize16 = new FontSize() { Val = 11D };
            var color16 = new Color() { Rgb = "FF7F7F7F" };
            var fontName16 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering16 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet16 = new FontCharSet() { Val = 204 };
            var fontScheme16 = new FontScheme() { Val = FontSchemeValues.Minor };

            font16.Append(italic1);
            font16.Append(fontSize16);
            font16.Append(color16);
            font16.Append(fontName16);
            font16.Append(fontFamilyNumbering16);
            font16.Append(fontCharSet16);
            font16.Append(fontScheme16);

            var font17 = new Font();
            var bold7 = new Bold();
            var fontSize17 = new FontSize() { Val = 11D };
            var color17 = new Color() { Theme = (UInt32Value)1U };
            var fontName17 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering17 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet17 = new FontCharSet() { Val = 204 };
            var fontScheme17 = new FontScheme() { Val = FontSchemeValues.Minor };

            font17.Append(bold7);
            font17.Append(fontSize17);
            font17.Append(color17);
            font17.Append(fontName17);
            font17.Append(fontFamilyNumbering17);
            font17.Append(fontCharSet17);
            font17.Append(fontScheme17);

            var font18 = new Font();
            var fontSize18 = new FontSize() { Val = 11D };
            var color18 = new Color() { Theme = (UInt32Value)0U };
            var fontName18 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering18 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet18 = new FontCharSet() { Val = 204 };
            var fontScheme18 = new FontScheme() { Val = FontSchemeValues.Minor };

            font18.Append(fontSize18);
            font18.Append(color18);
            font18.Append(fontName18);
            font18.Append(fontFamilyNumbering18);
            font18.Append(fontCharSet18);
            font18.Append(fontScheme18);

            var font19 = new Font();
            var bold8 = new Bold();
            var fontSize19 = new FontSize() { Val = 12D };
            var color19 = new Color() { Theme = (UInt32Value)1U };
            var fontName19 = new FontName() { Val = "Calibri" };
            var fontFamilyNumbering19 = new FontFamilyNumbering() { Val = 2 };
            var fontCharSet19 = new FontCharSet() { Val = 204 };
            var fontScheme19 = new FontScheme() { Val = FontSchemeValues.Minor };

            font19.Append(bold8);
            font19.Append(fontSize19);
            font19.Append(color19);
            font19.Append(fontName19);
            font19.Append(fontFamilyNumbering19);
            font19.Append(fontCharSet19);
            font19.Append(fontScheme19);

            fonts1.Append(font1);
            fonts1.Append(font2);
            fonts1.Append(font3);
            fonts1.Append(font4);
            fonts1.Append(font5);
            fonts1.Append(font6);
            fonts1.Append(font7);
            fonts1.Append(font8);
            fonts1.Append(font9);
            fonts1.Append(font10);
            fonts1.Append(font11);
            fonts1.Append(font12);
            fonts1.Append(font13);
            fonts1.Append(font14);
            fonts1.Append(font15);
            fonts1.Append(font16);
            fonts1.Append(font17);
            fonts1.Append(font18);
            fonts1.Append(font19);

            var fills1 = new Fills() { Count = (UInt32Value)34U };

            var fill1 = new Fill();
            var patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            var fill2 = new Fill();
            var patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            var fill3 = new Fill();

            var patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor1 = new ForegroundColor() { Rgb = "FFC6EFCE" };

            patternFill3.Append(foregroundColor1);

            fill3.Append(patternFill3);

            var fill4 = new Fill();

            var patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor2 = new ForegroundColor() { Rgb = "FFFFC7CE" };

            patternFill4.Append(foregroundColor2);

            fill4.Append(patternFill4);

            var fill5 = new Fill();

            var patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor3 = new ForegroundColor() { Rgb = "FFFFEB9C" };

            patternFill5.Append(foregroundColor3);

            fill5.Append(patternFill5);

            var fill6 = new Fill();

            var patternFill6 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor4 = new ForegroundColor() { Rgb = "FFFFCC99" };

            patternFill6.Append(foregroundColor4);

            fill6.Append(patternFill6);

            var fill7 = new Fill();

            var patternFill7 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor5 = new ForegroundColor() { Rgb = "FFF2F2F2" };

            patternFill7.Append(foregroundColor5);

            fill7.Append(patternFill7);

            var fill8 = new Fill();

            var patternFill8 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor6 = new ForegroundColor() { Rgb = "FFA5A5A5" };

            patternFill8.Append(foregroundColor6);

            fill8.Append(patternFill8);

            var fill9 = new Fill();

            var patternFill9 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor7 = new ForegroundColor() { Rgb = "FFFFFFCC" };

            patternFill9.Append(foregroundColor7);

            fill9.Append(patternFill9);

            var fill10 = new Fill();

            var patternFill10 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor8 = new ForegroundColor() { Theme = (UInt32Value)4U };

            patternFill10.Append(foregroundColor8);

            fill10.Append(patternFill10);

            var fill11 = new Fill();

            var patternFill11 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor9 = new ForegroundColor() { Theme = (UInt32Value)4U, Tint = 0.79998168889431442D };
            var backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill11.Append(foregroundColor9);
            patternFill11.Append(backgroundColor1);

            fill11.Append(patternFill11);

            var fill12 = new Fill();

            var patternFill12 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor10 = new ForegroundColor() { Theme = (UInt32Value)4U, Tint = 0.59999389629810485D };
            var backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill12.Append(foregroundColor10);
            patternFill12.Append(backgroundColor2);

            fill12.Append(patternFill12);

            var fill13 = new Fill();

            var patternFill13 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor11 = new ForegroundColor() { Theme = (UInt32Value)4U, Tint = 0.39997558519241921D };
            var backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill13.Append(foregroundColor11);
            patternFill13.Append(backgroundColor3);

            fill13.Append(patternFill13);

            var fill14 = new Fill();

            var patternFill14 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor12 = new ForegroundColor() { Theme = (UInt32Value)5U };

            patternFill14.Append(foregroundColor12);

            fill14.Append(patternFill14);

            var fill15 = new Fill();

            var patternFill15 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor13 = new ForegroundColor() { Theme = (UInt32Value)5U, Tint = 0.79998168889431442D };
            var backgroundColor4 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill15.Append(foregroundColor13);
            patternFill15.Append(backgroundColor4);

            fill15.Append(patternFill15);

            var fill16 = new Fill();

            var patternFill16 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor14 = new ForegroundColor() { Theme = (UInt32Value)5U, Tint = 0.59999389629810485D };
            var backgroundColor5 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill16.Append(foregroundColor14);
            patternFill16.Append(backgroundColor5);

            fill16.Append(patternFill16);

            var fill17 = new Fill();

            var patternFill17 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor15 = new ForegroundColor() { Theme = (UInt32Value)5U, Tint = 0.39997558519241921D };
            var backgroundColor6 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill17.Append(foregroundColor15);
            patternFill17.Append(backgroundColor6);

            fill17.Append(patternFill17);

            var fill18 = new Fill();

            var patternFill18 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor16 = new ForegroundColor() { Theme = (UInt32Value)6U };

            patternFill18.Append(foregroundColor16);

            fill18.Append(patternFill18);

            var fill19 = new Fill();

            var patternFill19 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor17 = new ForegroundColor() { Theme = (UInt32Value)6U, Tint = 0.79998168889431442D };
            var backgroundColor7 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill19.Append(foregroundColor17);
            patternFill19.Append(backgroundColor7);

            fill19.Append(patternFill19);

            var fill20 = new Fill();

            var patternFill20 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor18 = new ForegroundColor() { Theme = (UInt32Value)6U, Tint = 0.59999389629810485D };
            var backgroundColor8 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill20.Append(foregroundColor18);
            patternFill20.Append(backgroundColor8);

            fill20.Append(patternFill20);

            var fill21 = new Fill();

            var patternFill21 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor19 = new ForegroundColor() { Theme = (UInt32Value)6U, Tint = 0.39997558519241921D };
            var backgroundColor9 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill21.Append(foregroundColor19);
            patternFill21.Append(backgroundColor9);

            fill21.Append(patternFill21);

            var fill22 = new Fill();

            var patternFill22 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor20 = new ForegroundColor() { Theme = (UInt32Value)7U };

            patternFill22.Append(foregroundColor20);

            fill22.Append(patternFill22);

            var fill23 = new Fill();

            var patternFill23 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor21 = new ForegroundColor() { Theme = (UInt32Value)7U, Tint = 0.79998168889431442D };
            var backgroundColor10 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill23.Append(foregroundColor21);
            patternFill23.Append(backgroundColor10);

            fill23.Append(patternFill23);

            var fill24 = new Fill();

            var patternFill24 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor22 = new ForegroundColor() { Theme = (UInt32Value)7U, Tint = 0.59999389629810485D };
            var backgroundColor11 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill24.Append(foregroundColor22);
            patternFill24.Append(backgroundColor11);

            fill24.Append(patternFill24);

            var fill25 = new Fill();

            var patternFill25 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor23 = new ForegroundColor() { Theme = (UInt32Value)7U, Tint = 0.39997558519241921D };
            var backgroundColor12 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill25.Append(foregroundColor23);
            patternFill25.Append(backgroundColor12);

            fill25.Append(patternFill25);

            var fill26 = new Fill();

            var patternFill26 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor24 = new ForegroundColor() { Theme = (UInt32Value)8U };

            patternFill26.Append(foregroundColor24);

            fill26.Append(patternFill26);

            var fill27 = new Fill();

            var patternFill27 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor25 = new ForegroundColor() { Theme = (UInt32Value)8U, Tint = 0.79998168889431442D };
            var backgroundColor13 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill27.Append(foregroundColor25);
            patternFill27.Append(backgroundColor13);

            fill27.Append(patternFill27);

            var fill28 = new Fill();

            var patternFill28 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor26 = new ForegroundColor() { Theme = (UInt32Value)8U, Tint = 0.59999389629810485D };
            var backgroundColor14 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill28.Append(foregroundColor26);
            patternFill28.Append(backgroundColor14);

            fill28.Append(patternFill28);

            var fill29 = new Fill();

            var patternFill29 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor27 = new ForegroundColor() { Theme = (UInt32Value)8U, Tint = 0.39997558519241921D };
            var backgroundColor15 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill29.Append(foregroundColor27);
            patternFill29.Append(backgroundColor15);

            fill29.Append(patternFill29);

            var fill30 = new Fill();

            var patternFill30 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor28 = new ForegroundColor() { Theme = (UInt32Value)9U };

            patternFill30.Append(foregroundColor28);

            fill30.Append(patternFill30);

            var fill31 = new Fill();

            var patternFill31 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor29 = new ForegroundColor() { Theme = (UInt32Value)9U, Tint = 0.79998168889431442D };
            var backgroundColor16 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill31.Append(foregroundColor29);
            patternFill31.Append(backgroundColor16);

            fill31.Append(patternFill31);

            var fill32 = new Fill();

            var patternFill32 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor30 = new ForegroundColor() { Theme = (UInt32Value)9U, Tint = 0.59999389629810485D };
            var backgroundColor17 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill32.Append(foregroundColor30);
            patternFill32.Append(backgroundColor17);

            fill32.Append(patternFill32);

            var fill33 = new Fill();

            var patternFill33 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor31 = new ForegroundColor() { Theme = (UInt32Value)9U, Tint = 0.39997558519241921D };
            var backgroundColor18 = new BackgroundColor() { Indexed = (UInt32Value)65U };

            patternFill33.Append(foregroundColor31);
            patternFill33.Append(backgroundColor18);

            fill33.Append(patternFill33);

            var fill34 = new Fill();

            var patternFill34 = new PatternFill() { PatternType = PatternValues.Solid };
            var foregroundColor32 = new ForegroundColor() { Theme = (UInt32Value)4U, Tint = 0.59999389629810485D };
            var backgroundColor19 = new BackgroundColor() { Indexed = (UInt32Value)64U };

            patternFill34.Append(foregroundColor32);
            patternFill34.Append(backgroundColor19);

            fill34.Append(patternFill34);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);
            fills1.Append(fill6);
            fills1.Append(fill7);
            fills1.Append(fill8);
            fills1.Append(fill9);
            fills1.Append(fill10);
            fills1.Append(fill11);
            fills1.Append(fill12);
            fills1.Append(fill13);
            fills1.Append(fill14);
            fills1.Append(fill15);
            fills1.Append(fill16);
            fills1.Append(fill17);
            fills1.Append(fill18);
            fills1.Append(fill19);
            fills1.Append(fill20);
            fills1.Append(fill21);
            fills1.Append(fill22);
            fills1.Append(fill23);
            fills1.Append(fill24);
            fills1.Append(fill25);
            fills1.Append(fill26);
            fills1.Append(fill27);
            fills1.Append(fill28);
            fills1.Append(fill29);
            fills1.Append(fill30);
            fills1.Append(fill31);
            fills1.Append(fill32);
            fills1.Append(fill33);
            fills1.Append(fill34);

            var borders1 = new Borders() { Count = (UInt32Value)11U };

            var border1 = new Border();
            var leftBorder1 = new LeftBorder();
            var rightBorder1 = new RightBorder();
            var topBorder1 = new TopBorder();
            var bottomBorder1 = new BottomBorder();
            var diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            var border2 = new Border();
            var leftBorder2 = new LeftBorder();
            var rightBorder2 = new RightBorder();
            var topBorder2 = new TopBorder();

            var bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thick };
            var color20 = new Color() { Theme = (UInt32Value)4U };

            bottomBorder2.Append(color20);
            var diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            var border3 = new Border();
            var leftBorder3 = new LeftBorder();
            var rightBorder3 = new RightBorder();
            var topBorder3 = new TopBorder();

            var bottomBorder3 = new BottomBorder() { Style = BorderStyleValues.Thick };
            var color21 = new Color() { Theme = (UInt32Value)4U, Tint = 0.499984740745262D };

            bottomBorder3.Append(color21);
            var diagonalBorder3 = new DiagonalBorder();

            border3.Append(leftBorder3);
            border3.Append(rightBorder3);
            border3.Append(topBorder3);
            border3.Append(bottomBorder3);
            border3.Append(diagonalBorder3);

            var border4 = new Border();
            var leftBorder4 = new LeftBorder();
            var rightBorder4 = new RightBorder();
            var topBorder4 = new TopBorder();

            var bottomBorder4 = new BottomBorder() { Style = BorderStyleValues.Medium };
            var color22 = new Color() { Theme = (UInt32Value)4U, Tint = 0.39997558519241921D };

            bottomBorder4.Append(color22);
            var diagonalBorder4 = new DiagonalBorder();

            border4.Append(leftBorder4);
            border4.Append(rightBorder4);
            border4.Append(topBorder4);
            border4.Append(bottomBorder4);
            border4.Append(diagonalBorder4);

            var border5 = new Border();

            var leftBorder5 = new LeftBorder() { Style = BorderStyleValues.Thin };
            var color23 = new Color() { Rgb = "FF7F7F7F" };

            leftBorder5.Append(color23);

            var rightBorder5 = new RightBorder() { Style = BorderStyleValues.Thin };
            var color24 = new Color() { Rgb = "FF7F7F7F" };

            rightBorder5.Append(color24);

            var topBorder5 = new TopBorder() { Style = BorderStyleValues.Thin };
            var color25 = new Color() { Rgb = "FF7F7F7F" };

            topBorder5.Append(color25);

            var bottomBorder5 = new BottomBorder() { Style = BorderStyleValues.Thin };
            var color26 = new Color() { Rgb = "FF7F7F7F" };

            bottomBorder5.Append(color26);
            var diagonalBorder5 = new DiagonalBorder();

            border5.Append(leftBorder5);
            border5.Append(rightBorder5);
            border5.Append(topBorder5);
            border5.Append(bottomBorder5);
            border5.Append(diagonalBorder5);

            var border6 = new Border();

            var leftBorder6 = new LeftBorder() { Style = BorderStyleValues.Thin };
            var color27 = new Color() { Rgb = "FF3F3F3F" };

            leftBorder6.Append(color27);

            var rightBorder6 = new RightBorder() { Style = BorderStyleValues.Thin };
            var color28 = new Color() { Rgb = "FF3F3F3F" };

            rightBorder6.Append(color28);

            var topBorder6 = new TopBorder() { Style = BorderStyleValues.Thin };
            var color29 = new Color() { Rgb = "FF3F3F3F" };

            topBorder6.Append(color29);

            var bottomBorder6 = new BottomBorder() { Style = BorderStyleValues.Thin };
            var color30 = new Color() { Rgb = "FF3F3F3F" };

            bottomBorder6.Append(color30);
            var diagonalBorder6 = new DiagonalBorder();

            border6.Append(leftBorder6);
            border6.Append(rightBorder6);
            border6.Append(topBorder6);
            border6.Append(bottomBorder6);
            border6.Append(diagonalBorder6);

            var border7 = new Border();
            var leftBorder7 = new LeftBorder();
            var rightBorder7 = new RightBorder();
            var topBorder7 = new TopBorder();

            var bottomBorder7 = new BottomBorder() { Style = BorderStyleValues.Double };
            var color31 = new Color() { Rgb = "FFFF8001" };

            bottomBorder7.Append(color31);
            var diagonalBorder7 = new DiagonalBorder();

            border7.Append(leftBorder7);
            border7.Append(rightBorder7);
            border7.Append(topBorder7);
            border7.Append(bottomBorder7);
            border7.Append(diagonalBorder7);

            var border8 = new Border();

            var leftBorder8 = new LeftBorder() { Style = BorderStyleValues.Double };
            var color32 = new Color() { Rgb = "FF3F3F3F" };

            leftBorder8.Append(color32);

            var rightBorder8 = new RightBorder() { Style = BorderStyleValues.Double };
            var color33 = new Color() { Rgb = "FF3F3F3F" };

            rightBorder8.Append(color33);

            var topBorder8 = new TopBorder() { Style = BorderStyleValues.Double };
            var color34 = new Color() { Rgb = "FF3F3F3F" };

            topBorder8.Append(color34);

            var bottomBorder8 = new BottomBorder() { Style = BorderStyleValues.Double };
            var color35 = new Color() { Rgb = "FF3F3F3F" };

            bottomBorder8.Append(color35);
            var diagonalBorder8 = new DiagonalBorder();

            border8.Append(leftBorder8);
            border8.Append(rightBorder8);
            border8.Append(topBorder8);
            border8.Append(bottomBorder8);
            border8.Append(diagonalBorder8);

            var border9 = new Border();

            var leftBorder9 = new LeftBorder() { Style = BorderStyleValues.Thin };
            var color36 = new Color() { Rgb = "FFB2B2B2" };

            leftBorder9.Append(color36);

            var rightBorder9 = new RightBorder() { Style = BorderStyleValues.Thin };
            var color37 = new Color() { Rgb = "FFB2B2B2" };

            rightBorder9.Append(color37);

            var topBorder9 = new TopBorder() { Style = BorderStyleValues.Thin };
            var color38 = new Color() { Rgb = "FFB2B2B2" };

            topBorder9.Append(color38);

            var bottomBorder9 = new BottomBorder() { Style = BorderStyleValues.Thin };
            var color39 = new Color() { Rgb = "FFB2B2B2" };

            bottomBorder9.Append(color39);
            var diagonalBorder9 = new DiagonalBorder();

            border9.Append(leftBorder9);
            border9.Append(rightBorder9);
            border9.Append(topBorder9);
            border9.Append(bottomBorder9);
            border9.Append(diagonalBorder9);

            var border10 = new Border();
            var leftBorder10 = new LeftBorder();
            var rightBorder10 = new RightBorder();

            var topBorder10 = new TopBorder() { Style = BorderStyleValues.Thin };
            var color40 = new Color() { Theme = (UInt32Value)4U };

            topBorder10.Append(color40);

            var bottomBorder10 = new BottomBorder() { Style = BorderStyleValues.Double };
            var color41 = new Color() { Theme = (UInt32Value)4U };

            bottomBorder10.Append(color41);
            var diagonalBorder10 = new DiagonalBorder();

            border10.Append(leftBorder10);
            border10.Append(rightBorder10);
            border10.Append(topBorder10);
            border10.Append(bottomBorder10);
            border10.Append(diagonalBorder10);

            var border11 = new Border();

            var leftBorder11 = new LeftBorder() { Style = BorderStyleValues.Medium };
            var color42 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder11.Append(color42);

            var rightBorder11 = new RightBorder() { Style = BorderStyleValues.Medium };
            var color43 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder11.Append(color43);

            var topBorder11 = new TopBorder() { Style = BorderStyleValues.Medium };
            var color44 = new Color() { Indexed = (UInt32Value)64U };

            topBorder11.Append(color44);

            var bottomBorder11 = new BottomBorder() { Style = BorderStyleValues.Medium };
            var color45 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder11.Append(color45);
            var diagonalBorder11 = new DiagonalBorder();

            border11.Append(leftBorder11);
            border11.Append(rightBorder11);
            border11.Append(topBorder11);
            border11.Append(bottomBorder11);
            border11.Append(diagonalBorder11);

            borders1.Append(border1);
            borders1.Append(border2);
            borders1.Append(border3);
            borders1.Append(border4);
            borders1.Append(border5);
            borders1.Append(border6);
            borders1.Append(border7);
            borders1.Append(border8);
            borders1.Append(border9);
            borders1.Append(border10);
            borders1.Append(border11);

            var cellStyleFormats1 = new CellStyleFormats()
            {
                Count = (UInt32Value)42U
            };

            var cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };
            var cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)4U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)2U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)5U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)3U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat6 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)5U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat7 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)6U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat8 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)7U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat9 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)8U, FillId = (UInt32Value)4U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat10 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)9U, FillId = (UInt32Value)5U, BorderId = (UInt32Value)4U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat11 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)10U, FillId = (UInt32Value)6U, BorderId = (UInt32Value)5U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat12 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)11U, FillId = (UInt32Value)6U, BorderId = (UInt32Value)4U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat13 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)12U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)6U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat14 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)13U, FillId = (UInt32Value)7U, BorderId = (UInt32Value)7U, ApplyNumberFormat = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat15 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)14U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat16 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)8U, BorderId = (UInt32Value)8U, ApplyNumberFormat = false, ApplyFont = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat17 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)15U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyFill = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat18 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)16U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)9U, ApplyNumberFormat = false, ApplyFill = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat19 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)17U, FillId = (UInt32Value)9U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat20 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)10U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat21 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)11U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat22 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)12U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat23 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)17U, FillId = (UInt32Value)13U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat24 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)14U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat25 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)15U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat26 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)16U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat27 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)17U, FillId = (UInt32Value)17U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat28 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)18U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat29 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)19U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat30 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)20U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat31 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)17U, FillId = (UInt32Value)21U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat32 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)22U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat33 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)23U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat34 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)24U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat35 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)17U, FillId = (UInt32Value)25U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat36 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)26U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat37 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)27U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat38 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)28U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat39 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)17U, FillId = (UInt32Value)29U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat40 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)30U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat41 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)31U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            var cellFormat42 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)32U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };

            cellStyleFormats1.Append(cellFormat1);
            cellStyleFormats1.Append(cellFormat2);
            cellStyleFormats1.Append(cellFormat3);
            cellStyleFormats1.Append(cellFormat4);
            cellStyleFormats1.Append(cellFormat5);
            cellStyleFormats1.Append(cellFormat6);
            cellStyleFormats1.Append(cellFormat7);
            cellStyleFormats1.Append(cellFormat8);
            cellStyleFormats1.Append(cellFormat9);
            cellStyleFormats1.Append(cellFormat10);
            cellStyleFormats1.Append(cellFormat11);
            cellStyleFormats1.Append(cellFormat12);
            cellStyleFormats1.Append(cellFormat13);
            cellStyleFormats1.Append(cellFormat14);
            cellStyleFormats1.Append(cellFormat15);
            cellStyleFormats1.Append(cellFormat16);
            cellStyleFormats1.Append(cellFormat17);
            cellStyleFormats1.Append(cellFormat18);
            cellStyleFormats1.Append(cellFormat19);
            cellStyleFormats1.Append(cellFormat20);
            cellStyleFormats1.Append(cellFormat21);
            cellStyleFormats1.Append(cellFormat22);
            cellStyleFormats1.Append(cellFormat23);
            cellStyleFormats1.Append(cellFormat24);
            cellStyleFormats1.Append(cellFormat25);
            cellStyleFormats1.Append(cellFormat26);
            cellStyleFormats1.Append(cellFormat27);
            cellStyleFormats1.Append(cellFormat28);
            cellStyleFormats1.Append(cellFormat29);
            cellStyleFormats1.Append(cellFormat30);
            cellStyleFormats1.Append(cellFormat31);
            cellStyleFormats1.Append(cellFormat32);
            cellStyleFormats1.Append(cellFormat33);
            cellStyleFormats1.Append(cellFormat34);
            cellStyleFormats1.Append(cellFormat35);
            cellStyleFormats1.Append(cellFormat36);
            cellStyleFormats1.Append(cellFormat37);
            cellStyleFormats1.Append(cellFormat38);
            cellStyleFormats1.Append(cellFormat39);
            cellStyleFormats1.Append(cellFormat40);
            cellStyleFormats1.Append(cellFormat41);
            cellStyleFormats1.Append(cellFormat42);

            var cellFormats1 = new CellFormats() { Count = (UInt32Value)5U };
            var cellFormat43 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

            var cellFormat44 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyAlignment = true };
            var alignment1 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center };

            cellFormat44.Append(alignment1);

            var cellFormat45 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)18U, FillId = (UInt32Value)33U, BorderId = (UInt32Value)10U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            var alignment2 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center };

            cellFormat45.Append(alignment2);

            var cellFormat46 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyAlignment = true, ApplyProtection = true };
            var alignment3 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center };
            var protection1 = new Protection() { Locked = false };

            cellFormat46.Append(alignment3);
            cellFormat46.Append(protection1);

            var cellFormat47 = new CellFormat() { NumberFormatId = (UInt32Value)22U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true, ApplyAlignment = true, ApplyProtection = true };
            var alignment4 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center };
            var protection2 = new Protection() { Locked = false };

            cellFormat47.Append(alignment4);
            cellFormat47.Append(protection2);

            cellFormats1.Append(cellFormat43);
            cellFormats1.Append(cellFormat44);
            cellFormats1.Append(cellFormat45);
            cellFormats1.Append(cellFormat46);
            cellFormats1.Append(cellFormat47);

            var cellStyles1 = new CellStyles() { Count = (UInt32Value)42U };
            var cellStyle1 = new CellStyle() { Name = "20% — акцент1", FormatId = (UInt32Value)19U, BuiltinId = (UInt32Value)30U, CustomBuiltin = true };
            var cellStyle2 = new CellStyle() { Name = "20% — акцент2", FormatId = (UInt32Value)23U, BuiltinId = (UInt32Value)34U, CustomBuiltin = true };
            var cellStyle3 = new CellStyle() { Name = "20% — акцент3", FormatId = (UInt32Value)27U, BuiltinId = (UInt32Value)38U, CustomBuiltin = true };
            var cellStyle4 = new CellStyle() { Name = "20% — акцент4", FormatId = (UInt32Value)31U, BuiltinId = (UInt32Value)42U, CustomBuiltin = true };
            var cellStyle5 = new CellStyle() { Name = "20% — акцент5", FormatId = (UInt32Value)35U, BuiltinId = (UInt32Value)46U, CustomBuiltin = true };
            var cellStyle6 = new CellStyle() { Name = "20% — акцент6", FormatId = (UInt32Value)39U, BuiltinId = (UInt32Value)50U, CustomBuiltin = true };
            var cellStyle7 = new CellStyle() { Name = "40% — акцент1", FormatId = (UInt32Value)20U, BuiltinId = (UInt32Value)31U, CustomBuiltin = true };
            var cellStyle8 = new CellStyle() { Name = "40% — акцент2", FormatId = (UInt32Value)24U, BuiltinId = (UInt32Value)35U, CustomBuiltin = true };
            var cellStyle9 = new CellStyle() { Name = "40% — акцент3", FormatId = (UInt32Value)28U, BuiltinId = (UInt32Value)39U, CustomBuiltin = true };
            var cellStyle10 = new CellStyle() { Name = "40% — акцент4", FormatId = (UInt32Value)32U, BuiltinId = (UInt32Value)43U, CustomBuiltin = true };
            var cellStyle11 = new CellStyle() { Name = "40% — акцент5", FormatId = (UInt32Value)36U, BuiltinId = (UInt32Value)47U, CustomBuiltin = true };
            var cellStyle12 = new CellStyle() { Name = "40% — акцент6", FormatId = (UInt32Value)40U, BuiltinId = (UInt32Value)51U, CustomBuiltin = true };
            var cellStyle13 = new CellStyle() { Name = "60% — акцент1", FormatId = (UInt32Value)21U, BuiltinId = (UInt32Value)32U, CustomBuiltin = true };
            var cellStyle14 = new CellStyle() { Name = "60% — акцент2", FormatId = (UInt32Value)25U, BuiltinId = (UInt32Value)36U, CustomBuiltin = true };
            var cellStyle15 = new CellStyle() { Name = "60% — акцент3", FormatId = (UInt32Value)29U, BuiltinId = (UInt32Value)40U, CustomBuiltin = true };
            var cellStyle16 = new CellStyle() { Name = "60% — акцент4", FormatId = (UInt32Value)33U, BuiltinId = (UInt32Value)44U, CustomBuiltin = true };
            var cellStyle17 = new CellStyle() { Name = "60% — акцент5", FormatId = (UInt32Value)37U, BuiltinId = (UInt32Value)48U, CustomBuiltin = true };
            var cellStyle18 = new CellStyle() { Name = "60% — акцент6", FormatId = (UInt32Value)41U, BuiltinId = (UInt32Value)52U, CustomBuiltin = true };
            var cellStyle19 = new CellStyle() { Name = "Акцент1", FormatId = (UInt32Value)18U, BuiltinId = (UInt32Value)29U, CustomBuiltin = true };
            var cellStyle20 = new CellStyle() { Name = "Акцент2", FormatId = (UInt32Value)22U, BuiltinId = (UInt32Value)33U, CustomBuiltin = true };
            var cellStyle21 = new CellStyle() { Name = "Акцент3", FormatId = (UInt32Value)26U, BuiltinId = (UInt32Value)37U, CustomBuiltin = true };
            var cellStyle22 = new CellStyle() { Name = "Акцент4", FormatId = (UInt32Value)30U, BuiltinId = (UInt32Value)41U, CustomBuiltin = true };
            var cellStyle23 = new CellStyle() { Name = "Акцент5", FormatId = (UInt32Value)34U, BuiltinId = (UInt32Value)45U, CustomBuiltin = true };
            var cellStyle24 = new CellStyle() { Name = "Акцент6", FormatId = (UInt32Value)38U, BuiltinId = (UInt32Value)49U, CustomBuiltin = true };
            var cellStyle25 = new CellStyle() { Name = "Ввод ", FormatId = (UInt32Value)9U, BuiltinId = (UInt32Value)20U, CustomBuiltin = true };
            var cellStyle26 = new CellStyle() { Name = "Вывод", FormatId = (UInt32Value)10U, BuiltinId = (UInt32Value)21U, CustomBuiltin = true };
            var cellStyle27 = new CellStyle() { Name = "Вычисление", FormatId = (UInt32Value)11U, BuiltinId = (UInt32Value)22U, CustomBuiltin = true };
            var cellStyle28 = new CellStyle() { Name = "Заголовок 1", FormatId = (UInt32Value)2U, BuiltinId = (UInt32Value)16U, CustomBuiltin = true };
            var cellStyle29 = new CellStyle() { Name = "Заголовок 2", FormatId = (UInt32Value)3U, BuiltinId = (UInt32Value)17U, CustomBuiltin = true };
            var cellStyle30 = new CellStyle() { Name = "Заголовок 3", FormatId = (UInt32Value)4U, BuiltinId = (UInt32Value)18U, CustomBuiltin = true };
            var cellStyle31 = new CellStyle() { Name = "Заголовок 4", FormatId = (UInt32Value)5U, BuiltinId = (UInt32Value)19U, CustomBuiltin = true };
            var cellStyle32 = new CellStyle() { Name = "Итог", FormatId = (UInt32Value)17U, BuiltinId = (UInt32Value)25U, CustomBuiltin = true };
            var cellStyle33 = new CellStyle() { Name = "Контрольная ячейка", FormatId = (UInt32Value)13U, BuiltinId = (UInt32Value)23U, CustomBuiltin = true };
            var cellStyle34 = new CellStyle() { Name = "Название", FormatId = (UInt32Value)1U, BuiltinId = (UInt32Value)15U, CustomBuiltin = true };
            var cellStyle35 = new CellStyle() { Name = "Нейтральный", FormatId = (UInt32Value)8U, BuiltinId = (UInt32Value)28U, CustomBuiltin = true };
            var cellStyle36 = new CellStyle() { Name = "Обычный", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
            var cellStyle37 = new CellStyle() { Name = "Плохой", FormatId = (UInt32Value)7U, BuiltinId = (UInt32Value)27U, CustomBuiltin = true };
            var cellStyle38 = new CellStyle() { Name = "Пояснение", FormatId = (UInt32Value)16U, BuiltinId = (UInt32Value)53U, CustomBuiltin = true };
            var cellStyle39 = new CellStyle() { Name = "Примечание", FormatId = (UInt32Value)15U, BuiltinId = (UInt32Value)10U, CustomBuiltin = true };
            var cellStyle40 = new CellStyle() { Name = "Связанная ячейка", FormatId = (UInt32Value)12U, BuiltinId = (UInt32Value)24U, CustomBuiltin = true };
            var cellStyle41 = new CellStyle() { Name = "Текст предупреждения", FormatId = (UInt32Value)14U, BuiltinId = (UInt32Value)11U, CustomBuiltin = true };
            var cellStyle42 = new CellStyle() { Name = "Хороший", FormatId = (UInt32Value)6U, BuiltinId = (UInt32Value)26U, CustomBuiltin = true };

            cellStyles1.Append(cellStyle1);
            cellStyles1.Append(cellStyle2);
            cellStyles1.Append(cellStyle3);
            cellStyles1.Append(cellStyle4);
            cellStyles1.Append(cellStyle5);
            cellStyles1.Append(cellStyle6);
            cellStyles1.Append(cellStyle7);
            cellStyles1.Append(cellStyle8);
            cellStyles1.Append(cellStyle9);
            cellStyles1.Append(cellStyle10);
            cellStyles1.Append(cellStyle11);
            cellStyles1.Append(cellStyle12);
            cellStyles1.Append(cellStyle13);
            cellStyles1.Append(cellStyle14);
            cellStyles1.Append(cellStyle15);
            cellStyles1.Append(cellStyle16);
            cellStyles1.Append(cellStyle17);
            cellStyles1.Append(cellStyle18);
            cellStyles1.Append(cellStyle19);
            cellStyles1.Append(cellStyle20);
            cellStyles1.Append(cellStyle21);
            cellStyles1.Append(cellStyle22);
            cellStyles1.Append(cellStyle23);
            cellStyles1.Append(cellStyle24);
            cellStyles1.Append(cellStyle25);
            cellStyles1.Append(cellStyle26);
            cellStyles1.Append(cellStyle27);
            cellStyles1.Append(cellStyle28);
            cellStyles1.Append(cellStyle29);
            cellStyles1.Append(cellStyle30);
            cellStyles1.Append(cellStyle31);
            cellStyles1.Append(cellStyle32);
            cellStyles1.Append(cellStyle33);
            cellStyles1.Append(cellStyle34);
            cellStyles1.Append(cellStyle35);
            cellStyles1.Append(cellStyle36);
            cellStyles1.Append(cellStyle37);
            cellStyles1.Append(cellStyle38);
            cellStyles1.Append(cellStyle39);
            cellStyles1.Append(cellStyle40);
            cellStyles1.Append(cellStyle41);
            cellStyles1.Append(cellStyle42);

            var differentialFormats1 = new DifferentialFormats()
            {
                Count = (UInt32Value)0U
            };

            var tableStyles1 = new TableStyles()
            {
                Count = (UInt32Value)0U,
                DefaultTableStyle = "TableStyleMedium2",
                DefaultPivotStyle = "PivotStyleLight16"
            };

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);

            workbookStylesPart.Stylesheet = stylesheet1;
        }
    }
}
