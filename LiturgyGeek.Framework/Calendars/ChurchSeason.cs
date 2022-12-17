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
        public ChurchSeason(string? occasionCode, ChurchDate startDate, ChurchDate endDate, bool isDefault = false)
            : base(startDate, endDate)
        {
            OccasionCode = occasionCode;
            IsDefault = isDefault;
        }

        public ChurchSeason(ChurchDate startDate, ChurchDate endDate, bool isDefault = false, IEnumerable<ChurchEvent>? events = default)
            : this(default, startDate, endDate, isDefault)
        {
        }

        public ChurchSeason(ChurchSeason other)
            : this(other.OccasionCode, other.StartDate, other.EndDate, other.IsDefault)
        {
        }

        public ChurchSeason Clone() => new ChurchSeason(this);

        object ICloneable.Clone() => Clone();
    }
}
