using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class WeeklyDate : ChurchDate
    {
        public DayOfWeek DayOfWeek { get; private init; }

        public WeeklyDate(DayOfWeek dayOfWeek)
        {
            DayOfWeek = dayOfWeek;
        }

        public override string ToString() => DayOfWeek.ToString();

        public static new WeeklyDate Parse(string text) => new WeeklyDate(Enum.Parse<DayOfWeek>(text));

        public override bool IsRecurring => true;

        public override DateTime? Resolve(ChurchCalendar calendar, int year, DateTime? seed = default)
        {
            if (!seed.HasValue)
                return new DateTime(year, 1, 1, calendar.FixedCalendar).First(DayOfWeek);

            var result = seed.Value.AddDays(7);
            return result.Year == year ? result : default;
        }
    }
}
