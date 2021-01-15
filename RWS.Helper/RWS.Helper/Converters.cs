using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace RWS.Helper
{
    public static partial class Converters
    {
        public static string ToStringS(this object value, ToStringSmarter toStringSmarter)
        {
            return toStringSmarter.ToStrSmarter(value);
        }

        public static string ToPolygon(this string value)
        {
            var split = value.Split(new string[] { "," }, StringSplitOptions.None);
            var polygonPostgis = string.Empty;
            for (var i = 0; i < split.Length - 1; i = i + 2)
            {
                if (i > 0)
                    polygonPostgis += ",";

                polygonPostgis += split[i] + " " + split[i + 1] + " 1";
            }

            return polygonPostgis;
        }

        public static string HexToString(this string hexString)
        {
            return string.Join("", Regex.Split(hexString, "(?<=\\G..)(?!$)").Select(x => (char)Convert.ToByte(x, 16)));
        }

        public static IEnumerable<object> ToIEnumerable(this object iEnumerable)
        {
            return (IEnumerable<object>)iEnumerable;
        }

        public static string ToMD5(this string value)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return string.Join("", md5Hash.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(dt => dt.ToString("x2")));
            }
        }

        public static string AlignCenter(this string value, int totalSize, char fillSpacesBy = ' ')
        {
            var collsToCenter = (totalSize - value.Length) / 2;
            if (collsToCenter > 0)
                value = value.PadLeft(totalSize - collsToCenter, fillSpacesBy).PadRight(totalSize, fillSpacesBy);

            return value;
        }

        public static decimal? ToNullDecimal(this object value, CultureInfo cult = null, int decimalsIFDataNoHaveFloatPoint = 0, string decimalSeparator = ".")
        {
            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()) && !value.ToString().ToLower().Equals(""))
            {
                var ret = new Nullable<decimal>(value.ToDecimal(cult, decimalsIFDataNoHaveFloatPoint, decimalSeparator));
                if (ret.GetValueOrDefault(0) == decimal.MaxValue)
                    return new Nullable<decimal>();
                else
                    return ret;
            }
            return new Nullable<decimal>();
        }

        public static decimal ToDecimal(this object value, CultureInfo cult = null, int decimalsIFDataNoHaveFloatPoint = 0, string decimalSeparator = ".")
        {
            try
            {
                if (value is decimal)
                    return (decimal)value;

                var decsep = decimalSeparator.Substring(0, 1);

                var vle = value is string ? (string)value : value.ToString();
                if (decimalsIFDataNoHaveFloatPoint > 0 && !vle.Contains(decsep))
                    vle = vle.Insert(vle.Length - decimalsIFDataNoHaveFloatPoint, decsep);
                if (cult != null && cult.Name.Equals("pt-BR"))
                    vle = (vle.LastIndexOf('.') > vle.LastIndexOf(',')) ? vle.Replace('.', '_').Replace(',', '.').Replace('_', ',') : vle;
                else if (cult != null && cult.Name.Equals("en-US"))
                    vle = (vle.LastIndexOf('.') < vle.LastIndexOf(',')) ? vle.Replace('.', '_').Replace(',', '.').Replace('_', ',') : vle;
                else
                    return (decimal.Parse(vle));
                return (decimal.Parse(vle, cult));
            }
            catch
            {
                if (decimalSeparator.Length > 1)
                    return Decimal.MaxValue;
                return decimal.Zero;
            }
        }

        public static char? ToNullChar(this string value)
        {
            if (value.Trim().Length > 0)
                return new Nullable<char>(((string)value)[0]);
            return new Nullable<char>();
        }

        /// <summary>
        /// return first element not whitespace from string or a char whitespace value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char ToChar(this string value)
        {
            if (value.Trim().Length > 0)
                return value.Trim()[0];
            return ' ';
        }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <param name="stringObject">The string object</param>
        /// <returns>The string</returns>
        public static string ToString(this object stringObject)
        {
            if (stringObject == null)
                return string.Empty;
            else if (stringObject is string)
                return ((string)stringObject).Trim();
            else
                return stringObject.ToString();
        }

        /// <summary>
        ///
        /// </summary>

        /// <param name="option"> split default or custom OptFormats by string " opt " and get the format by the index passed</param>
        /// <param name="OptFormarts"></param>
        /// <returns></returns>
        public static string ToStringNow(this DateTime now, int option = 0, string OptFormarts = "dd/MM/yy HH:mm:ss opt dd/MM/yy opt HH:mm:ss opt ddMMyyHHmmss opt dd/MM/yyyy")
        {
            return now.ToString(OptFormarts.Split(new string[] { " opt " }, StringSplitOptions.RemoveEmptyEntries)[option]);
        }

        public static string GetStackMax(this Exception e, int maxLeghtRTL = 10)
        {
            return e.StackTrace.Length > maxLeghtRTL ? e.StackTrace.Substring(e.StackTrace.Length - maxLeghtRTL) : e.StackTrace;
        }

        public static string GetInnerExcMSG(this Exception e, bool isRecursive = false, string formatBefore = "\n\nINNER:")
        {
            var innerText = "";
            while (isRecursive && e.InnerException != null)
            {
                innerText += e.InnerException.Message;
                e = e.InnerException;
            }
            innerText = e.InnerException != null ? e.InnerException.Message : "" + innerText;
            return string.IsNullOrWhiteSpace(innerText) ? "" : formatBefore + innerText;
        }

        public static string ToStringDateOrNullDate(this object dateObject, string format = "dd/MM/yyyy")
        {
            if (dateObject == null)
                return string.Empty;
            else
            if (dateObject is string)
                return ((string)dateObject).Trim();
            else
                if (dateObject is DateTime)
                return dateObject.ToDateTime().ToString(format);
            else if (dateObject is DateTime?)
                return dateObject.ToNullDateTime().Value.ToString(format);

            return string.Empty;
        }

        public static bool ToBool(this object boolObject)
        {
            if (boolObject == null)
                return false;
            else if (boolObject is bool)
                return (bool)boolObject;
            else if (boolObject is string && string.IsNullOrWhiteSpace(boolObject.ToString()))
                return false;
            else if (boolObject is string && "true false".Contains(boolObject.ToString().ToLowerInvariant()))
                return bool.Parse(boolObject.ToString());

            return false;
        }

        public static bool? ToNullBool(this object boolObject)
        {
            if (boolObject == null)
                return null;
            else if (boolObject is bool)
                return (bool)boolObject;
            else if (boolObject is string && !string.IsNullOrEmpty(boolObject.ToString()) && "truefalse".Contains(boolObject.ToString().ToLowerInvariant()))
                return bool.Parse(boolObject.ToString());
            else if (boolObject is string && string.IsNullOrWhiteSpace(boolObject.ToString()))
                return null;

            return false;
        }

        /// <summary>
        /// Converts to Int32
        /// </summary>
        /// <param name="intObject">The Int object</param>
        /// <returns>The Int32</returns>
        public static int ToInt(this object intObject)
        {

            int.TryParse((intObject ?? "").ToString().Trim(), out int data);

            return data;
        }

        public static short ToShort(this object intObject)
        {
            short.TryParse((intObject ?? "").ToString().Trim(), out short data);
            return data;
        }

        public static short? ToNullShort(this object intObject)
        {
            if (short.TryParse(intObject.ToString().Trim(), out short data))
                return data;
            else
                return null;
        }

        public static long ToLong(this object intObject)
        {
            long.TryParse((intObject ?? "").ToString().Trim(), out long data);
            return data;
        }

        public static long? ToNullLong(this object intObject)
        {
            long? data = new Nullable<long>();
            try
            {
                if (long.TryParse(intObject.ToString().Trim(), out long lng))
                    return lng;
                else
                    return data;
            }
            catch {
                return data;
            }            
        }

        /// <summary>
        /// Try convert a type Object or type string Object to Guid
        /// </summary>
        /// <param name="guidOrStringGuidObject"></param>
        /// <returns>The Guid Inside Object, String</returns>
        public static Guid ToGuid(this object guidOrStringGuidObject)
        {
            if (guidOrStringGuidObject is Guid)
                return (Guid)guidOrStringGuidObject;
            else
                return new Guid(guidOrStringGuidObject.ToString());
        }

        public static Guid? ToNullGuid(this object guidOrStringGuidOrNull)
        {
            if (guidOrStringGuidOrNull == null)
                return null;
            try
            {
                return new Nullable<Guid>(guidOrStringGuidOrNull.ToGuid());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ToStringFromNuableObject(this object nuableObject, string format = null)
        {
            if (nuableObject == null || string.IsNullOrWhiteSpace(nuableObject.ToString()))
                return string.Empty;
            else if (string.IsNullOrWhiteSpace(format))
                return nuableObject.ToString();
            else
            {
                try
                {
                    var method = nuableObject.GetType().GetMethod("ToString", new Type[] { typeof(string) });
                    return method.Invoke(nuableObject, new object[] { format }).ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public static DateTime ToDateTime(this object dateTimeObject, string format = "dd/MM/yyyy")
        {
            if (dateTimeObject is DateTime)
            {
                return (DateTime)dateTimeObject;
            }
            else
            {
                try
                {
                    return DateTime.ParseExact(dateTimeObject.ToString(), format, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.ParseExact(dateTimeObject.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }
        }

        public static string ToStringDateTimeNullLongTicks(this long? ticks, string format = "dd/MM/yyyy")
        {
            if (ticks.HasValue)
            {
                return (new DateTime(ticks.Value)).ToString(format);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Converts to Nullable Int32
        /// </summary>
        /// <param name="intObject">The Int object</param>
        /// <returns>The Int32?</returns>
        public static int? ToNullInt(this object intObject)
        {
            try
            {
                if (int.TryParse(intObject.ToString().Trim(), out int data))
                    return data;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ToNullDateTime(this object dateTimeObject, string format = "dd/MM/yyyy")
        {
            if (dateTimeObject is DateTime)
                return (DateTime)dateTimeObject;
            else
            {
                if (DateTime.TryParseExact(dateTimeObject.ToString(), format, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out DateTime date))
                    return date;
                else if (DateTime.TryParseExact(dateTimeObject.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out date))
                    return date;
                else
                    return null;
            }
        }

        /// <summary>
        /// Converts string to time span
        /// </summary>
        /// <param name="timeObject">The time string</param>
        /// <returns>The time span</returns>
        public static TimeSpan? ToTimeSpan(this object timeObject)
        {
            if (!(timeObject is string) || string.IsNullOrEmpty((string)timeObject))
                return null;
            else
            {
                if (timeObject.ToString().StartsWith("-"))
                    return (new TimeSpan(int.Parse(timeObject.ToString().Substring(1, 2)), int.Parse(timeObject.ToString().Substring(3, 2)), 0)).Negate();
                else
                    return new TimeSpan(int.Parse(timeObject.ToString().Substring(0, 2)), int.Parse(timeObject.ToString().Substring(2, 2)), 0);
            }
        }

        public static void CopyTo(this Object S, Object T)
        {
            foreach (var pS in S.GetType().GetProperties())
            {
                foreach (var pT in T.GetType().GetProperties())
                {
                    if (pT.Name != pS.Name) continue;
                    (pT.GetSetMethod()).Invoke(T, new object[] { pS.GetGetMethod().Invoke(S, null) });
                }
            }
        }

        /// <summary>
        /// Calculates diurnal time between two DateTime
        /// </summary>
        /// <param name="init">The init date time</param>
        /// <param name="end">The end date time</param>
        /// <returns>The diurnal time</returns>
        public static TimeSpan CalculateDiurnalTime(this DateTime init, DateTime end, int startDiurnalTime = 6, int endDiurnalTime = 21)
        {
            if (init.TimeOfDay < TimeSpan.FromHours(startDiurnalTime))
            {
                if (end.TimeOfDay < TimeSpan.FromHours(startDiurnalTime))
                    return TimeSpan.Zero;
                else if (end.TimeOfDay < TimeSpan.FromHours(endDiurnalTime))
                    return end.TimeOfDay - TimeSpan.FromHours(startDiurnalTime);
                else
                    return TimeSpan.FromHours(15);
            }
            else if (init.TimeOfDay < TimeSpan.FromHours(endDiurnalTime))
            {
                if (end.TimeOfDay < TimeSpan.FromHours(startDiurnalTime))
                    return TimeSpan.FromHours(endDiurnalTime) - init.TimeOfDay;
                else if (end.TimeOfDay < TimeSpan.FromHours(endDiurnalTime))
                    return end - init;
                else
                    return TimeSpan.FromHours(endDiurnalTime) - init.TimeOfDay;
            }
            else
            {
                if (end.TimeOfDay < TimeSpan.FromHours(startDiurnalTime))
                    return TimeSpan.Zero;
                else if (end.TimeOfDay < TimeSpan.FromHours(endDiurnalTime))
                    return end.TimeOfDay - TimeSpan.FromHours(startDiurnalTime);
                else
                    return TimeSpan.Zero;
            }
        }
              
        public static dynamic ToTypeName(this object data, string typeName)
        {
            typeName = typeName.ToLower();

            if (typeName.Contains("string"))
            {
                return data.ToString();
            }
            else if (typeName.Contains("guid"))
                if (typeName.Contains("null"))
                    return data.ToNullGuid();
                else
                    return data.ToGuid();
            else if (typeName.Contains("int"))
                if (typeName.Contains("null"))
                    return data.ToNullInt();
                else
                    return data.ToInt();
            else if (typeName.Contains("datetime"))
            {
                if (!string.IsNullOrWhiteSpace(data.ToStringFromNuableObject()) && data.ToStringFromNuableObject().Length < 10)
                    data = "01/" + data;

                if (typeName.Contains("null"))

                    return data.ToNullDateTime();
                else
                    return data.ToDateTime();
            }
            else if (typeName.Contains("decimal"))
            {
                var cult = new CultureInfo("pt-BR");
                if (typeName.Contains("null"))
                    return data.ToNullDecimal(cult);
                else
                    return data.ToDecimal(cult);
            }
            else if (typeName.Contains("bool"))
                if (data.ToString().Equals("true,false"))
                    return true;
                else
                if (typeName.Contains("null"))
                    return data.ToNullBool();
                else
                    return data.ToBool();
            else if (typeName.Contains("char"))
            {
                return data.ToStringFromNuableObject().ToChar();
            }

            return data;
        }
    }
}