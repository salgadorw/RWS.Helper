using System;

namespace RWS.Helper
{
    public class ToStringSmarter
    {
        private static ToStringSmarter _default;

        public static ToStringSmarter GetDefault
        {
            get
            {
                if (_default == null)
                    _default = new ToStringSmarter();
                return _default;
            }
            set { _default = value; }
        }

        public Func<DateTime, string> DateTimeF { get; set; }

        public Func<Decimal, string> DecimalF { get; set; }

        public string ToStrSmarter(object value)
        {
            if (value == null)
                return "";

            var retValue = value.ToString();

            if (value is DateTime)
            {
                if (this.DateTimeF != null)
                    retValue = this.DateTimeF((DateTime)value);
                else
                    retValue = value.ToStringDateOrNullDate();
            }
            else if (value is DateTime?)
            {
                if (this.DateTimeF != null)
                    retValue = this.DateTimeF(((DateTime?)value).Value);
                else
                    retValue = value.ToStringDateOrNullDate();
            }
            else if (value is decimal)
            {
                if (this.DecimalF != null)
                    retValue = this.DecimalF((decimal)value);
            }
            else if (value is decimal?)
            {
                if (this.DecimalF != null)
                    retValue = this.DecimalF(((decimal?)value).Value);
            }
            return retValue;
        }
    }
}