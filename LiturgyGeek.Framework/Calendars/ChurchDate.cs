using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public abstract class ChurchDate
    {
        public static ChurchDate Parse(string text)
        {
            throw new NotImplementedException();
        }

        public abstract bool IsRecurring { get; }

        public abstract DateTime? Resolve(ChurchCalendar calendar, int year, DateTime? seed = default);
    }
}
