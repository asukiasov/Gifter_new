using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace SixtyThreeBits.Core.Utilities
{
    public class UtilityCollection
    {
        #region Properties
        public readonly string ContentRootPath;
        public readonly string WebRootPath;        
        public readonly CultureInfo CultureInvariant;
        public readonly CultureInfo CultureKA;
        public readonly CultureInfo CultureUS;        

        public readonly ReadOnlyCollection<Language> SupportedLanguages;
        public readonly ReadOnlyCollection<CultureInfo> SupportedCultures;
        public readonly ReadOnlyCollection<string> SupportedLanguageStrings;
        public readonly string SupportedLanguagesRegex;
        public Language LanguageDefault;
        #endregion

        #region Constructors
        public UtilityCollection(string contentRootPath, string webRootPath)
        {
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;
            CultureInvariant = CultureInfo.InvariantCulture;
            CultureKA = new CultureInfo(Enums.Languages.GEORGIAN) { NumberFormat = new NumberFormatInfo { CurrencyDecimalSeparator = "." } };
            CultureUS = new CultureInfo(Enums.Languages.ENGLISH);
            SupportedLanguages = new List<Language>
            {
                new Language { LanguageCultureCode = Enums.Languages.GEORGIAN, LanguageName = "ქართული", Culture = CultureKA } ,
                new Language { LanguageCultureCode = Enums.Languages.ENGLISH, LanguageName = "English", Culture = CultureUS } ,
            }.AsReadOnly();
            SupportedCultures = SupportedLanguages.Select(item => item.Culture).ToList().AsReadOnly();
            SupportedLanguageStrings = SupportedLanguages.Select(item => item.LanguageCultureCode).ToList().AsReadOnly();
            SupportedLanguagesRegex = string.Join('|', SupportedLanguages.Select(item => item.LanguageCultureCode));

            LanguageDefault = SupportedLanguages.FirstOrDefault(item => item.LanguageCultureCode == Enums.Languages.GEORGIAN);
        }
        #endregion

        #region Methods
        public DateTime? AddDateByTimeUnit(DateTime? inputDate, Enums.TimeUnitCodes timeUnitCode, int? timeUnitValue)
        {
            if (inputDate.HasValue && timeUnitValue.HasValue)
            {
                switch (timeUnitCode)
                {
                    case Enums.TimeUnitCodes.MILLISECOND: { inputDate = inputDate.Value.AddMilliseconds(timeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.SECOND: { inputDate = inputDate.Value.AddSeconds(timeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.MINUTE: { inputDate = inputDate.Value.AddMinutes(timeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.HOUR: { inputDate = inputDate.Value.AddHours(timeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.DAY: { inputDate = inputDate.Value.AddDays(timeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.WEEK: { inputDate = inputDate.Value.AddDays(timeUnitValue.Value * 7); break; }
                    case Enums.TimeUnitCodes.MONTH: { inputDate = inputDate.Value.AddMonths(timeUnitValue.Value); break; }
                    case Enums.TimeUnitCodes.YEAR: { inputDate = inputDate.Value.AddYears(timeUnitValue.Value); break; }
                }
            }

            return inputDate;
        }

        public string FormatDate(object date, CultureInfo culture = null)
        {
            return string.Format(culture, Constants.Formats.DateEval, date);
        }

		public string FormatDateSqlParseFriendly(object date)
		{
			return date == null ? null : string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-ddTHH:mm:ss}", date);
		}

		public string FormatDateTime(object date)
        {
            return string.Format(Constants.Formats.DateTimeEval, date);
        }

        public string FormatDateTimeAsVerbal(DateTime? inputDate)
        {
            if (inputDate.HasValue)
            {
                var daysPassed = Math.Round((DateTime.Now - inputDate.Value).TotalDays);
                string dateTimeString;
                switch (daysPassed)
                {
                    case 0:
                        {
                            dateTimeString = $"Today {string.Format("{0:HH:mm}", inputDate)}";
                            break;
                        }
                    case 1:
                        {
                            dateTimeString = $"Yesterday {string.Format("{0:HH:mm}", inputDate)}";
                            break;
                        }
                    default:
                        {
                            dateTimeString = FormatDateTime(inputDate);
                            break;
                        }
                }
                return dateTimeString;
            }
            else
            {
                return null;
            }
        }        

        public string FormatFileSizeBytes(long? fileSizeBytes)
        {
            if (fileSizeBytes.HasValue)
            {
                var fileSizeBytesDouble = Convert.ToDouble(fileSizeBytes);
                if (fileSizeBytesDouble > 1073741823) // 1GB
                {
                    return $"{Math.Round(fileSizeBytesDouble / 1073741824, 1)}GB";
                }
                else if (fileSizeBytesDouble > 1048576) // 1MB
                {
                    return $"{Math.Round(fileSizeBytesDouble / 1048576, 1)}MB";
                }
                else if (fileSizeBytesDouble > 1023) // 1KB
                {
                    return $"{Math.Round(fileSizeBytesDouble / 1024, 0)}KB";
                }
                else
                {
                    return $"{fileSizeBytes} Bytes";
                }
            }
            else
            {
                return null;
            }
        }

        public string FormatPercent(object percent)
        {
            return string.Format("{0:0.##}", percent);
        }

        public string FormatPrice(object price, string currencySign = null)
        {
            string result;

            if (string.IsNullOrWhiteSpace(currencySign))
            {
                result = string.Format("{0:#,#.#}", price);
            }
            else
            {
                var isPrecedingCurrencySign = currencySign is "$" or "€" or "£";
                if (isPrecedingCurrencySign)
                {
                    result = string.Format("{1}{0:#,#.#}", price, currencySign);
                }
                else
                {
                    result = string.Format("{0:#,#.#}{1}", price, currencySign);
                }
            }

            return result;
        }

        public string FormatPriceValue(object price)
        {
            return string.Format("{0:#.##}", price);
        }

        public string FormatQuantity(object value)
        {
            return string.Format("{0:#,#.#}", value);
        }

        public string FormatQuantityValue(object value)
        {
            return string.Format("{0:#.#}", value);
        }

        public string GetResourceByKey(string resourcesKey, string languageCultureCode)
        {
            if (string.IsNullOrWhiteSpace(resourcesKey))
            {
                return null;
            }
            else
            {
                var rm = new ResourceManager(typeof(Resources));
                var resourceValue = string.IsNullOrWhiteSpace(languageCultureCode) ? rm.GetString(resourcesKey) : rm.GetString(resourcesKey, new CultureInfo(languageCultureCode));
                return resourceValue;
            }
        }

        public Language GetSupportedLanguageOrDefault(string languageCultureCode)
        {
            var language = SupportedLanguages.FirstOrDefault(item => item.LanguageCultureCode == languageCultureCode, LanguageDefault);
            return language;
        }

        public T GetValuesByLanguage<T>(string culture = null, T georgianValue = default, T englishValue = default)
        {
            switch (culture)
            {
                case Enums.Languages.GEORGIAN: { return georgianValue; }
                case Enums.Languages.ENGLISH: { return englishValue; }
                default: { return georgianValue; }
            }
        }

        // JWT Encoding
        /*
        public string JwtDecode(string jwtToken, string jwtEncodingSecretKey)
        {
            try
            {
                var json = JwtBuilder.Create()
                     .WithAlgorithm(new HMACSHA256Algorithm())
                     .MustVerifySignature()
                    .WithSecret(jwtEncodingSecretKey)
                    .Decode(jwtToken);
                return json;
            }
            catch
            {
                return null;
            }
        }
        public T JwtDecode<T>(string jwtToken, string jwtEncodingSecretKey)
        {
            var jwtokenDecoded = JwtDecode(jwtToken, jwtEncodingSecretKey);
            if (jwtokenDecoded == null)
            {
                return default(T);
            }
            else
            {
                var jObject = JObject.Parse(jwtokenDecoded);
                var payload = jObject["payload"].ToString();
                var result = payload.DeserializeJsonTo<T>();
                return result;
            }
        }
        public string JwtEncode(string payload, string jwtEncodingSecretKey, DateTime? expirationDate = null)
        {
            var builder = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .AddClaim("payload", payload)
                .WithSecret(jwtEncodingSecretKey);

            if (expirationDate.HasValue)
            {
                var dateTimeOffset = new DateTimeOffset(expirationDate.Value.ToUniversalTime());
                var ExpirationDateUnixTimeSeconds = dateTimeOffset.ToUnixTimeSeconds();
                builder.AddClaim("exp", ExpirationDateUnixTimeSeconds);
            }

            var result = builder.Encode();
            return result;
        }
        public string JwtEncode<T>(T payload, string jwtEncodingSecretKey, DateTime? expirationDate = null) where T : class
        {
            var result = JwtEncode(
                payload: payload.ToJson(),
                jwtEncodingSecretKey: jwtEncodingSecretKey,
                expirationDate: expirationDate
            );
            return result;
        }
        */
        #endregion

        #region Nested Classes
        public class Language
        {
            #region Properties
            public string LanguageName { get; set; }
            public string LanguageCultureCode { get; set; }
            public CultureInfo Culture { get; set; }
            #endregion

            #region Methods
            public override string ToString()
            {
                return $"{LanguageCultureCode} - {LanguageName}";
            }
            #endregion
        }
        #endregion
    }

    public static class UtilityExtensions
    {
        #region Methods
        public static T MapViaJsonSerialization<T>(this object source)
        {
            var json = source.ToJson();
            var destination = json.DeserializeJsonTo<T>();
            return destination;
        }
        #endregion
    }
}