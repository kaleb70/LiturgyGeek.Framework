using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class MonthlyDate : ChurchDate
    {
        public int Day { get; private init; }

        public DayOfWeek? DayOfWeek { get; private init; }

        public int? DaySpan { get; private init; }

        public MonthlyDate(int day) : this(day, default(DayOfWeek?), default(int?))
        {
        }

        public MonthlyDate(int day, DayOfWeek dayOfWeek) : this(day, (DayOfWeek?)dayOfWeek, default(int?))
        {
        }

        public MonthlyDate(int day, DayOfWeek dayOfWeek, int daySpan) : this(day, (DayOfWeek?)dayOfWeek, (int?)daySpan)
        {
        }

        private MonthlyDate(int day, DayOfWeek? dayOfWeek, int? daySpan)
        {
            if (day < -31 || day > 31)
                throw new ArgumentOutOfRangeException(nameof(day), "Must be a nonzero value between -31 and 31");
            else if (day == 0)
                throw new ArgumentException("Value must be nonzero", nameof(day));

            if (dayOfWeek.HasValue && !Enum.IsDefined(dayOfWeek.Value))
                throw new ArgumentException("Invalid value", nameof(dayOfWeek));

            if (daySpan < 1 || daySpan > 7)
                throw new ArgumentOutOfRangeException(nameof(daySpan), "Must be between 1 and 7");

            Day = day;
            DayOfWeek = dayOfWeek;
            DaySpan = daySpan;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("*/");
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

        public static new MonthlyDate Parse(string text)
        {
            var split = text.Split('/');
            if (split.Length < 2 || split.Length > 4 || split[0].Trim() != "*")
                throw new FormatException();
            var day = int.Parse(split[1]);
            var dayOfWeek = split.Length > 2 ? Enum.Parse<DayOfWeek>(split[2]) : default(DayOfWeek?);
            var daySpan = split.Length > 3 ? int.Parse(split[3]) : default(int?);
            return new MonthlyDate(day, dayOfWeek, daySpan);
        }

        public override bool IsRecurring => false;

        public override DateTime? Resolve(ChurchCalendar calendar, int year, DateTime? seed = default)
        {
            DateTime result;
            if (Day < 0)
            {
                DateTime basis = seed.HasValue
                                    ? new DateTime(seed.Value.Year, seed.Value.Month, 1).AddMonths(2)
                                    : new DateTime(year, 2, 1);

                while (basis.AddDays(-1).Day < -Day)
                    basis = basis.AddMonths(1);

                result = basis.AddDays(Day);
            }
            else
            {
                DateTime basis = seed.HasValue
                                    ? new DateTime(seed.Value.Year, seed.Value.Month, 1).AddMonths(1)
                                    : new DateTime(year, 1, 1);

                while (basis.AddDays(Day - 1).Month != basis.Month)
                    basis = basis.AddMonths(1);

                result = basis.AddDays(Day);
            }

            if (DayOfWeek.HasValue)
            {
                var adjusted = result.First(DayOfWeek.Value);
                if (DaySpan.HasValue && (adjusted - result).TotalDays >= DaySpan)
                    return default;
                result = adjusted;
            }

            return result.Year == year ? result : default;
        }
    }
}
