

using System.Globalization;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsEven(this int i) => (i & 1) == 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(this int value) => (value & 1) != 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(this long value) => (value & 1L) != 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string Ordinal(this int value, CultureInfo culture)
        {
            return ((long)value).Ordinal(culture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Ordinal(this int value)
        {
            return value.Ordinal(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string Ordinal(this long value, CultureInfo culture)
        {
            var mod10 = value % 10;
            var mod100 = value % 100;

            string key = mod10 switch
            {
                1 when mod100 != 11 => LocalizationHelper.Ordinal_1,
                2 when mod100 != 12 => LocalizationHelper.Ordinal_2,
                3 when mod100 != 13 => LocalizationHelper.Ordinal_3,
                _ => LocalizationHelper.Ordinal_N
            };

            string format = LocalizationHelper.Get(key, culture);
            return string.Format(format, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Ordinal(this long value)
        {
            return value.Ordinal(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPrime(this int value)
        {
            if (value <= 1) return false;
            if (value <= 3) return true;
            if (value % 2 == 0 || value % 3 == 0) return false;

            int limit = (int)Math.Sqrt(value);
            for (int i = 5; i <= limit; i += 6)
            {
                if (value % i == 0 || value % (i + 2) == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPrime(this long value)
        {
            if (value <= 1) return false;
            if (value <= 3) return true;
            if (value % 2 == 0 || value % 3 == 0) return false;

            long limit = (long)Math.Sqrt(value);
            for (long i = 5; i <= limit; i += 6)
            {
                if (value % i == 0 || value % (i + 2) == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static decimal Clamp(this decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Clamp(this double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(this float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this double value)
        {
            // IEEE-safe check
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value) &&
                   Math.Abs(value % 1) < double.Epsilon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this float value)
        {
            return !float.IsNaN(value) &&
                   !float.IsInfinity(value) &&
                   Math.Abs(value % 1) < float.Epsilon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this decimal value)
        {
            return value == Math.Truncate(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static string ToPercentage(this double value, int decimals = 2)
        {
            return (value * 100).ToString($"F{decimals}") + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static string ToPercentage(this float value, int decimals = 2)
        {
            return (value * 100).ToString($"F{decimals}") + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static string ToPercentage(this decimal value, int decimals = 2)
        {
            return (value * 100).ToString($"F{decimals}") + "%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static string ToPercentage<T>(this T value, int decimals = 2) where T : struct, IConvertible
        {
            decimal d;

            try
            {
                d = Convert.ToDecimal(value);
            }
            catch
            {
                throw new NotSupportedException($"{nameof(ToPercentage)} is not supported for type {typeof(T)}");
            }

            return (d * 100).ToString($"F{decimals}") + "%";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="inclusive"></param>
        /// <returns></returns>
        public static bool Between<T>(this T value, T min, T max, bool inclusive = true) where T : IComparable<T>
        {
            if (inclusive)
            {
                return value.CompareTo(min) >= 0 &&
                       value.CompareTo(max) <= 0;
            }

            return value.CompareTo(min) > 0 &&
                   value.CompareTo(max) < 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(this long value) => (value & 1L) == 0;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static double PercentOf(this double value, double total) => total == 0 ? 0 : value / total * 100;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(this int value, int min, int max) => Math.Min(max, Math.Max(min, value));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this int value) => ((long)value).ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this long value) => value > 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this short value) => ((long)value).ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this ushort value) => ((ulong)value).ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this ulong value) => value > 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this int? value) => !value.HasValue ? null : value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this long? value) => !value.HasValue ? null : value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this ulong? value) => !value.HasValue ? null : value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this short? value) => !value.HasValue ? null : value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this ushort? value) => !value.HasValue ? null : ((ulong)value.Value).ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this int? value) => value.HasValue && value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this long? value) => value.HasValue && value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this ulong? value) => value.HasValue && value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this short? value) => value.HasValue && value.Value.ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this ushort? value) => value.HasValue && ((ulong)value).ToBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this long value) => value switch
        {
            0 => false,
            1 => true,
            _ => null,
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this ulong value) => value switch
        {
            0 => false,
            1 => true,
            _ => null,
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this int value) => ((long)value).ToNullableBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this short value) => ((long)value).ToNullableBoolean();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool? ToNullableBoolean(this ushort value) => ((ulong)value).ToNullableBoolean();
    }
}
