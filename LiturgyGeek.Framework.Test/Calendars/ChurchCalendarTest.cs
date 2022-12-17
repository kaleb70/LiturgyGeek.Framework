using LiturgyGeek.Framework.Calendars;
using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Clcs.Enums;
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
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
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
                VerifyEventRank(calendar.EventRanks[0], 1, "great.feast", false, true);
                VerifyEventRank(calendar.EventRanks[1], 3, "vigil", false, true);

                Assert.AreEqual(2, calendar.Seasons.Count);
                VerifySeason(calendar.Seasons[0], "ordinary", "1/1", "12/31", true);
                VerifySeason(calendar.Seasons[1], "advent", "11/15", "12/24", false);

                Assert.AreEqual(2, calendar.Events.Count);
                VerifyEvent(calendar.Events[0], "pascha", new ChurchDate[] { "1/Sunday" }, null, null, "great.feast");
                VerifyEvent(calendar.Events[1], "holy.cross", new ChurchDate[] { "9/14" }, null, null, "great.feast");
            }
        }

        private void VerifyEventRank(ChurchEventRank eventRank, int precedence, string rankCode, bool monthViewHeadline, bool monthViewContent)
        {
            Assert.AreEqual(precedence, eventRank.Precedence);
            Assert.AreEqual(rankCode, eventRank.RankCode);
            Assert.AreEqual(monthViewHeadline, eventRank.MonthViewHeadline);
            Assert.AreEqual(monthViewContent, eventRank.MonthViewContent);
        }

        private void VerifySeason(ChurchSeason season, string occasionCode, ChurchDate startDate, ChurchDate endDate, bool isDefault)
        {
            Assert.AreEqual(occasionCode, season.OccasionCode);
            Assert.AreEqual(startDate, season.StartDate);
            Assert.AreEqual(endDate, season.EndDate);
            Assert.AreEqual(isDefault, season.IsDefault);
        }

        private void VerifyEvent(ChurchEvent churchEvent, string occasionCode, ChurchDate[] dates, string? name, string? shortName, string? rankCode)
        {
            Assert.AreEqual(occasionCode, churchEvent.OccasionCode);
            CollectionAssert.AreEqual(dates, churchEvent.Dates);
            Assert.AreEqual(name, churchEvent.Name);
            Assert.AreEqual(shortName, churchEvent.ShortName);
            Assert.AreEqual(rankCode, churchEvent.RankCode);
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
            Assert.AreEqual(common.Occasions["pascha"].ShortName, result.Events[0].ShortName);

            Assert.AreEqual(common.Occasions["christmas"].Name, result.Events[1].Name);
            Assert.AreEqual(common.Occasions["christmas"].ShortName, result.Events[1].ShortName);

            Assert.AreEqual(calendar.Events[2].Name, result.Events[2].Name);
            Assert.AreEqual(calendar.Events[2].ShortName, result.Events[2].ShortName);
        }

        private class Provider : IChurchCalendarProvider
        {
            private readonly ChurchCommon common = new ChurchCommon
            {
                Occasions = new Dictionary<string, ChurchOccasion>
                {
                    { "pascha", new ChurchOccasion("The Resurrection of Our Lord and Savior Jesus Christ", "Holy Pascha") },
                    { "christmas", new ChurchOccasion("The Nativity of Our Lord God and Savior Jesus Christ", "Christmas") },
                    { "conception.mary", new ChurchOccasion("The Conception of Mary", "Conception of Mary") },
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
