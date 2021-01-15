
namespace RWS.Helper
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static TypeCast As<TypeCast>(this object obj) where TypeCast : class
        {
            return obj as TypeCast;
        }

    }
}
