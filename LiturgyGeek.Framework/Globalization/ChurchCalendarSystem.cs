﻿using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Globalization
{
    public class ChurchCalendarSystem
    {
        public Calendar FixedCalendar { get; private init; }

        public PaschalCalendar MoveableCalendar { get; private init; }

        public ChurchCalendarSystem(Calendar fixedCalendar, PaschalCalendar moveableCalendar)
        {
            FixedCalendar = fixedCalendar;
            MoveableCalendar = moveableCalendar;
        }

        public IEnumerable<(TEvent Event, DateTime Date)> ResolveAll<TEvent>(DateTime startDate, DateTime endDate, IEnumerable<(TEvent Event, ChurchDate Date)> churchEvents)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            int startYear = startDate.Month < 4 ? startDate.Year - 1 : startDate.Year;
            int endYear = endDate.Month > 9 ? endDate.Year + 1 : endDate.Year;

            for (int year = startYear; year <= endYear; year++)
            {
                foreach (var churchEvent in churchEvents)
                {
                    DateTime? resolvedDate = default;
                    do
                    {
                        resolvedDate = churchEvent.Date.Resolve(this, year, resolvedDate);
                        if (resolvedDate >= startDate && resolvedDate < endDate)
                            yield return (Event: churchEvent.Event, Date: resolvedDate.Value);

                    } while (resolvedDate.HasValue);
                }
            }
        }
    }
}
