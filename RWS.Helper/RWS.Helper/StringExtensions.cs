namespace RWS.Helper
{
    using System.Collections.Generic;

    public static class StringExtensions
    {
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !str.IsNullOrEmpty();
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string Join(this IEnumerable<string> array, string separator = ",")
        {
            return string.Join(separator, array);
        }

        public static string Format(this string toFormat, params object[] args)
        {
            return string.Format(toFormat, args);
        }

        public static string SubstrUntilOrHalf(this string str, string untilThisFirstCharOccurrence)
        {
            str = str ?? string.Empty;

            var result = str.LastIndexOf(untilThisFirstCharOccurrence) > -1 ?
                str.Substring(0, str.LastIndexOf(untilThisFirstCharOccurrence))
                : str.Substring(0, (int)decimal.Divide(str.Length, 2));


            return result;
        }

        public static string ReplaceASubsetOfThisWithASubsetUntilOrHalf(this string str, string subsetStart, string subsetEnd, string replaceWithValueUntil)
        {
            if (!(str ?? string.Empty).ToLowerInvariant().Contains(subsetStart.ToLowerInvariant()))
            {
                return str;
            }

            var indexSubsetStart = str.ToLowerInvariant().LastIndexOf(subsetStart.ToLowerInvariant()) + subsetStart.Length;

            var indexSubsetEnd = str.IndexOf(subsetEnd.ToLowerInvariant(),
                               indexSubsetStart);

            var subset = str.Substring(indexSubsetStart
                           ,
                               indexSubsetEnd.Equals(-1) ?
                               str.Length - indexSubsetStart :
                               indexSubsetEnd - indexSubsetStart
                           );

            var strResult = str.Replace(subset, subset.SubstrUntilOrHalf(replaceWithValueUntil)
                +
                (
                    indexSubsetEnd.Equals(-1)
                    ?
                        (subset.Trim().EndsWith("}") ? "}" : string.Empty)
                    :
                        (subset.Trim().EndsWith("\"") ? $"{replaceWithValueUntil}\"" : replaceWithValueUntil)
                ));

            return strResult;
        }

    }
}