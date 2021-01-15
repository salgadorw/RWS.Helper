using System.Collections.Generic;

namespace RWS.Helper
{
    public static class GenericIEqualityComparer
    {
        public static IEqualityComparer<T> Comparer<T>(string propertyOrMethodName, object[] methodParameters)
        {
            return (IEqualityComparer<T>)(new PropertyComparer<T>(propertyOrMethodName, methodParameters));
        }
    }
}