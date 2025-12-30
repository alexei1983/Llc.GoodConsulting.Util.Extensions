using System.Globalization;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// Noun to use for localizing count strings.
    /// </summary>
    public enum CountNoun
    {
        /// <summary>
        /// item, elemento, élément, Element, элемент, פריט, 项目, 項目, 항목
        /// </summary>
        Item,

        /// <summary>
        /// result, resultado, résultat, Ergebnis, результат, תוצאה, 结果, 結果, 결과
        /// </summary>
        Result,

        /// <summary>
        /// file, archivo, fichier, arquivo, Datei, файл, קובץ, 文件, ファイル, 파일
        /// </summary>
        File,

        /// <summary>
        /// user, usuario, utilisateur, usuário, Benutzer, пользователь, משתמש, 用户, ユーザー, 사용자
        /// </summary>
        User,

        /// <summary>
        /// warning, advertencia, avertissement, aviso, Warnung, предупреждение, אזהרה, 警告, 警告, 경고
        /// </summary>
        Warning,

        /// <summary>
        /// error, erreur, erro, Fehler, ошибка, שגיאה, 错误, エラー, 오류
        /// </summary>
        Error,

        /// <summary>
        /// record, registro, enregistrement, Datensatz, запись, רשומה, 记录, レコード, 레코드
        /// </summary>
        Record,

        /// <summary>
        /// device, dispositivo, appareil, Gerät, устройство, התקן, 设备, デバイス, 장치
        /// </summary>
        Device
    }

    /// <summary>
    /// Gramatical gender.
    /// </summary>
    public enum GrammaticalGender
    {
        /// <summary>
        /// Masculine gender.
        /// </summary>
        Masculine,

        /// <summary>
        /// Feminine gender.
        /// </summary>
        Feminine,

        /// <summary>
        /// Neuter gender.
        /// </summary>
        Neuter
    }


    /// <summary>
    /// 
    /// </summary>
    public static class LocalizedCountExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="noun"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToLocalizedCount(this int count, CountNoun noun, CultureInfo culture)
        {
            culture ??= CultureInfo.InvariantCulture;
            var lang = culture.TwoLetterISOLanguageName;
            var nounKey = noun.ToString();

            // zero
            if (count == 0)
            {
                var hebrewZero = HebrewZeroPhraseHelper.GetZeroPhrase(noun, culture, CountNounResources.ResourceManager);
                if (!string.IsNullOrEmpty(hebrewZero))
                    return hebrewZero;

                var zero = CountNounResources.Get($"Zero.{nounKey}", culture);
                if (!string.IsNullOrEmpty(zero))
                    return zero;
            }

            // CJK — Chinese
            if (lang == "zh")
            {
                var classifier = CountNounResources.Get($"Classifier.{nounKey}.Zh", culture) ?? "个";
                var word = CountNounResources.Get($"Noun.{nounKey}.One", culture);
                return $"{count} {classifier}{word}";
            }

            // CJK — Japanese
            if (lang == "ja")
            {
                var classifier = CountNounResources.Get($"Classifier.{nounKey}.Ja", culture) ?? "つ";
                var word = CountNounResources.Get($"Noun.{nounKey}.One", culture);
                return $"{count}{classifier}{word}";
            }

            // CJK — Korean
            if (lang == "ko")
            {
                var classifier = CountNounResources.Get($"Classifier.{nounKey}.Ko", culture) ?? "개";
                var word = CountNounResources.Get($"Noun.{nounKey}.One", culture);
                return $"{count} {classifier}{word}";
            }

            // Russian plural rules
            if (lang == "ru")
            {
                return $"{count} {GetRussianForm(count, nounKey, culture)}";
            }

            // Default (Western)
            var form = count == 1 ? "One" : "Many";
            var value = CountNounResources.Get($"Noun.{nounKey}.{form}", culture)
                        ?? CountNounResources.Get($"Noun.{nounKey}.One", culture);

            return $"{count} {value}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="nounKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        static string? GetRussianForm(int count, string nounKey, CultureInfo culture)
        {
            int mod10 = count % 10;
            int mod100 = count % 100;

            if (mod10 == 1 && mod100 != 11)
                return CountNounResources.Get($"Noun.{nounKey}.One", culture);

            if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14))
                return CountNounResources.Get($"Noun.{nounKey}.Few", culture);

            return CountNounResources.Get($"Noun.{nounKey}.Many", culture);
        }
    }
}
