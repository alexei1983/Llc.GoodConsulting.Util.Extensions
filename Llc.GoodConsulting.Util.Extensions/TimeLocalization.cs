using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Collections.Concurrent;

namespace Llc.GoodConsulting.Util.Extensions
{   
    /// <summary>
    /// 
    /// </summary>
    public static class TimeLocalization
    {
        const string Resources = nameof(Resources);
        const string TimeStrings = nameof(TimeStrings);

        static readonly ResourceManager _rm = new($"{nameof(Llc)}.{nameof(GoodConsulting)}.{nameof(Util)}.{nameof(Extensions)}.{Resources}.{TimeStrings}",
                                                  Assembly.GetExecutingAssembly());

        static readonly ConcurrentDictionary<string, string> _cache = new();

        /// <summary>
        /// Retrieves a localized string by key using the given culture.
        /// Falls back to English ("en") if the specified culture is missing.
        /// </summary>
        public static string Get(string key, CultureInfo? culture)
        {
            ArgumentNullException.ThrowIfNull(key);

            culture ??= CultureInfo.CurrentUICulture;

            var cacheKey = $"{culture.Name}:{key}";
            if (_cache.TryGetValue(cacheKey, out var cached))
                return cached;

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
    }
}
