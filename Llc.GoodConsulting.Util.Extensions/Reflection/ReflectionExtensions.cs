using System.Reflection;

namespace Llc.GoodConsulting.Util.Extensions.Reflection
{
    /// <summary>
    /// Extension methods to utilize with reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this MemberInfo member, bool inherit = true) where T : Attribute
        {
            return member.GetCustomAttributes(typeof(T), inherit).Length > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit = true) where T : Attribute
        {
            return (T[])member.GetCustomAttributes(typeof(T), inherit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public |
                                      BindingFlags.NonPublic |
                                      BindingFlags.Instance |
                                      BindingFlags.Static |
                                      BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            return type.GetFields(BindingFlags.Public |
                                  BindingFlags.NonPublic |
                                  BindingFlags.Instance |
                                  BindingFlags.Static |
                                  BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object? GetPropertyValue(this object? obj, string propertyName)
        {
            if (obj == null)
                return null;

            var prop = obj.GetType().GetProperty(propertyName);
            return prop?.GetValue(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPropertyValue(this object? obj, string propertyName, object? value)
        {
            if (obj == null)
                return false;

            var prop = obj.GetType().GetProperty(propertyName);

            if (prop == null || !prop.CanWrite) 
                return false;

            if (value == null)
            {
                prop.SetValue(obj, null);
                return true;
            }

            try
            {
                var converted = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(obj, converted);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object? GetFieldValue(this object? obj, string fieldName)
        {
            if (obj == null)
                return null;

            var fld = obj.GetType().GetField(fieldName);
            return fld?.GetValue(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetFieldValue(this object? obj, string fieldName, object? value)
        {
            if (obj == null)
                return false;

            var fld = obj.GetType().GetField(fieldName);

            if (fld == null)
                return false;

            if (value == null)
            {
                fld.SetValue(obj, null);
                return true;
            }

            try
            {
                var converted = Convert.ChangeType(value, fld.FieldType);
                fld.SetValue(obj, converted);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyMatchingPropertiesTo<TSource, TDestination>(this TSource? source, 
                                                                           TDestination? destination,
                                                                           bool inherit = false,
                                                                           StringComparison propertyNameComparison = default) where TDestination : class, new()
                                                                                                                              where TSource : class, new()
        {
            if (source == null || destination == null)
                return;

            var flags = GetBindingFlags(inherit, false, false);

            var destProps = typeof(TDestination).GetProperties(flags);

            foreach (var src in typeof(TSource).GetProperties(flags))
            {
                var dest = destProps.FirstOrDefault(dp => string.Equals(dp.Name, src.Name, propertyNameComparison));
                if (dest != null && dest.CanWrite && dest.PropertyType.IsAssignableFrom(src.PropertyType))
                    dest.SetValue(destination, src.GetValue(source));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="inherit"></param>
        /// <param name="propertyNameComparison"></param>
        /// <returns></returns>
        public static TDestination? CreateInstanceFromMatchingProperties<TSource, TDestination>(this TSource source,
                                                                                                bool inherit = false,
                                                                                                StringComparison propertyNameComparison = default) where TDestination : class, new()
                                                                                                                                                   where TSource : class, new()
        {
            if (source == null)
                return default;

            var dest = Activator.CreateInstance<TDestination>();
            source.CopyMatchingPropertiesTo(dest, inherit, propertyNameComparison);
            return dest;       
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="inherit"></param>
        /// <param name="propertyNameComparison"></param>
        public static void CopyMatchingPropertiesTo(this object? source, 
                                                    object? destination,
                                                    bool inherit = false,
                                                    StringComparison propertyNameComparison = default)
        {
            if (source == null || destination == null)
                return;

            var flags = GetBindingFlags(inherit, false, false);

            var destProps = destination.GetType().GetProperties(flags);

            foreach (var src in source.GetType().GetProperties(flags))
            {
                var dest = destProps.FirstOrDefault(dp => string.Equals(dp.Name, src.Name, propertyNameComparison));
                if (dest != null && dest.CanWrite && dest.PropertyType.IsAssignableFrom(src.PropertyType))
                    dest.SetValue(destination, src.GetValue(source));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool Implements<TInterface>(this Type type)
        {
            return typeof(TInterface).IsAssignableFrom(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool Implements(this Type type, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? GetDefault(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            type = type.GetNonNullableType();

            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Public |
                                   BindingFlags.NonPublic |
                                   BindingFlags.Instance |
                                   BindingFlags.Static |
                                   BindingFlags.FlattenHierarchy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return type.GetProperties().Where(p => p.HasAttribute<T>(inherit));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesWhere(this Assembly assembly, Func<Type, bool> predicate)
        {
            return assembly.GetTypes().Where(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertyInfo? GetPropertyIgnoreCase(this Type type, string name, bool inherit = false)
        {
            return type.GetProperties(GetBindingFlags(inherit, false, false))
                       .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inherit"></param>
        /// <param name="nonPublic"></param>
        /// <returns></returns>
        static BindingFlags GetBindingFlags(bool inherit, bool nonPublic, bool @static)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (inherit)
                flags |= BindingFlags.FlattenHierarchy;
            if (nonPublic)
                flags |= BindingFlags.NonPublic;
            if (@static)
                flags |= BindingFlags.Static;
            return flags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="type"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> OfType(this IEnumerable<FieldInfo> fields, Type type, bool strict = true)
        {
            return fields.Where(f => (f.FieldType == type) || (!strict && f.FieldType.IsAssignableFrom(type)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="type"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> OfType(this IEnumerable<PropertyInfo> props, Type type, bool strict = true)
        {
            return props.Where(p => (p.PropertyType == type) || (!strict && p.PropertyType.IsAssignableFrom(type)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="props"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> OfType<T>(this IEnumerable<PropertyInfo> props, bool strict = true)
        {
            return props.OfType(typeof(T), strict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> OfType<T>(this IEnumerable<FieldInfo> fields, bool strict = true)
        {
            return fields.OfType(typeof(T), strict);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ofType"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> Constants(this Type type, bool inherit = true, bool nonPublic = false)
        {
            var fieldInfo = type.GetFields(GetBindingFlags(inherit, nonPublic, true));

            foreach (var fi in fieldInfo)
            {
                if (fi.IsLiteral && !fi.IsInitOnly)
                    yield return fi;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object?>> ConstantsWithValues(this Type type,
                                                                                     bool inherit = true, bool nonPublic = false)
        {
            foreach (var constant in type.Constants(inherit, nonPublic))
                yield return new KeyValuePair<string, object?>(constant.Name, constant.GetRawConstantValue());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> Constants<T>(this T obj, bool inherit = true, bool nonPublic = false) => obj != null ? obj.GetType().Constants(inherit, nonPublic) : [];
    }
}
