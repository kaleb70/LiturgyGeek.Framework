using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchRule
    {
        public string Summary { get; set; }

        public string? Elaboration { get; set; }

        [JsonConstructor]
        public ChurchRule(string summary)
        {
            Summary = summary;
        }
    }
}
