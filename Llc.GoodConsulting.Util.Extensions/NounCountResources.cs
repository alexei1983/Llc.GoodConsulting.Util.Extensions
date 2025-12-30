using System.Globalization;
using System.Resources;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class CountNounResources
    {
        static readonly ResourceManager _rm =
            new($"{LocalizationHelper.NamespacePrefix}.{LocalizationHelper.CountNouns}", LocalizationHelper.ExecutingAssembly);

        internal static ResourceManager ResourceManager => _rm;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string? Get(string key, CultureInfo culture)
            => _rm.GetString(key, culture);
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class HebrewNounGender
    {
        private static readonly Dictionary<CountNoun, GrammaticalGender> Map =
            new()
            {
            { CountNoun.Item,    GrammaticalGender.Masculine },
            { CountNoun.User,    GrammaticalGender.Masculine },
            { CountNoun.File,    GrammaticalGender.Masculine },
            { CountNoun.Device,  GrammaticalGender.Masculine },
            { CountNoun.Result,  GrammaticalGender.Feminine },
            { CountNoun.Error,   GrammaticalGender.Feminine },
            { CountNoun.Warning, GrammaticalGender.Feminine },
            { CountNoun.Record,  GrammaticalGender.Feminine }
            };

        public static GrammaticalGender GetGender(CountNoun noun)
            => Map.TryGetValue(noun, out var g)
                ? g
                : GrammaticalGender.Masculine; // safe default
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class HebrewZeroPhraseHelper
    {
        public static string? GetZeroPhrase(CountNoun noun, CultureInfo culture, ResourceManager resourceManager)
        {
            // only applies to Hebrew
            if (!culture.TwoLetterISOLanguageName.Equals("he", StringComparison.OrdinalIgnoreCase))
                return null;

            var gender = HebrewNounGender.GetGender(noun);
            var nounKey = noun.ToString();

            // preferred gender-specific key
            var genderKey = gender == GrammaticalGender.Masculine
                ? $"Zero.{nounKey}.Masc"
                : $"Zero.{nounKey}.Fem";

            var value = resourceManager.GetString(genderKey, culture);

            // fallback chain (very important for robustness)
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = resourceManager.GetString($"Zero.{nounKey}", culture);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            // last resort (should never hit)
            return "לא נמצאו פריטים";
        }
    }
}
