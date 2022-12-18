using LiturgyGeek.Framework.Calendars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Test.Calendars
{
    [TestClass]
    public class CalendarEvaluatorTest
    {
        [TestMethod]
        public void TestEvaluateOneDay()
        {
            var provider = new CalendarProvider();
            var evaluator = new CalendarEvaluator(provider);

            var result = evaluator.Evaluate("DummyCalendar", new DateTime(2022, 8, 28), new DateTime(2022, 10, 2));
            Assert.AreEqual(35, result.Length);
            Assert.AreEqual(1, result.Count(e => e.Events.Count > 0));
        }

        public class CalendarProvider : IChurchCalendarProvider
        {
            public ChurchCalendar GetCalendar(string churchCalendarCode)
            {
                using (var stream = File.OpenRead(@$"Calendars\{churchCalendarCode}.json"))
                    return JsonSerializer.Deserialize<ChurchCalendar>(stream, Helpers.JsonSerializerOptions)!;
            }

            public ChurchCommon GetCommon() => new ChurchCommon();
        }
    }
}
