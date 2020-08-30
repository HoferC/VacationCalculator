using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PublicHoliday;

namespace VacationCalculator
{
    public static class DateTimeExtensions
    {
        public static USAPublicHoliday _holidayCalendar = new USAPublicHoliday();

        public static DateTime AddWorkdays(this DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday &&
                    !newDate.IsHoliday())
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }

        public static bool IsHoliday(this DateTime originalDate)
        {
            // INSERT YOUR HOlIDAY-CODE HERE!
            if (_holidayCalendar.PublicHolidays(originalDate.Year).Contains(originalDate.Date))
            {
                return true;
            }    
            return false;
        }
    }

}
