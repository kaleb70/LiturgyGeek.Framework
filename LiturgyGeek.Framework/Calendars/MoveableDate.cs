using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class MoveableDate : ChurchDate
    {
        public int Week { get; private init; }

        public DayOfWeek DayOfWeek { get; private init; }

        public MoveableDate(int week, DayOfWeek dayOfWeek)
        {
            Week = week;
            DayOfWeek = dayOfWeek;
        }

        public override string ToString() => Week.ToString("+0;-0;0") + '/' + DayOfWeek.ToString();

        public static new MoveableDate Parse(string text)
        {
            var split = text.Split('/');
            if (split.Length != 2)
                throw new FormatException();
            return new MoveableDate(int.Parse(split[0]), Enum.Parse<DayOfWeek>(split[1]));
        }

        public override bool IsRecurring => false;

        public override DateTime? Resolve(ChurchCalendar calendar, int year, DateTime? seed = default)
        {
            if (seed.HasValue)
                return null;

            var pascha = calendar.MoveableCalendar.FindPascha(year);
            int week = Week > 0 ? Week - 1 : Week;
            return pascha.AddDays(week * 7 + (int)DayOfWeek);
        }
    }
}
