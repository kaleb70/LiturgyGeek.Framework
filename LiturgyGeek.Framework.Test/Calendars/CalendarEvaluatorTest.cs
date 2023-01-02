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
        public void TestFiveWeeks()
        {
            var provider = new CalendarProvider();
            var evaluator = new CalendarEvaluator(provider);

            var result = evaluator.Evaluate("DummyCalendar", new DateTime(2022, 8, 28), new DateTime(2022, 10, 2));
            Assert.AreEqual(35, result.Length);
            Assert.AreEqual(1, result.Count(e => e.Events.Length > 0));

            Assert.AreEqual(1, result[17].Events.Length);
            Assert.IsTrue(result[17].Events[0].Event._MonthViewContent);
        }

        [TestMethod]
        public void TestTransfer()
        {
            var provider = new CalendarProvider();
            var evaluator = new CalendarEvaluator(provider);



            var result = evaluator.Evaluate("DummyCalendar", new DateTime(2078, 5, 8), new DateTime(2078, 5, 10));
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual(1, result[0].Events.Length);
            Assert.AreEqual("pascha", result[0].Events[0].Event.OccasionKey);

            Assert.AreEqual(1, result[1].Events.Length);
            Assert.AreEqual("john", result[1].Events[0].Event.OccasionKey);



            result = evaluator.Evaluate("DummyCalendar", new DateTime(2023, 5, 8), new DateTime(2023, 5, 10));
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual(1, result[0].Events.Length);
            Assert.AreEqual("john", result[0].Events[0].Event.OccasionKey);

            Assert.AreEqual(0, result[1].Events.Length);
        }

        [TestMethod]
        public void TestHolyCross()
        {
            var provider = new CalendarProvider();
            var evaluator = new CalendarEvaluator(provider);

            var result = evaluator.Evaluate("DummyCalendar", new DateTime(2022, 9, 14), new DateTime(2022, 9, 15));
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0].Events.Length);
            Assert.IsTrue(result[0].Events[0].Event._MonthViewContent);

            Assert.AreEqual(1, result[0].Rules.Length);
            Assert.AreEqual("fast.strict", result[0].Rules[0].Rule.Key);
            Assert.IsTrue(result[0].Rules[0].RuleGroup.Value._MonthViewHeadline);
            Assert.IsFalse(result[0].Rules[0].RuleGroup.Value._MonthViewContent);
        }

        [TestMethod]
        public void TestBasilTheGreat()
        {
            var provider = new CalendarProvider();
            var evaluator = new CalendarEvaluator(provider);

            var result = evaluator.Evaluate("DummyCalendar", new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0].Events.Length);
            Assert.IsTrue(result[0].Events[0].Event._MonthViewContent);
        }

        public class CalendarProvider : IChurchCalendarProvider
        {
            public ChurchCalendar GetCalendar(string calendarKey)
            {
                using (var stream = File.OpenRead(@$"Calendars\{calendarKey}.json"))
                    return JsonSerializer.Deserialize<ChurchCalendar>(stream, Helpers.JsonSerializerOptions)!;
            }

            public ChurchCommon GetCommon() => new ChurchCommon();
        }
    }
}
