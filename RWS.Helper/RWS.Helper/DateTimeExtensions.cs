namespace RWS.Helper
{
    using System;

   
    public static class DateTimeExtensions
    {
        public static string ToStringDefaut(this DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        public static long DiffMinutes(this DateTime datetime, DateTime comparer)
        {
            return (long)datetime.Subtract(comparer).TotalMinutes;
        }    

    }
   
}