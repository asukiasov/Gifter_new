using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.IO;

namespace SixtyThreeBits.Core.Utilities
{
    public class UtilityCollection
    {
        #region Properties
        public System.Globalization.CultureInfo CultureInvariant = System.Globalization.CultureInfo.InvariantCulture;
        public System.Globalization.CultureInfo CultureKA => new System.Globalization.CultureInfo("ka-ge");
        public System.Globalization.CultureInfo CultureUS => new System.Globalization.CultureInfo("en-us");
        AppSettingsCollection AppSettings;
        #endregion

        #region Constructors
        public UtilityCollection(AppSettingsCollection AppSettings)
        {
            this.AppSettings = AppSettings;
        }
        #endregion

        #region Methods
        public DateTime? AddDateByTimeUnit(DateTime? InputDate, Enums.TimeUnitCodes TimeUnitCode, int? TimeUnitValue)
        {
            if (InputDate.HasValue && TimeUnitValue.HasValue)
            {
                switch (TimeUnitCode)
                {
                    case Enums.TimeUnitCodes.MILLISECOND: { InputDate = InputDate.Value.AddMilliseconds(TimeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.SECOND: { InputDate = InputDate.Value.AddSeconds(TimeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.MINUTE: { InputDate = InputDate.Value.AddMinutes(TimeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.HOUR: { InputDate = InputDate.Value.AddHours(TimeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.DAY: { InputDate = InputDate.Value.AddDays(TimeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.WEEK: { InputDate = InputDate.Value.AddDays(TimeUnitValue.Value * 7); break; }
                    case Enums.TimeUnitCodes.MONTH: { InputDate = InputDate.Value.AddMonths(TimeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.YEAR: { InputDate = InputDate.Value.AddYears(TimeUnitValue.Value); break; }
                }
            }

            return InputDate;
        }

        public void DeleteUploadedFile(string Filename, string FolderPhysicalPath = null)
        {
            if (string.IsNullOrWhiteSpace(FolderPhysicalPath))
            {
                if (File.Exists($"{AppSettings.UploadFolderPhysicalPath}{Filename}"))
                {
                    File.Delete($"{AppSettings.UploadFolderPhysicalPath}{Filename}");
                }
            }
            else
            {
                if (File.Exists($"{FolderPhysicalPath}{Filename}"))
                {
                    File.Delete($"{FolderPhysicalPath}{Filename}");
                }
            }
        }

        public void DeleteFolder(string FolderPath)
        {
            if (Directory.Exists(FolderPath))
            {
                Directory.Delete(FolderPath, true);
            }
        }

        public static string FormatDate(object Date)
        {
            return string.Format(Constants.Formats.DateEval, Date);
        }

        public string FormatDateTime(object Date)
        {
            return string.Format(Constants.Formats.DateTimeEval, Date);
        }

        public string FormatDateTimeAsVerbal(DateTime? InputDate)
        {
            if (InputDate.HasValue)
            {
                var DaysPassed = Math.Round((DateTime.Now - InputDate.Value).TotalDays);
                string DateTimeString;
                switch (DaysPassed)
                {
                    case 0:
                        {
                            DateTimeString = $"Today {string.Format("{0:HH:mm}", InputDate)}";
                            break;
                        }
                    case 1:
                        {
                            DateTimeString = $"Yesterday {string.Format("{0:HH:mm}", InputDate)}";
                            break;
                        }
                    default:
                        {
                            DateTimeString = FormatDateTime(InputDate);
                            break;
                        }
                }
                return DateTimeString;
            }
            else
            {
                return null;
            }
        }

        public string FormatPrice(object Value, bool WithCurrencySign, string CurrencySign = "₾")
        {
            if (WithCurrencySign)
            {
                return string.Format("{0:#,#.#}{1}", Value, CurrencySign);
            }
            else
            {
                return string.Format("{0:#,#.#}", Value);
            }
        }

        public string FormatQuantity(object Value)
        {
            return string.Format("{0:#,#.#}", Value);
        }

        public string GetDatabaseErrorMessage(SixtyThreeBitsDataObject DALItem)
        {
            string ErrorMessage = null;
            if (DALItem != null)
            {
                if (DALItem.IsError)
                {
                    if (DALItem.IsClient)
                    {
                        ErrorMessage = DALItem.ErrorMessage;
                    }
                    else
                    {
                        ErrorMessage = Resources.TextError;
                    }
                }
            }

            return ErrorMessage;
        }

        public T GetValuesByLanguage<T>(string Culture = null, T GeorgianValue = default(T), T EnglishValue = default(T), T RussianValue = default(T))
        {
            switch (Culture)
            {
                case Enums.Languages.GEORGIAN: { return GeorgianValue; }
                case Enums.Languages.ENGLISH: { return EnglishValue; }
                case Enums.Languages.RUSSIAN: { return RussianValue; }
                default: { return GeorgianValue; }
            }
        }
        
        /// <summary>
        /// Get http path of file
        /// </summary>
        /// <param name="Filename">Name of the file</param>
        /// <param name="SubFolders">Subfolders string, that MUST NOT have slash in the beginning and MUST HAVE slash in the end, like sub1/sub2/sub3/ </param>
        /// <returns></returns>
        public string GetUploadedFileHttpPath(string Filename, string SubFolders = null)
        {
            return string.IsNullOrWhiteSpace(Filename) ? null : $"{AppSettings.UploadFolderVirtualPath}{SubFolders}{Filename}";
        }
        
        /// <summary>
        /// Get full physical path of file.
        /// </summary>
        /// <param name="Filename">Name of the file</param>
        /// <param name="SubFolders">Subfolders string, that MUST NOT have back slash in the beginning and MUST HAVE back slash in the end, like sub1\sub2\sub3\ </param>
        /// <returns></returns>
        public string GetUploadedFilePhysicalPath(string Filename, string SubFolders = null)
        {            
            return $"{AppSettings.UploadFolderPhysicalPath}{SubFolders}{Filename}";
        }                

        public bool IsImage(string Filename)
        {
            return string.IsNullOrWhiteSpace(Filename) ? false : new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" }.Contains(Path.GetExtension(Filename).ToUpper());
        }
        #endregion
    }
}