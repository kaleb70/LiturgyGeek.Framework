using LiturgyGeek.Framework.Clcs.Dates;
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

        public IEnumerable<(TSeason Season, DateTime StartDate, DateTime EndDate)> ResolveAll<TSeason>(DateTime minDate, DateTime maxDate, IEnumerable<(TSeason Season, ChurchDate StartDate, ChurchDate EndDate)> seasons)
        {
            minDate = minDate.Date;
            maxDate = maxDate.Date;

            int startYear = minDate.Month < 4 ? minDate.Year - 1 : minDate.Year;
            int endYear = maxDate.Month > 9 ? maxDate.Year + 1 : maxDate.Year;

            for (int year = startYear; year <= endYear; year++)
            {
                foreach (var season in seasons)
                {
                    DateTime? resolvedStartDate = season.StartDate.Resolve(this, year);
                    DateTime? resolvedEndDate = season.EndDate.Resolve(this, year);
                    if (resolvedStartDate > resolvedEndDate && resolvedStartDate!.GetType() == resolvedEndDate!.GetType())
                        resolvedEndDate = season.EndDate.Resolve(this, year + 1);
                    if (resolvedStartDate <= resolvedEndDate && resolvedEndDate >= minDate && resolvedStartDate < maxDate)
                        yield return (Season: season.Season, StartDate: resolvedStartDate.Value, EndDate: resolvedEndDate.Value);
                }
            }
        }

        public IEnumerable<(TEvent Event, DateTime Date)> ResolveAll<TEvent>(DateTime minDate, DateTime maxDate, IEnumerable<(TEvent Event, ChurchDate Date)> churchEvents)
        {
            minDate = minDate.Date;
            maxDate = maxDate.Date;

            int startYear = minDate.Month < 4 ? minDate.Year - 1 : minDate.Year;
            int endYear = maxDate.Month > 9 ? maxDate.Year + 1 : maxDate.Year;

            for (int year = startYear; year <= endYear; year++)
            {
                foreach (var churchEvent in churchEvents)
                {
                    DateTime? resolvedDate = default;
                    do
                    {
                        resolvedDate = churchEvent.Date.Resolve(this, year, resolvedDate);
                        if (resolvedDate >= minDate && resolvedDate < maxDate)
                            yield return (Event: churchEvent.Event, Date: resolvedDate.Value);

                    } while (resolvedDate.HasValue);
                }
            }
        }
    }
}
