using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using System;
using System.IO;

namespace SixtyThreeBits.Core.Utilities
{
    public class UtilityCollection
    {
        #region Properties
        public System.Globalization.CultureInfo CultureInvariant = System.Globalization.CultureInfo.InvariantCulture;
        public System.Globalization.CultureInfo CultureKA => new System.Globalization.CultureInfo("ka-ge");
        public System.Globalization.CultureInfo CultureUS => new System.Globalization.CultureInfo("en-us");
        AppSettingsModel AppSettings;
        #endregion

        #region Constructors
        public UtilityCollection(AppSettingsModel AppSettings)
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
        
        public string GetUploadedFileHttpPath(string Filename)
        {
            return string.IsNullOrWhiteSpace(Filename) ? null : $"{AppSettings.UploadFolderHttpPath}{Filename}";
        }

        public string GetUploadedFilePhysicalPath(string Filename)
        {
            return $"{AppSettings.UploadFolderPhysicalPath}{Filename}";
        }        
        #endregion
    }    
}