
namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T ThrowIfNull<T>(this T? obj, string name)
        {
            if (obj == null) 
                throw new ArgumentNullException(name);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static T IfNullThen<T>(this T? obj, T fallback) => obj == null ? fallback : obj;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static T? Coalesce<T>(this IEnumerable<T?>? objs)
        {
            objs ??= [];
            return objs.FirstOrDefault(o => o is not null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T? value, params T?[] list) => list.Contains(value);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T? value, params T?[] list) => !list.Contains(value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object? obj) => obj is null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IEnumerable<T> OfType<T>(this IEnumerable<object?>? objs, bool strict = true)
        {
            objs ??= [];
            return objs.Where(o => o != null)
                       .Cast<object>()
                       .Where(o => o.GetType() == typeof(T) || (!strict && o.GetType().IsAssignableTo(typeof(T))))
                       .Cast<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="type"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IEnumerable<object> OfType(this IEnumerable<object?>? objs, Type type, bool strict = true)
        {
            objs ??= [];
            return objs.Where(o => o != null)
                       .Cast<object>()
                       .Where(o => o.GetType() == type || (!strict && o.GetType().IsAssignableTo(type)));
        }
    }
}
