using LiturgyGeek.Framework.Calendars;
using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Clcs.Enums;
using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Test.Calendars
{
    [TestClass]
    public class ChurchCalendarTest
    {
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicyEx.CamelCaseEx,
            IgnoreReadOnlyFields = true,
        };

        [TestMethod]
        public void TestDeserialize()
        {
            using (var stream = File.OpenRead(@"Calendars\DummyCalendar.json"))
            {
                var calendar = JsonSerializer.Deserialize<ChurchCalendar>(stream, jsonSerializerOptions)!;

                Assert.AreEqual("Dummy Calendar", calendar.Name);
                Assert.AreEqual("byzantine", calendar.TraditionCode);
                Assert.AreEqual(CalendarReckoning.RevisedJulian, calendar.SolarReckoning);
                Assert.AreEqual(CalendarReckoning.Julian, calendar.PaschalReckoning);

                Assert.AreEqual(2, calendar.EventRanks.Count);
                VerifyEventRank(calendar.EventRanks["great.feast"], 1, false, true);
                VerifyEventRank(calendar.EventRanks["vigil"], 3, false, true);

                Assert.AreEqual(2, calendar.Seasons.Count);
                VerifySeason(calendar.Seasons["ordinary"], "1/1", "12/31", true);
                VerifySeason(calendar.Seasons["advent"], "11/15", "12/24", false);

                Assert.AreEqual(2, calendar.Events.Count);
                VerifyEvent(calendar.Events[0], "pascha", new ChurchDate[] { "1/Sunday" }, null, null, "great.feast");
                VerifyEvent(calendar.Events[1], "holy.cross", new ChurchDate[] { "9/14" }, null, null, "great.feast");
            }
        }

        private void VerifyEventRank(ChurchEventRank eventRank, int precedence, bool monthViewHeadline, bool monthViewContent)
        {
            Assert.AreEqual(precedence, eventRank.Precedence);
            Assert.AreEqual(monthViewHeadline, eventRank._MonthViewHeadline);
            Assert.AreEqual(monthViewContent, eventRank._MonthViewContent);
        }

        private void VerifySeason(ChurchSeason season, ChurchDate startDate, ChurchDate endDate, bool isDefault)
        {
            Assert.AreEqual(startDate, season.StartDate);
            Assert.AreEqual(endDate, season.EndDate);
            Assert.AreEqual(isDefault, season.IsDefault);
        }

        private void VerifyEvent(ChurchEvent churchEvent, string occasionKey, ChurchDate[] dates, string? name, string? longName, string? eventRankKey)
        {
            Assert.AreEqual(occasionKey, churchEvent.OccasionKey);
            CollectionAssert.AreEqual(dates, churchEvent.Dates);
            Assert.AreEqual(name, churchEvent.Name);
            Assert.AreEqual(longName, churchEvent.LongName);
            Assert.AreEqual(eventRankKey, churchEvent.EventRankKey);
        }

        [TestMethod]
        public void TestCloneAndResolve()
        {
            var provider = new Provider();
            var common = provider.GetCommon();
            var calendar = provider.GetCalendar("OrthodoxNC");

            var result = calendar.CloneAndResolve(provider);

            Assert.AreNotSame(calendar, result);

            Assert.AreEqual(common.Occasions["pascha"].Name, result.Events[0].Name);
            Assert.AreEqual(common.Occasions["pascha"].LongName, result.Events[0].LongName);

            Assert.AreEqual(common.Occasions["christmas"].Name, result.Events[1].Name);
            Assert.AreEqual(common.Occasions["christmas"].LongName, result.Events[1].LongName);

            Assert.AreEqual(calendar.Events[2].Name, result.Events[2].Name);
            Assert.AreEqual(calendar.Events[2].LongName, result.Events[2].LongName);
        }

        private class Provider : IChurchCalendarProvider
        {
            private readonly ChurchCommon common = new ChurchCommon
            {
                Occasions = new Dictionary<string, ChurchOccasion>
                {
                    { "pascha", new ChurchOccasion("Holy Pascha", "The Resurrection of Our Lord and Savior Jesus Christ") },
                    { "christmas", new ChurchOccasion("Christmas", "The Nativity of Our Lord God and Savior Jesus Christ") },
                    { "conception.mary", new ChurchOccasion("Conception of Mary", "The Conception of Mary") },
                },
            };

            private readonly Dictionary<string, ChurchCalendar> calendars = new Dictionary<string, ChurchCalendar>
            {
                {
                    "OrthodoxNC",
                    new ChurchCalendar("Orthodox (New Calendar)", "byzantine", CalendarReckoning.RevisedJulian, CalendarReckoning.Julian)
                    {
                        Events = new List<ChurchEvent>
                        {
                            ChurchEvent.ByOccasion("pascha", "1/Sunday"),
                            ChurchEvent.ByOccasion("christmas", "12/25"),
                            ChurchEvent.ByOccasion("conception.mary", "12/9", "The Conception by Righteous Anna of the Most-holy Theotokos", "Conception of the Theotokos"),
                        },
                    }
                },
            };

            public ChurchCommon GetCommon() => common;

            public ChurchCalendar GetCalendar(string churchCalendarCode) => calendars[churchCalendarCode];
        }
    }
}
