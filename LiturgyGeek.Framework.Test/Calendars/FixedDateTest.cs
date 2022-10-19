﻿using LiturgyGeek.Framework.Calendars;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Test.Calendars
{
    [TestClass]
    public class FixedDateTest
    {
        private static readonly ChurchCalendar westernCalendar = new ChurchCalendar(new GregorianCalendar(), new GregorianPaschalCalendar());
        private static readonly ChurchCalendar easternNewCalendar = new ChurchCalendar(new RevisedJulianCalendar(), new JulianPaschalCalendar());
        private static readonly ChurchCalendar easternOldCalendar = new ChurchCalendar(new JulianCalendar(), new JulianPaschalCalendar());

        [TestMethod]
        public void TestSimpleCase()
        {
            var fixedDate = new FixedDate(3, 25);

            Assert.AreEqual(new DateTime(2022, 3, 25), Resolve(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 3, 25), Resolve(fixedDate, easternNewCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 4, 7), Resolve(fixedDate, easternOldCalendar, 2022));
        }

        [TestMethod]
        public void TestFebruary29()
        {
            var fixedDate = new FixedDate(2, 29);

            Assert.AreEqual(new DateTime(2020, 2, 29), Resolve(fixedDate, westernCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 2, 29), Resolve(fixedDate, easternNewCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 3, 13), Resolve(fixedDate, easternOldCalendar, 2020));

            Assert.IsNull(Resolve(fixedDate, westernCalendar, 2022));
            Assert.IsNull(Resolve(fixedDate, easternNewCalendar, 2022));
            Assert.IsNull(Resolve(fixedDate, easternOldCalendar, 2022));

            Assert.AreEqual(new DateTime(2000, 2, 29), Resolve(fixedDate, westernCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 2, 29), Resolve(fixedDate, easternNewCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 3, 13), Resolve(fixedDate, easternOldCalendar, 2000));

            Assert.IsNull(Resolve(fixedDate, westernCalendar, 2100));
            Assert.IsNull(Resolve(fixedDate, easternNewCalendar, 2100));
            Assert.AreEqual(new DateTime(2100, 3, 14), Resolve(fixedDate, easternOldCalendar, 2100));
        }

        [TestMethod]
        public void TestLastOfFebruary()
        {
            var fixedDate = new FixedDate(3, -1);

            Assert.AreEqual(new DateTime(2020, 2, 29), Resolve(fixedDate, westernCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 2, 29), Resolve(fixedDate, easternNewCalendar, 2020));
            Assert.AreEqual(new DateTime(2020, 3, 13), Resolve(fixedDate, easternOldCalendar, 2020));

            Assert.AreEqual(new DateTime(2022, 2, 28), Resolve(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 2, 28), Resolve(fixedDate, easternNewCalendar, 2022));
            Assert.AreEqual(new DateTime(2022, 3, 13), Resolve(fixedDate, easternOldCalendar, 2022));

            Assert.AreEqual(new DateTime(2000, 2, 29), Resolve(fixedDate, westernCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 2, 29), Resolve(fixedDate, easternNewCalendar, 2000));
            Assert.AreEqual(new DateTime(2000, 3, 13), Resolve(fixedDate, easternOldCalendar, 2000));

            Assert.AreEqual(new DateTime(2100, 2, 28), Resolve(fixedDate, westernCalendar, 2100));
            Assert.AreEqual(new DateTime(2100, 2, 28), Resolve(fixedDate, easternNewCalendar, 2100));
            Assert.AreEqual(new DateTime(2100, 3, 14), Resolve(fixedDate, easternOldCalendar, 2100));
        }

        [TestMethod]
        public void TestDayOfWeek()
        {
            var fixedDate = new FixedDate(11, 1, DayOfWeek.Monday);

            Assert.AreEqual(new DateTime(2022, 11, 7), Resolve(fixedDate, westernCalendar, 2022));
            Assert.AreEqual(new DateTime(2023, 11, 6), Resolve(fixedDate, westernCalendar, 2023));
            Assert.AreEqual(new DateTime(2024, 11, 4), Resolve(fixedDate, westernCalendar, 2024));
            Assert.AreEqual(new DateTime(2025, 11, 3), Resolve(fixedDate, westernCalendar, 2025));
            Assert.AreEqual(new DateTime(2026, 11, 2), Resolve(fixedDate, westernCalendar, 2026));
            Assert.AreEqual(new DateTime(2027, 11, 1), Resolve(fixedDate, westernCalendar, 2027));
            Assert.AreEqual(new DateTime(2028, 11, 6), Resolve(fixedDate, westernCalendar, 2028));
            Assert.AreEqual(new DateTime(2029, 11, 5), Resolve(fixedDate, westernCalendar, 2029));
        }

        [TestMethod]
        public void TestDaySpan()
        {
            var fixedDate = new FixedDate(11, 1, DayOfWeek.Monday, 3);

            Assert.IsNull(Resolve(fixedDate, westernCalendar, 2022));
            Assert.IsNull(Resolve(fixedDate, westernCalendar, 2023));
            Assert.IsNull(Resolve(fixedDate, westernCalendar, 2024));
            Assert.AreEqual(new DateTime(2025, 11, 3), Resolve(fixedDate, westernCalendar, 2025));
            Assert.AreEqual(new DateTime(2026, 11, 2), Resolve(fixedDate, westernCalendar, 2026));
            Assert.AreEqual(new DateTime(2027, 11, 1), Resolve(fixedDate, westernCalendar, 2027));
            Assert.IsNull(Resolve(fixedDate, westernCalendar, 2028));
            Assert.IsNull(Resolve(fixedDate, westernCalendar, 2029));
        }

        private DateTime? Resolve(FixedDate fixedDate, ChurchCalendar churchCalendar, int year)
        {
            var result = fixedDate.Resolve(churchCalendar, year);
            if (result.HasValue)
                Assert.IsNull(fixedDate.Resolve(churchCalendar, year, result));
            return result;
        }
    }
}
