using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchSeason : Clcs.Model.ChurchSeason, ICloneable<ChurchSeason>
    {
        [JsonConstructor]
        public ChurchSeason(ChurchDate startDate, ChurchDate endDate)
            : base(startDate, endDate)
        {
        }

        public ChurchSeason(ChurchSeason other)
            : this(other.StartDate, other.EndDate)
        {
            IsDefault = other.IsDefault;
        }

        public ChurchSeason Clone() => new ChurchSeason(this);

        object ICloneable.Clone() => Clone();
    }
}
