using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Threading.Tasks;

namespace RWS.Helper
{
    public enum GetNumbersSrtLocation
    {
        Start,
        End,
        Middle,
        All
    }
    public static class GMethods
    {
        /// <summary>
        /// execute action in the leaf object fist
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="node"></param>
        /// <param name="action"></param>
        public static void TreeForEachFistLeaf<TSource>(this TSource node, Action<TSource> action) where TSource: IRWSTreeNode<TSource>
        {
            if(node.Children != null)
                foreach (var nodeChild in node.Children)
                {                    
                    nodeChild.TreeForEachFistLeaf<TSource>(action);
                }
            action(node.Data);
        }

        /// <summary>
        /// execute action in the root object first
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="node"></param>
        /// <param name="action"></param>
        public static void TreeForEachFirstRoot<TSource>(this TSource node, Action<TSource> action) where TSource : IRWSTreeNode<TSource>
        {
            action(node.Data);
            if (node.Children != null)
                foreach (var nodeChild in node.Children)
                {
                    nodeChild.TreeForEachFirstRoot<TSource>(action);
                }
        }

        public static void RWSUnZipFile(this string path, string destinationFileName)
        {
            ZipFile.ExtractToDirectory(path, destinationFileName);
        }
        
        public static int RWSToUxTmStp(this DateTime dt, DateTimeKind dtk = DateTimeKind.Unspecified)
        {
            return (int)(dt.ToUniversalTime().Subtract(new DateTime(1970, 1, 1,0,0,0, DateTimeKind.Utc))).TotalSeconds;
        }

        public static DateTime RWSUxTmStpToDTime(this string unixTimeStamp, DateTimeKind dtk = DateTimeKind.Unspecified)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, dtk);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp.ToInt());
            return dtDateTime;
        }

        public static IQueryable<TSource> RWSPaginate<TSource>(this IQueryable<TSource> data, int pageIndex, int pageSize)
        {
            return  data.Skip(pageSize * pageIndex).Take(pageSize);
        }

        public static string RWSRemoveAccents(this string value)
        {
            const string StrComAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç'�";
            const string StrSemAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc _";

            for (var i = 0; i <= StrComAcentos.Length; i++)
                value = value.Replace(StrComAcentos[i].ToString().Trim(), StrSemAcentos[i].ToString().Trim());

            return value;
        }

        public static IEnumerable<TSource> RWSDistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string RWSRndStr(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return Enumerable.Repeat(IntPtr.Zero, length)
              .Select(s => chars[random.Next(chars.Length)]).RWSCharJoin("");
        }

        public static string RWSWtDbugNote(this Exception e, string customMessage = "", params object[] customMessageData)
        {
            string errorText = string.Format(customMessage, customMessageData) + "Support Error:" + GetInnerExMSGRecursive(e) + "\n\nSupport Stack:" + e.StackTrace;
            Debug.WriteLine(errorText);
            return errorText;
        }

        private static string GetInnerExMSGRecursive(Exception e)
        {
            string innerMessage = string.Empty;
            if (e.InnerException != null)
                innerMessage = GetInnerExMSGRecursive(e.InnerException)+"\n";
            return e.Message + innerMessage;
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="pkName"></param>
        /// <param name="pkValue">pkValue Have to be of the same type of primary key type</param>
        /// <returns></returns>
        public static bool RWSFindDim<T>(this T entity, string pkName, object pkValue) where T : class
        {
            var result = false;
            
            var propT = entity.GetType().GetProperty(pkName);

            if (propT != null)
            {
                var value = propT.GetValue(entity, null);
                result = (value??(string.Empty)).Equals(pkValue);
            }

            return result;
        }

        public static bool RWSIsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string RWSConcat(this object text, object toConcat, string separator = " - ")
        {
            return text + separator + (toConcat??"");
        }

        public static TSource RWSExeInLineF<TSource>(this TSource data, Func<TSource,TSource> inlineCode)
        {
            return inlineCode(data);           
        }
        public static void RWSExeInLine<TSource>(this TSource data, Action<TSource> inlineCode)
        {
            inlineCode(data);
        }
        /// <summary>
        /// Gets source object and return it into one Array
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="data"></param>
        /// <returns>TSource[]</returns>
        public static TSource[] RWSWrapInArray<TSource>(this TSource obj)
        {
            return new TSource[] { obj };
        }

        public static string RWSNameOfProp<T, TProp>(this T o, Expression<Func<T, TProp>> propertySelector)
        {
            MemberExpression body = (MemberExpression)propertySelector.Body;
            return body.Member.Name;
        }

        public static IEnumerable<TSource> RWSForEach<TSource>(this IEnumerable<TSource> data, Action<TSource> action)
        {           
            foreach (var x in data)
            {
                action(x);
            }            
            return data;
        }

        public static string RWSStrJoin(this IEnumerable<string> data, string separator=",")
        {
            return string.Join(separator, data);
        }

        public static string RWSCharJoin(this IEnumerable<char> data, string separator)
        {
            return string.Join(separator, data);
        }

        /// <summary>
        /// Get Numbers Inside a String (Middle Gets After Non Number Char Until End Or Non Number Char)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string RWSGetNumbers(this string value, GetNumbersSrtLocation location= GetNumbersSrtLocation.All)
        {            
            var numbers = "0123456789";
            var ret = "";
            if (!value.RWSIsEmpty())
            {
                value = value.Trim();
                switch (location)
                {
                    case GetNumbersSrtLocation.Start:
                        ret= value.TakeWhile(tw => numbers.Contains(tw)).RWSCharJoin("");
                        break;
                    case GetNumbersSrtLocation.End:                        
                        ret = value.Reverse().TakeWhile(tw => numbers.Contains(tw)).Reverse().RWSCharJoin("");
                        break;
                    case GetNumbersSrtLocation.Middle:
                        ret= value.SkipWhile(sw => !numbers.Contains(sw)).TakeWhile(tw => numbers.Contains(tw)).RWSCharJoin("");
                        break;
                    case GetNumbersSrtLocation.All:
                        ret= value.Where(w => numbers.Contains(w)).RWSCharJoin("");
                        break;
                }
            }
            return ret;
        }        
    }
}