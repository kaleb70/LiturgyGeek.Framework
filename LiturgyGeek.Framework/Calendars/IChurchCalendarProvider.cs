using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public interface IChurchCalendarProvider
    {
        ChurchCommon GetCommon();

        ChurchCalendar GetCalendar(string calendarKey);
    }
}
