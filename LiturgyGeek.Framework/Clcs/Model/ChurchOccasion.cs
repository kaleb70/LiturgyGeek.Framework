using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchOccasion
    {
        public string Name { get; set; }

        public string? LongName { get; set; }

        [JsonConstructor]
        public ChurchOccasion(string name)
        {
            Name = name;
        }
    }
}
