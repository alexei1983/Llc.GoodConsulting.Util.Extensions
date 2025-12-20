using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// Localization helper.
    /// </summary>
    internal static class LocalizationHelper
    {
        const string Resources = nameof(Resources);
        const string YesNo = nameof(YesNo);
        const string TimeStrings = nameof(TimeStrings);
        const string OrdinalStrings = nameof(OrdinalStrings);
        const string NamespacePrefix = $"{nameof(Llc)}.{nameof(GoodConsulting)}.{nameof(Util)}.{nameof(Extensions)}.{Resources}";
        static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

        public const string Yes = nameof(Yes);
        public const string No = nameof(No);
        public const string Ordinal_1 = nameof(Ordinal_1);
        public const string Ordinal_2 = nameof(Ordinal_2);
        public const string Ordinal_3 = nameof(Ordinal_3);
        public const string Ordinal_N = nameof(Ordinal_N);
        public const string SecondsAgo = nameof(SecondsAgo);
        public const string YearsAgo = nameof(YearsAgo);
        public const string MinutesAgo = nameof(MinutesAgo);
        public const string DaysAgo = nameof(DaysAgo);
        public const string HoursAgo = nameof(HoursAgo);
        public const string JustNow = nameof(JustNow);
        public const string InAFewSeconds = nameof(InAFewSeconds);
        public const string MonthsAgo = nameof(MonthsAgo);
        public const string InMinutes = nameof(InMinutes);
        public const string InSeconds = nameof(InSeconds);
        public const string InHours = nameof(InHours);
        public const string InDays = nameof(InDays);
        public const string InMonths = nameof(InMonths);
        public const string InYears = nameof(InYears);
        public const string Tomorrow = nameof(Tomorrow);
        public const string Yesterday = nameof(Yesterday);

        static readonly ResourceManager _yesNoRm = new($"{NamespacePrefix}.{YesNo}", ExecutingAssembly);
        static readonly ResourceManager _ordinalRm = new($"{NamespacePrefix}.{OrdinalStrings}", ExecutingAssembly);
        static readonly ResourceManager _timeRm = new($"{NamespacePrefix}.{TimeStrings}", ExecutingAssembly);

        static readonly ConcurrentDictionary<string, string> _cache = new();

        /// <summary>
        /// Retrieves a localized string by key using the given culture.
        /// </summary>
        /// <remarks>Falls back to English ("en") if the specified culture is missing.</remarks>
        public static string Get(string key, CultureInfo? culture)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            culture ??= CultureInfo.CurrentUICulture;

            var cacheKey = $"{culture.Name}:{key}";
            if (_cache.TryGetValue(cacheKey, out var cached))
                return cached;

            var _rm = GetRm(key);

            // try the fully qualified culture (e.g., fr-FR)
            var result = _rm.GetString(key, culture);

            if (result == null)
            {
                // try the two-letter ISO culture (e.g., "fr")
                var neutral = CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName);
                result = _rm.GetString(key, neutral);
            }

            if (result == null)
            {
                // fallback: en (English)
                var english = CultureInfo.GetCultureInfo("en");
                result = _rm.GetString(key, english);
            }

            if (result == null)
                throw new MissingManifestResourceException(
                    $"Resource key '{key}' not found for culture '{culture}'. Ensure resource file is embedded.");

            _cache[cacheKey] = result;
            return result;
        }

        /// <summary>
        /// Gets a localized formatted string.
        /// </summary>
        public static string Get(string key, CultureInfo culture, params object[] args)
        {
            var pattern = Get(key, culture);
            return string.Format(culture, pattern, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static ResourceManager GetRm(string key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            return key switch
            {
                Yes or No => _yesNoRm,
                Ordinal_1 or Ordinal_2 or Ordinal_3 or Ordinal_N => _ordinalRm,
                InAFewSeconds or JustNow or Yesterday or Tomorrow or SecondsAgo or MinutesAgo or HoursAgo or DaysAgo or MonthsAgo or YearsAgo or 
                InSeconds or InMinutes or InHours or InDays or InMonths or InYears => _timeRm,
                _ => throw new ArgumentException($"Cannot retrieve localization resource manager: invalid resource key ({key})", nameof(key)),
            };
        }
    }
}
