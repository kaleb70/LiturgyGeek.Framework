using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class Occasion
    {
        public string OccasionCode { get; set; }

        public string Name { get; set; }

        public string? ShortName { get; set; }

        public Occasion(string occasionCode, string name, string? shortName)
        {
            OccasionCode = occasionCode;
            Name = name;
            ShortName = shortName;
        }
    }
}
