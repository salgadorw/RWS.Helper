using System.Linq;
using System.Reflection;

namespace RWS.Helper
{
    public static class PT_BR_Month
    {
        public static readonly string Janeiro_1 = "Janeiro";
        public static readonly string Fevereiro_2 = "Fevereiro";
        public static readonly string Marco_3 = "Março";
        public static readonly string Abril_4 = "Abril";
        public static readonly string Maio_5 = "Maio";
        public static readonly string Junho_6 = "Junho";
        public static readonly string Julho_7 = "Julho";
        public static readonly string Agosto_8 = "Agosto";
        public static readonly string Setembro_9 = "Setembro";
        public static readonly string Outubro_10 = "Outubro";
        public static readonly string Novembro_11 = "Novembro";
        public static readonly string Dezembro_12 = "Dezembro";

        public static string GetMonthNameBRByNumber(this int month)
        {
            return typeof(PT_BR_Month).GetFields().Where(fd => fd.Name.EndsWith("_" + month.ToString())).FirstOrDefault().GetValue(null).ToString();
        }
    }
}