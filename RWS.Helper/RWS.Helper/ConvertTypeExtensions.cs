namespace RWS.Helper
{
    using System;
    using System.Linq;

    public static class ConvertTypeExtensions
    {
        public static TResult ToType<TResult>(this object obj, bool copyFields=true)
        {

            //create instance of T type object:
            var tmp = (TResult)Activator.CreateInstance(typeof(TResult));

            tmp.CopyPropertiesValueFrom(obj, copyFields);

            return tmp;
        }

        public static void CopyPropertiesValueFrom<Target, Source>(this Target target, Source source,  bool copyFields = false)
        {
            var sourceProperties = source.GetType().GetProperties().Select(s=> new { s.Name, Value = s.GetValue(source) });
            var targetProperties = target.GetType().GetProperties();

            targetProperties.ToList().ForEach(targetProperty =>
            {
                var foundOnSource = sourceProperties.FirstOrDefault(w => w.Name.Equals(targetProperty.Name));
                if (foundOnSource != null)
                    targetProperty.SetValue(target, foundOnSource.Value);
            });

            if (copyFields)
                target.CopyFieldsValueFrom(source);

        }

        public static void CopyFieldsValueFrom<Target, Source>(this Target target, Source source, bool copyProperties = false)
        {
            var sourceProperties = source.GetType().GetFields().Select(s => new { s.Name, Value = s.GetValue(source) });
            var targetProperties = target.GetType().GetFields();

            targetProperties.ToList().ForEach(targetProperty =>
            {
                var foundOnSource = sourceProperties.FirstOrDefault(w => w.Name.Equals(targetProperty.Name));
                if (foundOnSource != null)
                    targetProperty.SetValue(target, foundOnSource.Value);
            });

            if (copyProperties)
                target.CopyPropertiesValueFrom(source);
        }
    }
   
}