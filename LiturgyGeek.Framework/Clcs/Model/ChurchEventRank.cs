using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchEventRank
    {
        public int Precedence { get; set; }

        [JsonConstructor]
        public ChurchEventRank()
        {
        }
    }
}
