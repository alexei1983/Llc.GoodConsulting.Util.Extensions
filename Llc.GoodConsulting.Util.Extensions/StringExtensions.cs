using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool HasValue(this string? s) => !string.IsNullOrWhiteSpace(s);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string? SafeTrim(this string? s) => s?.Trim();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ValueOrDefault(this string? value, string? defaultValue = null) => !string.IsNullOrWhiteSpace(value) ? value : 
                                                                                                (!string.IsNullOrWhiteSpace(defaultValue) ? defaultValue : string.Empty);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string? s, T defaultValue = default) where T : struct => Enum.TryParse<T>(s.SafeTrim(), true, out var v) ? v : defaultValue;

        /// <summary>
        /// Pads the specified <see cref="string"/> on both the left and right with the specified character until it reaches the total width. 
        /// </summary>
        /// <remarks>Padding is centered. If the spacing is uneven, the right side gets the extra pad.</remarks>
        public static string Pad(this string? value, int totalWidth, char paddingChar = ' ')
        {
            value ??= string.Empty;

            if (value.Length >= totalWidth)
                return value;

            int spaces = totalWidth - value.Length;
            int left = spaces / 2;
            int right = spaces - left;

            return new string(paddingChar, left) + value + new string(paddingChar, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Left(this string? s, int count) => s?.Length >= count ? s[..count] : s ?? string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string? LeftNullable(this string? s, int count) => s?.Length >= count ? s[..count] : s;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Right(this string? s, int count)
        {
            if (string.IsNullOrEmpty(s) || count <= 0)
                return string.Empty;

            if (count >= s.Length)
                return s;

            return s.Substring(s.Length - count, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string? RightNullable(this string? s, int count)
        {
            if (s == null)
                return s;
            return s.Right(count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Mid(this string? s, int start, int length, bool zeroBasedIndex = true)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            int index = start;

            // Convert VB-style 1-based index to 0-based
            if (!zeroBasedIndex)
                index = index - 1;

            if (index < 0 || index >= s.Length || length <= 0)
                return string.Empty;

            if (index + length > s.Length)
                length = s.Length - index;

            return s.Substring(index, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string? MidNullable(this string? s, int start, int length, bool zeroBasedIndex = true)
        {
            if (s == null)
                return s;
            return s.Mid(start, length, zeroBasedIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string? Capitalize(this string? s)
            => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string? RemoveDiacritics(this string? s)
        {
            if (s == null)
                return s;

            var norm = s.Normalize(NormalizationForm.FormD);
            var chars = norm.Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
                                        != UnicodeCategory.NonSpacingMark);
            return new string(chars.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string? s, string value) => s?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Repeat(this string s, int count) => string.Concat(Enumerable.Repeat(s, count));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static string Slugify(this string? phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return string.Empty;

            // remove diacritics (accents)
            var normalized = phrase.RemoveDiacritics();

            if (string.IsNullOrWhiteSpace(normalized))
                return string.Empty;

            // lowercase
            normalized = normalized.ToLowerInvariant();

            // replace non-alphanumeric with hyphens
            normalized = NonAlphaNumericRegex().Replace(normalized, "-");

            // trim hyphens from both ends
            normalized = normalized.Trim('-');

            // collapse multiple hyphens
            return HyphenRegex().Replace(normalized, "-");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAscii(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            var span = value.AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] > 0x7F)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAsciiPrintable(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return true; // treat empty as ASCII printable

            var span = value.AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] < 32 || span[i] > 126)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInteger(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _) ||
                   ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? NullIfEmpty(this string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EmptyIfNull(this string? value)
        {
            return value ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitLines(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                yield break;

            using var reader = new StringReader(value);
            string? line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="spaces"></param>
        /// <returns></returns>
        public static string IndentLines(this string? value, int spaces)
        {
            if (string.IsNullOrEmpty(value) || spaces <= 0)
                return value ?? string.Empty;

            var indent = new string(' ', spaces);
            return string.Join(Environment.NewLine,
                value.SplitLines().Select(line => indent + line));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? ToBase64Utf8(this string? value)
        {
            if (value == null)
                return null;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ConstantTimeEquals(this string? a, string? b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32OrDefault(this string? value, int defaultValue = 0)
        {
            return int.TryParse(value, out var result)
                ? result
                : defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ToInt32(this string? value)
        {
            return int.TryParse(value, out var result)
                ? result
                : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToVisibleString(this string? value, bool escapeUnicode = false)
        {
            if (value == null)
                return "<null>";

            var sb = new StringBuilder(value.Length);

            foreach (char c in value)
            {
                switch (c)
                {
                    case '\0': sb.Append(@"\0"); break;   // Null
                    case '\a': sb.Append(@"\a"); break;   // Bell
                    case '\b': sb.Append(@"\b"); break;   // Backspace
                    case '\t': sb.Append(@"\t"); break;   // Tab
                    case '\n': sb.Append(@"\n"); break;   // Line feed
                    case '\v': sb.Append(@"\v"); break;   // Vertical tab
                    case '\f': sb.Append(@"\f"); break;   // Form feed
                    case '\r': sb.Append(@"\r"); break;   // Carriage return
                    case '\\': sb.Append(@"\\"); break;   // Backslash
                    default:
                        if (char.IsControl(c))
                        {
                            // Generic control character fallback (e.g., ASCII 1–31, 127)
                            sb.Append($"\\x{(int)c:X2}");
                        }
                        else
                        {
                            if (escapeUnicode && c > 127)
                                sb.Append($"\\u{(int)c:X4}");
                            else
                                sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NormalizeWhitespace(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var sb = new StringBuilder(value.Length);
            bool inWhitespace = false;

            foreach (char c in value)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!inWhitespace)
                        sb.Append(' ');
                    inWhitespace = true;
                }
                else
                {
                    sb.Append(c);
                    inWhitespace = false;
                }
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("-{2,}")]
        private static partial Regex HyphenRegex();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"[^a-z0-9]+")]
        private static partial Regex NonAlphaNumericRegex();
    }
}
