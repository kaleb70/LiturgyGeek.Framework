using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Test.Calendars
{
    internal static class Helpers
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicyEx.CamelCaseEx,
            IgnoreReadOnlyFields = true,
        };
    }
}
