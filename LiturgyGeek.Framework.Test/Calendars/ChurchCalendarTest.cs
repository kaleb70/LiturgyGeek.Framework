using LiturgyGeek.Framework.Calendars;
using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Clcs.Enums;
using ChurchRuleCriteria = LiturgyGeek.Framework.Clcs.Model.ChurchRuleCriteria;
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
        [TestMethod]
        public void TestDeserialize()
        {
            using (var stream = File.OpenRead(@"Calendars\DummyCalendar.json"))
            {
                var calendar = JsonSerializer.Deserialize<ChurchCalendar>(stream, Helpers.JsonSerializerOptions)!;

                Assert.AreEqual("Dummy Calendar", calendar.Name);
                Assert.AreEqual("byzantine", calendar.TraditionKey);
                Assert.AreEqual(CalendarReckoning.RevisedJulian, calendar.SolarReckoning);
                Assert.AreEqual(CalendarReckoning.Julian, calendar.PaschalReckoning);

                Assert.AreEqual(2, calendar.RuleGroups.Count);

                VerifyRuleGroup(calendar.RuleGroups["fast"], 4);
                VerifyRule(calendar.RuleGroups["fast"].Rules["fast.strict"], "Fast", "Strict Fast");
                VerifyRule(calendar.RuleGroups["fast"].Rules["fast.oil"], "Fast (oil/wine)", "Oil and Wine Permitted");
                VerifyRule(calendar.RuleGroups["fast"].Rules["fast.fish"], "Fast (fish)", "Fish, Oil, and Wine Permitted");
                VerifyRule(calendar.RuleGroups["fast"].Rules["fast.none"], "No Fast", null);

                VerifyRuleGroup(calendar.RuleGroups["colors"], 1);
                VerifyRule(calendar.RuleGroups["colors"].Rules["colors.green"], "Green", null);

                Assert.AreEqual(3, calendar.EventRanks.Count);
                VerifyEventRank(calendar.EventRanks["great.feast"], 1, false, true);
                VerifyEventRank(calendar.EventRanks["vigil"], 3, false, true);
                VerifyEventRank(calendar.EventRanks["ordinary"], 99, false, false);

                Assert.AreEqual(2, calendar.Seasons.Count);

                VerifySeason(calendar.Seasons["ordinary"], "1/1", "12/31", true, 1);
                Assert.AreEqual(2, calendar.Seasons["ordinary"].RuleCriteria["fast"].Length);
                VerifyRuleCriteria(calendar.Seasons["ordinary"].RuleCriteria["fast"][0],
                                    "fast.strict",
                                    includeDates: new ChurchDate[] { "Wednesday", "Friday" });
                VerifyRuleCriteria(calendar.Seasons["ordinary"].RuleCriteria["fast"][1],
                                    "fast.fish",
                                    includeDates: new ChurchDate[] { "Wednesday", "Friday" },
                                    includeRanks: new[] { "great.feast", "patron", "vigil" });

                VerifySeason(calendar.Seasons["advent"], "11/15", "12/24", false, 0);

                Assert.AreEqual(5, calendar.Events.Count);
                VerifyEvent(calendar.Events[0], "pascha", new ChurchDate[] { "1/Sunday" }, null, null, "great.feast", null, null);
                VerifyEvent(calendar.Events[1], "john", new ChurchDate[] { "5/8" }, null, null, "vigil", null, null);
                VerifyEvent(calendar.Events[2], "holy.cross", new ChurchDate[] { "9/14" }, null, null, "great.feast", null, null);
                VerifyEvent(calendar.Events[3], "basilGreat", new ChurchDate[] { "1/1" }, null, null, "ordinary", null, true);
                VerifyEvent(calendar.Events[4], "malachi", new ChurchDate[] { "1/3" }, null, null, null, null, null);
            }
        }

        private void VerifyRuleGroup(ChurchRuleGroup ruleGroup, int ruleCount)
        {
            Assert.AreEqual(ruleCount, ruleGroup.Rules.Count);
        }

        private void VerifyRule(ChurchRule rule, string summary, string? elaboration)
        {
            Assert.AreEqual(summary, rule.Summary);
            Assert.AreEqual(elaboration, rule.Elaboration);
        }

        private void VerifyRuleCriteria(ChurchRuleCriteria ruleCriteria,
                                        string ruleKey,
                                        ChurchDate? startDate = null,
                                        ChurchDate? endDate = null,
                                        ChurchDate[]? includeDates = null,
                                        string[]? includeRanks = null,
                                        ChurchDate[]? excludeDates = null)
        {
            Assert.AreEqual(ruleKey, ruleCriteria.RuleKey);
            Assert.AreEqual(startDate, ruleCriteria.StartDate);
            Assert.AreEqual(endDate, ruleCriteria.EndDate);
            CollectionAssert.AreEqual(includeDates ?? new ChurchDate[0], ruleCriteria.IncludeDates.ToArray());
            CollectionAssert.AreEqual(includeRanks ?? new string[0], ruleCriteria.IncludeRanks.ToArray());
            CollectionAssert.AreEqual(excludeDates ?? new ChurchDate[0], ruleCriteria.ExcludeDates.ToArray());
        }

        private void VerifyEventRank(ChurchEventRank eventRank, int precedence, bool monthViewHeadline, bool monthViewContent)
        {
            Assert.AreEqual(precedence, eventRank.Precedence);
            Assert.AreEqual(monthViewHeadline, eventRank._MonthViewHeadline);
            Assert.AreEqual(monthViewContent, eventRank._MonthViewContent);
        }

        private void VerifySeason(ChurchSeason season, ChurchDate startDate, ChurchDate endDate, bool isDefault, int ruleCriteriaCount)
        {
            Assert.AreEqual(startDate, season.StartDate);
            Assert.AreEqual(endDate, season.EndDate);
            Assert.AreEqual(isDefault, season.IsDefault);
            Assert.AreEqual(ruleCriteriaCount, season.RuleCriteria.Count);
        }

        private void VerifyEvent(ChurchEvent churchEvent, string occasionKey, ChurchDate[] dates, string? name, string? longName, string? eventRankKey, bool? monthViewHeadline, bool? monthViewContent)
        {
            Assert.AreEqual(occasionKey, churchEvent.OccasionKey);
            CollectionAssert.AreEqual(dates, churchEvent.Dates);
            Assert.AreEqual(name, churchEvent.Name);
            Assert.AreEqual(longName, churchEvent.LongName);
            Assert.AreEqual(eventRankKey, churchEvent.EventRankKey);
            Assert.AreEqual(monthViewHeadline, churchEvent._MonthViewHeadline);
            Assert.AreEqual(monthViewContent, churchEvent._MonthViewContent);
        }

        [TestMethod]
        public void TestCloneAndMerge()
        {
            var provider = new Provider();
            var common = provider.GetCommon();
            var calendar = provider.GetCalendar("OrthodoxNC");

            var result = calendar.CloneAndMerge(provider);

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
                    new ChurchCalendar("Orthodox (New Calendar)", "byzantine", "oca", "ordinary", CalendarReckoning.RevisedJulian, CalendarReckoning.Julian)
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

            public ChurchCalendar GetCalendar(string calendarKey) => calendars[calendarKey];
        }
    }
}
