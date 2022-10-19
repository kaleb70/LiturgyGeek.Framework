using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public interface IChurchEvent<TKey>
    {
        TKey Key { get; }

        ChurchDate[] Dates { get; }
    }
}
