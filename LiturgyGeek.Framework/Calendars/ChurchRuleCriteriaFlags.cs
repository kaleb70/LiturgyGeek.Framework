using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public enum ChurchRuleCriteriaFlags
    {
        ExcludeDates = 0b0000_0001,
        IncludeDates = 0b0000_0010,
        IncludeRanks = 0b0000_0100,
        Season = 0b0000_1000,
        Event = 0b0001_0000,
    }
}
