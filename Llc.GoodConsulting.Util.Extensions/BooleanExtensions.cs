using System.Globalization;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// Extension methods for working with <see cref="bool"/> values.
    /// </summary>
    public static class BooleanExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Toggle(this ref bool value) => value = !value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToYesNoString(this bool value, CultureInfo culture)
        {
            return LocalizationHelper.Get(value ? LocalizationHelper.Yes : LocalizationHelper.No, culture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToYesNoString(this bool? value, CultureInfo culture)
        {
            if (!value.HasValue)
                return string.Empty;
            return value.Value.ToYesNoString(culture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="whenTrue"></param>
        /// <param name="whenFalse"></param>
        /// <returns></returns>
        public static T Then<T>(this bool condition, T whenTrue, T whenFalse) => condition ? whenTrue : whenFalse;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool And(this bool a, bool b) => a && b;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Or(this bool a, bool b) => a || b;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool WhenTrue(this bool condition, Action action)
        {
            if (condition)
                action();
            return condition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool WhenFalse(this bool condition, Action action)
        {
            if (!condition)
                action();
            return condition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <example>config.If(debugEnabled, c => c.VerboseLogging = true).If(isProd, c => c.UseCaching = true);</example>
        public static T If<T>(this T obj, bool condition, Action<T> action)
        {
            if (condition)
                action(obj);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="onTrue"></param>
        /// <param name="onFalse"></param>
        /// <returns></returns>
        public static TResult Match<TResult>(this bool value, Func<TResult> onTrue, Func<TResult> onFalse)
        {
            return value ? onTrue() : onFalse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="ex"></param>
        public static void ThrowIfTrue(this bool condition, Exception ex)
        {
            if (condition)
                throw ex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="ex"></param>
        public static void ThrowIfFalse(this bool condition, Exception ex)
        {
            if (!condition)
                throw ex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T?> WhereIf<T>(this IEnumerable<T?>? source, bool condition, Func<T?, bool> predicate)
        {
            source ??= [];
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToYesNoString(this bool? value)
        {
            return value.ToYesNoString(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToYesNoString(this bool value)
        {
            return value.ToYesNoString(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt32(this bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToInt32String(this bool value)
        {
            return value.ToInt32().ToString(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ToNullableInt32(this bool? value)
        {
            if (!value.HasValue)
                return null;
            return value.Value.ToInt32();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToInt32String(this bool? value)
        {
            if (!value.HasValue)
                return string.Empty;
            return value.Value.ToInt32String();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //static readonly Dictionary<string, (string Yes, string No)> YesNoMap = new()
        //{
        //    // --- Europe ---
        //    { "en", ("Yes", "No") },                   // English — United Kingdom, United States, Canada, Australia, New Zealand, Belize, worldwide
        //    { "es", ("Sí", "No") },                    // Spanish — Spain, Mexico, Central America, South America
        //    { "fr", ("Oui", "Non") },                  // French — France, Canada, West Africa, Carribean
        //    { "de", ("Ja", "Nein") },                  // German — Germany, Austria, Switzerland
        //    { "it", ("Sì", "No") },                    // Italian — Italy, Switzerland
        //    { "pt", ("Sim", "Não") },                  // Portuguese — Portugal, Brazil
        //    { "ru", ("Да", "Нет") },                   // Russian — Russia, Belarus, Kazakhstan, Eastern Europe
        //    { "pl", ("Tak", "Nie") },                  // Polish — Poland
        //    { "nl", ("Ja", "Nee") },                   // Dutch — Netherlands, Belgium (Flanders), Suriname
        //    { "sv", ("Ja", "Nej") },                   // Swedish — Sweden, Finland
        //    { "fi", ("Kyllä", "Ei") },                 // Finnish — Finland
        //    { "da", ("Ja", "Nej") },                   // Danish — Denmark, Faroe Islands, Greenland
        //    { "no", ("Ja", "Nei") },                   // Norwegian - Norway
        //    { "nb", ("Ja", "Nei") },                   // Norwegian Bokmål — Norway
        //    { "nn", ("Ja", "Nei") },                   // Norwegian Nynorsk — Norway
        //    { "is", ("Já", "Nei") },                   // Icelandic — Iceland
        //    { "cs", ("Ano", "Ne") },                   // Czech — Czechia
        //    { "sk", ("Áno", "Nie") },                  // Slovak — Slovakia
        //    { "uk", ("Так", "Ні") },                   // Ukrainian — Ukraine
        //    { "be", ("Так", "Не") },                   // Belarusian — Belarus
        //    { "bg", ("Да", "Не") },                    // Bulgarian — Bulgaria
        //    { "sr", ("Да", "Не") },                    // Serbian — Serbia, Bosnia
        //    { "hr", ("Da", "Ne") },                    // Croatian — Croatia
        //    { "sl", ("Da", "Ne") },                    // Slovenian — Slovenia
        //    { "bs", ("Da", "Ne") },                    // Bosnian — Bosnia
        //    { "ro", ("Da", "Nu") },                    // Romanian — Romania, Moldova
        //    { "hu", ("Igen", "Nem") },                 // Hungarian — Hungary
        //    { "el", ("Ναι", "Όχι") },                  // Greek — Greece, Cyprus
        //    { "sq", ("Po", "Jo") },                    // Albanian — Albania, Kosovo
        //    { "hy", ("Այո", "Ոչ") },                   // Armenian — Armenia
        //    { "ka", ("დიახ", "არა") },                 // Georgian — Georgia
        //    { "gl", ("Si", "Non") },                   // Galician — Spain (Galicia)
        //    { "br", ("Ya", "Nann") },                  // Breton — France (Brittany)
        //    { "mt", ("Iva", "Le") },                   // Maltese — Malta
        //    { "la", ("Ita", "Non") },                  // Latin — Vatican
        //    { "cy", ("Ie", "Na") },                    // Welsh - Wales, United Kingdom
        //    { "ga", ("Tá", "Níl") },                   // Irish Gaelic - Ireland
        //    { "eu", ("Bai", "Ez") },                   // Basque - Spain, Iberian Penninsula
        //    { "ca", ("Sí", "No") },                    // Catalan - Spain, Iberian Penninsula
        //    { "lt", ("Taip", "Ne") },                  // Lithuanian - Lithuania
        //    { "lv", ("Jā", "Nē") },                    // Latvian - Latvia
        //    { "et", ("Jah", "Ei" ) },                  // Estonian - Estonia
        //    { "mk", ( "Да", "Не" ) },                  // Macedonian - Macedonia
        //    { "cnr", ("Da", "Ne") },                   // Montenegrin — Montenegro
        //    { "fy",  ("Ja", "Nee") },                  // Frisian (West Frisian) — Netherlands (Friesland)
        //    { "fo", ("Ja", "Nei") },                   // Faroese — Faroe Islands
        //    { "oc", ("Òc", "Non") },                   // Occitan — Southern France, Monaco, Aran Valley

        //    // --- Middle East / Central Asia ---
        //    { "ar", ("نعم", "لا") },                    // Arabic — Middle East, North Africa
        //    { "he", ("כן", "לא") },                    // Hebrew — Israel
        //    { "fa", ("بله", "نه") },                   // Persian — Iran, Afghanistan (Dari)
        //    { "tr", ("Evet", "Hayır") },               // Turkish — Turkey
        //    { "az", ("Bəli", "Xeyr") },                // Azerbaijani — Azerbaijan
        //    { "ku", ("Erê", "Na") },                   // Kurdish — Turkey, Iraq, Syria
        //    { "ps", ("هو", "نه") },                    // Pashto — Afghanistan, Pakistan
        //    { "tg", ("Ҳa", "Не") },                    // Tajik — Tajikistan
        //    { "uz", ("Ha", "Yo'q") },                  // Uzbek — Uzbekistan
        //    { "kk", ("Иә", "Жоқ") },                   // Kazakh — Kazakhstan
        //    { "ky", ("Ооба", "Жок") },                 // Kyrgyz — Kyrgyzstan
        //    { "tk", ("Hawa", "Ýok") },                 // Turkmen — Turkmenistan
        
        //    // --- Indian Subcontinent ---
        //    { "hi", ("हाँ", "नहीं") },                 // Hindi — India
        //    { "bn", ("হ্যাঁ", "না") },                 // Bengali — Bangladesh, India
        //    { "pa", ("ਹਾਂ", "ਨਹੀਂ") },                // Punjabi — India, Pakistan
        //    { "gu", ("હા", "ના") },                 // Gujarati — India
        //    { "ml", ("അതെ", "അല്ല") },          // Malayalam — India (Kerala)
        //    { "ta", ("ஆம்", "இல்லை") },         // Tamil — India, Sri Lanka
        //    { "te", ("అవును", "కాదు") },           // Telugu — India
        //    { "mr", ("हो", "नाही") },               // Marathi — India
        //    { "ur", ("ہاں", "نہیں") },             // Urdu — Pakistan, India
        //    { "si", ("ඔව්", "නැහැ") },             // Sinhala — Sri Lanka
        //    { "ne", ("हो", "होइन") },               // Nepali — Nepal
        //    { "as", ("হʼয়", "নহয়") },             // Assamese — India
        //    { "or", ("ହଁ", "ନା") },                 // Odia — India
        //    { "kn", ("ಹೌದು", "ಇಲ್ಲ") },           // Kannada - India

        //    // --- East Asia ---
        //    { "zh", ("是", "否") },                    // Chinese — China, Taiwan, Singapore
        //    { "ja", ("はい", "いいえ") },               // Japanese — Japan
        //    { "ko", ("예", "아니요") },                // Korean — South Korea, North Korea
        //    { "mn", ("Тийм", "Үгүй") },               // Mongolian — Mongolia

        //    // --- Southeast Asia ---
        //    { "vi", ("Có", "Không") },                // Vietnamese — Vietnam
        //    { "id", ("Ya", "Tidak") },                // Indonesian — Indonesia
        //    { "jv", ("Inggih", "Mboten") },           // Javanese — Indonesia (Java)
        //    { "ms", ("Ya", "Tidak") },                // Malay — Malaysia, Brunei
        //    { "th", ("ใช่", "ไม่ใช่") },                  // Thai — Thailand
        //    { "km", ("បាទ", "ទេ") },                  // Khmer — Cambodia
        //    { "lo", ("ແມ່ນ", "ບໍ່") },                  // Lao — Laos
        //    { "my", ("ဟုတ်", "မဟုတ်") },              // Burmese — Myanmar
        //    { "tl", ("Oo", "Hindi") },               // Tagalog - Phillipines
        //    { "fil", ("Oo", "Hindi") },              // Tagalog - Phillipines

        //    // --- Polynesia & Oceania ---
        //    { "haw", ("ʻAe", "ʻAʻole") },             // Hawaiian — Hawaii, United States
        //    { "mi", ("Āe", "Kāo") },                  // Māori — New Zealand
        //    { "sm", ("Ioe", "Leai") },                // Samoan — Samoa, American Samoa
        //    { "fj", ("Io", "Sega") },                 // Fijian — Fiji
        //    { "mh", ("Aet", "Yokwe jab") },           // Marshallese — Marshall Islands

        //    // --- Africa (major languages) ---
        //    { "sw", ("Ndiyo", "Hapana") },            // Swahili — Kenya, Tanzania, Uganda
        //    { "zu", ("Yebo", "Cha") },                // Zulu — South Africa
        //    { "xh", ("Ewe", "Hayi") },                // Xhosa — South Africa
        //    { "so", ("Haa", "Maya") },                // Somali — Somalia
        //    { "am", ("አዎን", "አይ") },                 // Amharic — Ethiopia
        //    { "rw", ("Yego", "Oya") },                // Kinyarwanda — Rwanda
        //    { "mg", ("Eny", "Tsia") },                // Malagasy — Madagascar
        //    { "st", ("E", "Che") },                   // Sesotho — Lesotho, South Africa
        //    { "tn", ("Ee", "Nnyaya") },               // Setswana — Botswana, South Africa
        //    { "ts", ("Ina", "Awa") },                 // Tsonga — South Africa, Mozambique
        //    { "ve", ("Ee", "Hai") },                  // Venda — South Africa, Zimbabwe
        //    { "wo", ("Waaw", "Déedéet") },            // Wolof — Senegal
        //    { "ig", ("Ee", "Mba") },                  // Igbo — Nigeria
        //    { "ha", ("I", "A'a") },                   // Hausa — Nigeria, Niger
        //    { "yo", ("Bẹẹni", "Rara") },              // Yoruba — Nigeria
        //    { "af", ("Ja", "Nee") },                  // Afrikaans - South Africa

        //    // --- Indigenous / Arctic / Native American ---
        //    { "iu", ("ᐊᐃᓐᓇᓐᖏᑐᖅ", "ᐱᓕᖅᑯᓐᖏᑐᖅ") },  // Inuktitut — Canada (Nunavut), Arctic Region
        //    { "nv", ("Aooʼ", "Doó’") },             // Navajo — Navajo Nation, Southwestern United States
        //    { "cr",  ("ᒦᓇᐦ", "ᓇᓂᔭᐤ") },             // Cree — Alberta, Saskatchewan, Manitoba, Quebec, Canada
        //    { "hop", ("Hìi", "Qaa") },              // Hopi — Arizona, United States
        //    { "lkt", ("Hą́", "Hiyá") },              // Lakota — North Dakota, South Dakota, Sioux
        //    { "chr", ("ᎥᎥ", "ᎥᏝ") },                 // Cherokee — Oklahoma, North Carolina, United States
        //    { "ain", ("エエ", "アン") },            // Ainu — Japan (Hokkaido), indigenous Ainu people

        //    // --- Constructed / Auxiliary ---
        //    { "eo", ("Jes", "Ne") },                // Esperanto — International constructed language
        //    { "tok", ("pona", "ike") },             // Toki Pona — Constructed minimalist language - informal code is "tp" but "tok" is official
        //    { "tlh", ("HIjaʼ", "ghobeʼ") },         // Klingon — Constructed language (Star Trek)
        //    { "vo",  ("Si", "No") },                // Volapük — International constructed language
        //};
    }
}
