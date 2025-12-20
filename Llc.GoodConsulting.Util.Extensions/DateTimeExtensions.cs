
using System.Data.SqlTypes;
using System.Globalization;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// Represents a specific week within a month.
    /// </summary>
    public enum Week
    {
        /// <summary>
        /// First week of the month.
        /// </summary>
        First,

        /// <summary>
        /// Second week of the month.
        /// </summary>
        Second,

        /// <summary>
        /// Third week of the month.
        /// </summary>
        Third,

        /// <summary>
        /// Fourth week of the month.
        /// </summary>
        Fourth,

        /// <summary>
        /// Last week of the month.
        /// </summary>
        Last
    }

    /// <summary>
    /// Extension methods for working with <see cref="DateTime"/> and <see cref="DateTimeOffset"/> objects.
    /// </summary>
    public static class DateTimeExtensions
    {
        readonly static DateTime MinSqlDateTime = SqlDateTime.MinValue.Value;
        readonly static DateTime MaxSqlDateTime = SqlDateTime.MaxValue.Value;

        /// <summary>
        /// Adds the specified number of calendar quarters to the <see cref="DateTime"/>.
        /// </summary>
        public static DateTime AddQuarters(this DateTime originalDate, int quarters)
        {
            return originalDate.AddMonths(quarters * 3);
        }

        /// <summary>
        /// Calculates the calendar quarter to which the specified <see cref="DateTime"/> belongs.
        /// </summary>
        public static int GetQuarter(this DateTime fromDate)
        {
            int month = fromDate.Month - 1;
            int month2 = Math.Abs(month / 3) + 1;
            return month2;
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the first day of the calendar quarter 
        /// for the specified <see cref="DateTime"/>.
        /// </summary>
        public static DateTime StartOfQuarter(this DateTime originalDate)
        {
            return AddQuarters(new DateTime(originalDate.Year, 1, 1), GetQuarter(originalDate) - 1);
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the last day of the calendar quarter 
        /// for the specified <see cref="DateTime"/>.
        /// </summary>
        public static DateTime EndOfQuarter(this DateTime originalDate)
        {
            return AddQuarters(new DateTime(originalDate.Year, 1, 1), GetQuarter(originalDate)).AddDays(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current">The current date</param>
        /// <param name="week">The week for the next date to get.</param>
        public static DateTime ToWeek(this DateTime current, Week week)
        {
            return week switch
            {
                Week.Second => current.StartOfMonth().AddDays(7),
                Week.Third => current.StartOfMonth().AddDays(14),
                Week.Fourth => current.StartOfMonth().AddDays(21),
                Week.Last => current.EndOfMonth().AddDays(-7),
                _ => current.StartOfMonth(),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsUnixEpoch(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return false;
            return dateTime.Value.IsUnixEpoch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsUnixEpoch(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                dateTime = dateTime.ToUniversalTime();
            return dateTime.Equals(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsUnixEpoch(this DateTimeOffset? dateTime)
        {
            if (!dateTime.HasValue)
                return false;

            return dateTime.Value.UtcDateTime.IsUnixEpoch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsUnixEpoch(this DateTimeOffset dateTime)
        {
            return dateTime.UtcDateTime.IsUnixEpoch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsValidSqlDate(this DateTime dateTime)
        {
            return dateTime >= MinSqlDateTime &&
                   dateTime <= MaxSqlDateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsValidSqlDate(this DateTime? dateTime) => !dateTime.HasValue || (dateTime.HasValue && dateTime.Value.IsValidSqlDate());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsValidSqlDate(this DateTimeOffset dateTime) => dateTime.UtcDateTime.IsValidSqlDate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsValidSqlDate(this DateTimeOffset? dateTime) => !dateTime.HasValue || (dateTime.HasValue && dateTime.Value.UtcDateTime.IsValidSqlDate());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime dt) => dt.Date;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTimeOffset StartOfDay(this DateTimeOffset dt) => dt.Date;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime StartOfMonth(this DateTime dt) => new(dt.Year, dt.Month, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTimeOffset StartOfMonth(this DateTimeOffset dt) => new(dt.Year, dt.Month, 1, 0, 0, 0, dt.Offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfMonth(this DateTime dt) => dt.StartOfMonth().AddMonths(1).AddTicks(-1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTimeOffset EndOfMonth(this DateTimeOffset dt) => dt.StartOfMonth().AddMonths(1).AddTicks(-1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime dt) => dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTimeOffset dt) => dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static int AgeInYears(this DateTime dob, DateTime? referenceDate = null)
        {
            var today = referenceDate?.Date ?? DateTime.Today;

            int age = today.Year - dob.Year;

            // has not had a birthday yet this year?
            if (today < dob.AddYears(age))
                age--;
            return age < 0 ? 0 : age;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static int AgeInYears(this DateTime? dob, DateTime? referenceDate = null)
        {
            if (!dob.HasValue)
                return 0;

            return dob.Value.AgeInYears(referenceDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static int AgeInYears(this DateTimeOffset dob, DateTimeOffset? referenceDate = null)
        {
            return dob.DateTime.AgeInYears(referenceDate?.DateTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static int AgeInYears(this DateTimeOffset? dob, DateTimeOffset? referenceDate = null)
        {
            return dob.HasValue ? dob.Value.DateTime.AgeInYears(referenceDate?.DateTime) : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string TimeAgo(this DateTime dateTime, CultureInfo culture, DateTime? reference = null)
        {
            var now = reference ?? DateTime.UtcNow;

            if (now.Kind == DateTimeKind.Local)
                now = now.ToUniversalTime();

            if (dateTime.Kind == DateTimeKind.Local)
                dateTime = dateTime.ToUniversalTime();

            var diff = now - dateTime;

            // seconds
            if (diff.TotalSeconds < 5)
                return LocalizationHelper.Get(LocalizationHelper.JustNow, culture);

            if (diff.TotalSeconds < 60)
                return LocalizationHelper.Get(LocalizationHelper.SecondsAgo, culture, (int)diff.TotalSeconds);

            // minutes
            if (diff.TotalMinutes < 60)
                return LocalizationHelper.Get(LocalizationHelper.MinutesAgo, culture, (int)diff.TotalMinutes);

            // hours
            if (diff.TotalHours < 24)
                return LocalizationHelper.Get(LocalizationHelper.HoursAgo, culture, (int)diff.TotalHours);

            // days
            if (diff.TotalDays < 2)
                return LocalizationHelper.Get(LocalizationHelper.Yesterday, culture);

            if (diff.TotalDays < 30)
                return LocalizationHelper.Get(LocalizationHelper.DaysAgo, culture, (int)diff.TotalDays);

            // months (calendar-accurate)
            int months = MonthsBetween(dateTime, now);
            if (months < 12)
                return LocalizationHelper.Get(LocalizationHelper.MonthsAgo, culture, months);

            // years
            int years = YearsBetween(dateTime, now);
            return LocalizationHelper.Get(LocalizationHelper.YearsAgo, culture, years);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string TimeAgo(this DateTime dateTime)
        {
            return dateTime.TimeAgo(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string TimeUtil(this DateTime dateTime)
        {
            return dateTime.TimeUntil(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string TimeAgo(this DateTimeOffset dateTime)
        {
            return dateTime.TimeAgo(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string TimeUtil(this DateTimeOffset dateTime)
        {
            return dateTime.TimeUntil(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string TimeAgo(this DateTimeOffset dateTime, CultureInfo culture)
        {
            return dateTime.UtcDateTime.TimeAgo(culture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string TimeUntil(this DateTimeOffset dateTime, CultureInfo culture)
        {
            return dateTime.UtcDateTime.TimeUntil(culture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="future"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string TimeUntil(this DateTime future, CultureInfo culture, DateTime? reference = null)
        {
            var now = reference ?? DateTime.UtcNow;

            if (now.Kind != DateTimeKind.Utc)
                now = now.ToUniversalTime();

            if (future.Kind != DateTimeKind.Utc)
                future = future.ToUniversalTime();

            if (future <= now)
                return LocalizationHelper.Get(LocalizationHelper.JustNow, culture);

            var diff = future - now;

            // seconds
            if (diff.TotalSeconds < 5)
                return LocalizationHelper.Get(LocalizationHelper.InAFewSeconds, culture);

            if (diff.TotalSeconds < 60)
                return LocalizationHelper.Get(LocalizationHelper.InSeconds, culture, (int)diff.TotalSeconds);

            // minutes
            if (diff.TotalMinutes < 60)
                return LocalizationHelper.Get(LocalizationHelper.InMinutes, culture, (int)diff.TotalMinutes);

            // hours
            if (diff.TotalHours < 24)
                return LocalizationHelper.Get(LocalizationHelper.InHours, culture, (int)diff.TotalHours);

            // days
            if (diff.TotalDays < 2)
                return LocalizationHelper.Get(LocalizationHelper.Tomorrow, culture);

            if (diff.TotalDays < 30)
                return LocalizationHelper.Get(LocalizationHelper.InDays, culture, (int)diff.TotalDays);

            // months (calendar-accurate)
            int months = MonthsBetween(now, future);
            if (months < 12)
                return LocalizationHelper.Get(LocalizationHelper.InMonths, culture, months);

            // years
            int years = YearsBetween(now, future);
            return LocalizationHelper.Get(LocalizationHelper.InYears, culture, years);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        static int MonthsBetween(DateTime start, DateTime end)
        {
            int months = (end.Year - start.Year) * 12 + end.Month - start.Month;

            // if the end day is less than the start day, subtract one month
            if (end.Day < start.Day)
                months--;
            return months < 0 ? 0 : months;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        static int YearsBetween(DateTime start, DateTime end)
        {
            int years = end.Year - start.Year;

            // if the end month/day hasn't occurred yet this year, subtract one year
            if (end.Month < start.Month ||
               (end.Month == start.Month && end.Day < start.Day))
                years--;
            return years < 0 ? 0 : years;
        }
    }
}
