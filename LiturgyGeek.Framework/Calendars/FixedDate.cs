using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class FixedDate : ChurchDate
    {
        public int Month { get; private init; }

        public int Day { get; private init; }

        public DayOfWeek? DayOfWeek { get; private init; }

        public int? DaySpan { get; private init; }

        public FixedDate(int month, int day) : this(month, day, default(DayOfWeek?), default(int?))
        {
        }

        public FixedDate(int month, int day, DayOfWeek dayOfWeek) : this(month, day, dayOfWeek, default(int?))
        {
        }

        public FixedDate(int month, int day, DayOfWeek dayOfWeek, int daySpan) : this(month, day, (DayOfWeek?)dayOfWeek, (int?)daySpan)
        {
        }

        private FixedDate(int month, int day, DayOfWeek? dayOfWeek, int? daySpan)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));
            switch (month)
            {
                case 9:
                case 4:
                case 6:
                case 11:
                    if (day < 0 || day > 30)
                        throw new ArgumentOutOfRangeException(nameof(day));
                    break;

                case 2:
                    if (day < 0 || day > 29)
                        throw new ArgumentOutOfRangeException(nameof(day));
                    break;

                case 3:
                    if (day == -1)
                        break;
                    goto default;

                default:
                    if (day < 0 || day > 31)
                        throw new ArgumentOutOfRangeException(nameof(day));
                    break;
            }

            if (dayOfWeek.HasValue && !Enum.IsDefined(dayOfWeek.Value))
                throw new ArgumentException("Invalid value", nameof(dayOfWeek));

            if (daySpan < 1 || daySpan > 7)
                throw new ArgumentOutOfRangeException(nameof(daySpan), "Must be between 1 and 7");

            Month = month;
            Day = day;
            DayOfWeek = dayOfWeek;
            DaySpan = daySpan;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append(Month);
            result.Append('/');
            result.Append(Day);
            if (DayOfWeek.HasValue)
            {
                result.Append('/');
                result.Append(DayOfWeek.ToString());
                if (DaySpan.HasValue)
                {
                    result.Append('/');
                    result.Append(DaySpan);
                }
            }
            return result.ToString();
        }

        public static new FixedDate Parse(string text)
        {
            var split = text.Split('/');
            if (split.Length < 2 || split.Length > 4)
                throw new FormatException();
            var month = int.Parse(split[0]);
            var day = int.Parse(split[1]);
            var dayOfWeek = split.Length > 2 ? Enum.Parse<DayOfWeek>(split[2]) : default(DayOfWeek?);
            var daySpan = split.Length > 3 ? int.Parse(split[3]) : default(int?);
            return new FixedDate(month, day, dayOfWeek, daySpan);
        }

        public override bool IsRecurring => false;

        public override DateTime? Resolve(ChurchCalendar calendar, int year, DateTime? seed = default)
        {
            if (seed.HasValue)
                return null;

            DateTime result;

            if (Day == 29 && Month == 2 && !calendar.FixedCalendar.IsLeapYear(year))
            {
                // 2/29 can only be resolved in leap years or if this is a day in a fixed week
                if (!DayOfWeek.HasValue)
                    return default;

                // for the case of a day in a fixed week, the start of the week moves to 3/1
                result = new DateTime(year, 3, 1, calendar.FixedCalendar);
            }
            else
            {
                result = (Day == -1 && Month == 3)
                            ? new DateTime(year, 3, 1, calendar.FixedCalendar).AddDays(-1)
                            : new DateTime(year, Month, Day, calendar.FixedCalendar);
            }

            if (DayOfWeek.HasValue)
            {
                var adjusted = result.First(DayOfWeek.Value);
                if (DaySpan.HasValue && (adjusted - result).TotalDays >= DaySpan)
                    return default;
                return adjusted;
            }
            return result;
        }
    }
}
