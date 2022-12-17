using LiturgyGeek.Framework.Clcs.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchSeason
    {
        public ChurchDate StartDate { get; set; }

        public ChurchDate EndDate { get; set; }

        public bool IsDefault { get; set; }

        [JsonConstructor]
        public ChurchSeason(ChurchDate startDate, ChurchDate endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
