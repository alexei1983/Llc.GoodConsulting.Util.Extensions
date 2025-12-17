using System.Text.Json;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan TimeIt(this Action action)
        {
            ArgumentNullException.ThrowIfNull(action, nameof(action));
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// Times a synchronous function and returns (result, elapsed time).
        /// </summary>
        public static (T Result, TimeSpan Elapsed) TimeIt<T>(this Func<T> func)
        {
            ArgumentNullException.ThrowIfNull(func, nameof(func));
            var sw = Stopwatch.StartNew();
            T result = func();
            sw.Stop();
            return (result, sw.Elapsed);
        }

        /// <summary>
        /// Times an asynchronous operation and returns the elapsed TimeSpan.
        /// </summary>
        public static async Task<TimeSpan> TimeItAsync(this Func<Task> func)
        {
            ArgumentNullException.ThrowIfNull(func, nameof(func));
            var sw = Stopwatch.StartNew();
            await func();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// Times an async function and returns (result, elapsed time).
        /// </summary>
        public static async Task<(T Result, TimeSpan Elapsed)> TimeItAsync<T>(this Func<Task<T>> func)
        {
            ArgumentNullException.ThrowIfNull(func, nameof(func));
            var sw = Stopwatch.StartNew();
            T result = await func();
            sw.Stop();
            return (result, sw.Elapsed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        public static void Dump<T>(this T? obj, JsonSerializerOptions options) => Console.WriteLine(JsonSerializer.Serialize(obj, options ?? Common.DefaultJsonOptions));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void Dump<T>(this T? obj) => obj.Dump(Common.DefaultJsonOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <param name="options"></param>
        public static void Dump<T>(this IEnumerable<T?>? objs, JsonSerializerOptions options) => Console.WriteLine(JsonSerializer.Serialize(objs ?? [], options ?? Common.DefaultJsonOptions));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public static void Dump<T>(this IEnumerable<T?>? objs) => objs.Dump(Common.DefaultJsonOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? ToEnumString<TEnum>(this TEnum? value,
                                                  char separator = ' ',
                                                  bool separateConsecutiveUpperChars = true,
                                                  bool separateConsecutiveDigits = false) where TEnum : struct, Enum
        {
            if (!value.HasValue)
                return null;
            return ToEnumString(value.Value, separator, separateConsecutiveUpperChars, separateConsecutiveDigits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToEnumString<TEnum>(this TEnum value,
                                                 char separator = ' ',
                                                 bool separateConsecutiveUpperChars = true,
                                                 bool separateConsecutiveDigits = false) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);

            if (!enumType.IsEnum)
                throw new ArgumentException($"{enumType.FullName} is not an enum type.", nameof(value));

            var valueStr = value.ToString();

            if (string.IsNullOrEmpty(valueStr))
                return string.Empty;

            bool? hasFlags = null;

            if (_flagsCache.TryGetValue(enumType, out var flags))
                hasFlags = flags;

            if (!hasFlags.HasValue)
            {
                hasFlags = enumType.GetCustomAttribute<FlagsAttribute>() != null;
                if (hasFlags.HasValue)
                    _flagsCache[enumType] = hasFlags.Value;
            }

            if (hasFlags.Value && valueStr.Contains(','))
            {
                var parts = valueStr.Split([','], StringSplitOptions.RemoveEmptyEntries);
                var sb = new StringBuilder();

                for (int i = 0; i < parts.Length; i++)
                {
                    var partName = parts[i].Trim();
                    var formatted = GetSingleEnumString(enumType, partName, separator, separateConsecutiveUpperChars, separateConsecutiveDigits);
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(formatted);
                }
                return sb.ToString();
            }

            // single enum value
            return GetSingleEnumString(enumType, valueStr, separator, separateConsecutiveUpperChars, separateConsecutiveDigits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="separator"></param>
        /// <param name="separateConsecutiveUpperChars"></param>
        /// <param name="separateConsecutiveDigits"></param>
        /// <returns></returns>
        static string FormatEnumName(string name, char separator, bool separateConsecutiveUpperChars, bool separateConsecutiveDigits)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            /***
             * The method below of trying to determine where to place whitespace is problematic whether or not the 
             * user wants to separate consecutive uppercase characters and/or digits.
             * 
             * For example:
             * 
             * Separating consecutive uppercase characters yields the following results on these 
             * hypothetical enum values:
             * 
             * Country.USA =>  U S A
             * BloodType.ABNegative => A B Negative
             * ThingSize.ABigThing => A Big Thing
             * 
             * NOT separating consecutive uppercase characters yields the following results on those same hypothetical 
             * enum values:
             * 
             * Country.USA => USA
             * BloodType.ABNegative => ABNegative
             * ThingSize.ABigThing => ABig Thing
             * 
             * Even using various alternate cases is problematic:
             * 
             * Country.Usa => Usa
             * BloodType.AbNegative => Ab Negative
             * ThingSize.aBigThing => a Big Thing
             * 
             * Country.usa => usa
             * BloodType.abNegative => ab Negative
             * 
             * This is a very strong argument in favor of using System.ComponentModel.DescriptionAttribute for anything 
             * but the most standard/vanilla enum value definition!
             ***/

            var sb = new StringBuilder(name.Length * 2);
            sb.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                char c = name[i];
                if ((char.IsUpper(c) && (separateConsecutiveUpperChars || !char.IsUpper(name[i - 1])))
                 || (char.IsDigit(c) && (separateConsecutiveDigits || !char.IsDigit(name[i - 1]))))
                {
                    sb.Append(separator);
                }
                sb.Append(c);
            }

            return sb.ToString().TrimStart(separator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="valueStr"></param>
        /// <param name="separator"></param>
        /// <param name="separateConsecutiveUpperChars"></param>
        /// <param name="separateConsecutiveDigits"></param>
        /// <returns></returns>
        static string GetSingleEnumString(Type enumType, string valueStr, char separator, bool separateConsecutiveUpperChars, bool separateConsecutiveDigits)
        {
            var key = (enumType, valueStr);

            var hasCachedNoDescription = false;

            // return cached value if already looked up
            if (_descriptionCache.TryGetValue(key, out var cached))
            {
                hasCachedNoDescription = NoDescriptionAttribute.Equals(cached);

                if (!hasCachedNoDescription)
                    return cached;
            }

            if (!hasCachedNoDescription)
            {
                // does the item have a description attribute defined and we haven't checked yet?
                var enumField = enumType.GetField(valueStr);

                if (enumField != null)
                {
                    var attribute = enumField.GetCustomAttribute<DescriptionAttribute>();
                    if (!string.IsNullOrEmpty(attribute?.Description))
                    {
                        // cache it
                        _descriptionCache[key] = attribute.Description;
                        return attribute.Description;
                    }
                }

                // mark the item as not having a description attribute
                _descriptionCache[key] = NoDescriptionAttribute;
            }

            return FormatEnumName(valueStr, separator, separateConsecutiveUpperChars, separateConsecutiveDigits);
        }

        static readonly ConcurrentDictionary<(Type, string), string> _descriptionCache = new();
        static readonly ConcurrentDictionary<Type, bool> _flagsCache = new();
        const string NoDescriptionAttribute = "[_NO_DESC_]";
    }
}
