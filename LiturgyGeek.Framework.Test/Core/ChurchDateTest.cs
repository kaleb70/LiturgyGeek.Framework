using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Test.Core
{
    [TestClass]
    public class ChurchDateTest
    {
        [TestMethod]
        public void TestParseWeeklyDate()
        {
            Assert.AreEqual(new WeeklyDate(DayOfWeek.Monday), ChurchDate.Parse("Monday"));
        }

        [TestMethod]
        public void TestParseMoveableDate()
        {
            Assert.AreEqual(new MoveableDate(-1, DayOfWeek.Sunday), ChurchDate.Parse("-1/Sunday"));
            Assert.AreEqual(new MoveableDate(6, DayOfWeek.Thursday), ChurchDate.Parse("6/Thursday"));
        }

        [TestMethod]
        public void TestParseFixedDate()
        {
            Assert.AreEqual(new FixedDate(1, 6), ChurchDate.Parse("1/6"));
            Assert.AreEqual(new FixedDate(12, 25), ChurchDate.Parse("12/25"));

            Assert.AreEqual(new FixedDate(11, 27, DayOfWeek.Sunday), ChurchDate.Parse("11/27/Sunday"));
            Assert.AreEqual(new FixedDate(12, 22, DayOfWeek.Thursday, 3), ChurchDate.Parse("12/22/Thursday/3"));

            Assert.AreEqual(new FixedDate(1, 1, DayOfWeek.Monday, DayOfWeek.Tuesday), ChurchDate.Parse("1/1/Monday-Tuesday"));
        }

        [TestMethod]
        public void TestParseMonthlyDate()
        {
            Assert.AreEqual(new MonthlyDate(10), ChurchDate.Parse("*/10"));
            Assert.AreEqual(new MonthlyDate(10, DayOfWeek.Sunday), ChurchDate.Parse("*/10/Sunday"));
        }
    }
}
