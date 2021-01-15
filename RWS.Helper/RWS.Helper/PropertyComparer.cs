using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RWS.Helper
{
    public class PropertyComparer<T> : IEqualityComparer<T>
    {
        private PropertyInfo _PropertyInfo = null;
        private string _MethodInfo = null;

        private object[] _MethodParameters = null;

        /// <summary>
        /// Creates a new instance of PropertyComparer.
        /// </summary>
        /// <param name="propertyName">The name of the property on type T
        /// to perform the comparison on.</param>
        public PropertyComparer(string propertyOrMethodName, object[] methodParameters)
        {
            _MethodParameters = methodParameters;

            //store a reference to the property info object for use during the comparison
            _PropertyInfo = typeof(T).GetProperty(propertyOrMethodName,
            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (_PropertyInfo == null)
            {
                _MethodInfo = propertyOrMethodName;

                var methods = typeof(T).GetMethods().Where(m => m.Name.Equals(_MethodInfo));
                if (methods.Count() <= 0)
                {
                    throw new ArgumentException(string.Format("{0} is not a property or method of type {1}.", propertyOrMethodName, typeof(T)));
                }
            }
        }

        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            //get the current value of the comparison property of x and of y
            object xValue = null;
            object yValue = null;
            if (_MethodInfo == null)
            {
                xValue = _PropertyInfo.GetValue(x, null);
                yValue = _PropertyInfo.GetValue(y, null);

                //if the xValue is null then we consider them equal if and only if yValue is null
                if (xValue == null)
                    return yValue == null;
            }
            else
            {
                //             xValue = typeof(T).InvokeMember(_MethodInfo, BindingFlags.InvokeMethod,
                //Type.DefaultBinder, x, _MethodParameters);
                //             yValue = typeof(T).InvokeMember(_MethodInfo, BindingFlags.InvokeMethod,
                //Type.DefaultBinder, x, _MethodParameters);

                //if the xValue is null then we consider them equal if and only if yValue is null
                if (xValue == null)
                    return yValue == null;
            }

            //use the default comparer for whatever type the comparison property is.
            return xValue.Equals(yValue);
        }

        public bool Equals(T x, T[] y)
        {
            //get the current value of the comparison property of x and of y
            object xValue = _PropertyInfo.GetValue(x, null);

            //if the xValue is null then we consider them equal if and only if yValue is null

            //use the default comparer for whatever type the comparison property is.
            return y.Contains((T)xValue);
        }

        public int GetHashCode(T obj)
        {
            //get the value of the comparison property out of obj
            object propertyValue = null;
            if (_PropertyInfo != null)
            {
                propertyValue = _PropertyInfo.GetValue(obj, null);
            }
            else
            {
                //             propertyValue = typeof(T).InvokeMember(_MethodInfo, BindingFlags.InvokeMethod,
                //Type.DefaultBinder, obj, _MethodParameters);
            }

            if (propertyValue == null)
                return 0;
            else
                return propertyValue.GetHashCode();
        }

        #endregion IEqualityComparer<T> Members
    }
}