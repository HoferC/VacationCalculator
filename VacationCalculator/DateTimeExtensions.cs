using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PublicHoliday;

namespace VacationCalculator
{
    public static class DateTimeExtensions
    {
        public static USAPublicHoliday _holidayCalendar = new USAPublicHoliday();

        /// <summary>
        /// Adds workdays to a given date. Does not count holidays as days.
        /// </summary>
        /// <param name="date">Start date of the calculation</param>
        /// <param name="workingDays">Number of working days to count</param>
        /// <returns>The date offset from the incoming date by the number of working days.</returns>
        public static DateTime AddWorkdays(this DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.IsWorkday())
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }

        /// <summary>
        /// Add non-weekend days to a given date. Counts holidays as days.
        /// </summary>
        /// <param name="date">Start date of the calculation</param>
        /// <param name="workingDays">Number of working days to count</param>
        /// <returns>The date offset from the incoming date by the number of vacation-earning days.</returns>
        public static DateTime AddVacationEarningDays(this DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.IsVacationEarningDay())
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }

        public static bool IsHoliday(this DateTime originalDate)
        {
            // INSERT YOUR HOLIDAY-CODE HERE!
            if (_holidayCalendar.PublicHolidays(originalDate.Year).Contains(originalDate.Date))
            {
                return true;
            }    
            return false;
        }

        public static bool IsWorkday(this DateTime originalDate)
        {
            if (originalDate.DayOfWeek == DayOfWeek.Saturday ||
                originalDate.DayOfWeek == DayOfWeek.Sunday ||
                originalDate.IsHoliday())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if this day earns vacation
        /// </summary>
        /// <param name="originalDate">Date to check for vacation earning</param>
        /// <returns>true if you earn vacation on this day, false otherwise</returns>
        public static bool IsVacationEarningDay(this DateTime originalDate)
        {
            if (originalDate.DayOfWeek == DayOfWeek.Saturday ||
                originalDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the workdays until the given date. (A workday is any day that is not Saturday, Sunday, or a holiday)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int WorkdaysUntil(this DateTime startDate, DateTime endDate)
        {
            int workDays = 0;
            int totalDays = (endDate.Date - startDate.Date).Days;
            // If end date is before start date, their difference in days will be negative
            int direction = totalDays < 0 ? -1 : 1;
            DateTime newDate = startDate;
            while (totalDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.IsWorkday())
                {
                    workDays++;
                }
                totalDays -= direction;
            }
            return workDays;
        }

        /// <summary>
        /// Gets the next workday after this date
        /// </summary>
        /// <param name="originalDate">Start date</param>
        /// <returns>The date of the next workday</returns>
        public static DateTime NextWorkday(this DateTime originalDate)
        {
            DateTime newDate = originalDate;
            while (!newDate.IsWorkday())
            {
                newDate = newDate.AddDays(1);
            }
            return newDate.Date;
        }
    }

}
