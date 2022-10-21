using LiturgyGeek.Framework.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Core
{
    public sealed class MoveableDate : ChurchDate
    {
        public int Week { get; private init; }

        public DayOfWeek DayOfWeek { get; private init; }

        private readonly int hashCode;

        public MoveableDate(int week, DayOfWeek dayOfWeek)
        {
            Week = week;
            DayOfWeek = dayOfWeek;

            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + Week.GetHashCode();
                hashCode = hashCode * 23 + DayOfWeek.GetHashCode();
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is MoveableDate other
                    && hashCode == other.hashCode
                    && Week == other.Week
                    && DayOfWeek == other.DayOfWeek;
        }

        public override int GetHashCode() => hashCode;

        public override string ToString() => Week.ToString("+0;-0;0") + '/' + DayOfWeek.ToString();

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
