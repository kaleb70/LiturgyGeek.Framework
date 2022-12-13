using LiturgyGeek.Framework.Calendars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Test.Calendars
{
    [TestClass]
    public class ChurchCalendarTest
    {
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
                    new ChurchCalendar("Orthodox (New Calendar)", CalendarReckoning.RevisedJulian, CalendarReckoning.Julian)
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
