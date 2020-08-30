using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VacationCalculator.ViewModels
{
    public class VacationManagerViewModel
    {
        /// <summary>
        /// Gets or sets the current year vacation in hours as of today.
        /// </summary>
        public double CurrentYearVacation { get; set; }

        /// <summary>
        /// Gets or sets the previous year's vacation in hours.
        /// </summary>
        public double PreviousYearVacation { get; set; }

        /// <summary>
        /// Gets or sets the number of floating holidays available
        /// </summary>
        public int FloatingHolidays { get; set; }

        /// <summary>
        /// Gets or sets the days required for the next vacation
        /// </summary>
        public int DesiredDays { get; set; }

        /// <summary>
        /// Gets or sets the vacation cap in hours
        /// </summary>
        public int CapHours { get; set; }

        private int _yearsOfService;
        /// <summary>
        /// Gets or sets the years of service to use for accrual and cap.
        /// </summary>
        public int YearsOfService
        {
            get
            {
                return _yearsOfService;
            }
            set
            {
                _yearsOfService = value;
                switch (_yearsOfService)
                {
                    case 1:
                        AccrualRate = 8.0 / 15.0;
                        CapHours = 160;
                        break;
                    case 2:
                        AccrualRate = 8.0 / 11.25;
                        CapHours = 200;
                        break;
                    case 3:
                        AccrualRate = 8.0 / 9.0;
                        CapHours = 256;
                        break;
                }
            }
        }

        /// <summary>
        /// The rate at which the user accrues vacation in hours-per-hour
        /// </summary>
        public double AccrualRate { get; set; }

        /// <summary>
        /// Total hours available in current and previous year.
        /// </summary>
        public double AvailableVacationHours
        {
            get
            {
                return CurrentYearVacation + PreviousYearVacation;
            }
        }

        /// <summary>
        /// Whole days of vacation that are available.
        /// </summary>
        public int AvailableVacationDays
        {
            get
            {
                return Convert.ToInt32(Math.Floor(AvailableVacationHours / 8));
            }
        }

        /// <summary>
        /// Whole days of vacation that are available, including floats
        /// </summary>
        public int AvailableVacationDaysWithFloating
        {
            get
            {
                return Convert.ToInt32(Math.Floor(AvailableVacationHours / 8)) + FloatingHolidays;
            }
        }

        /// <summary>
        /// Gets the days it takes to earn the next vacation day.
        /// </summary>
        public int DaysPerDay
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(8.0 / AccrualRate));
            }
        }

        /// <summary>
        /// Gets the hours it will take to earn the next full day.
        /// </summary>
        public double HoursUntilNextFullDay
        {
            get
            {
                double decimalPartOfExisting = AvailableVacationHours - Math.Floor(AvailableVacationHours);
                double integralPartOfExisting = Math.Floor(AvailableVacationHours) % 8;
                string[] vacationHourParts = decimalPartOfExisting.ToString().Split('.');
                int decimalPart = 0;
                if (vacationHourParts.Length == 2)
                {
                    decimalPart = int.Parse(vacationHourParts[1].Substring(0, Math.Min(3, vacationHourParts[1].Length)));
                }
                double hourOffset = 8 - ((integralPartOfExisting) + (double)decimalPart / Math.Pow(10, decimalPart.ToString().Length));
                return hourOffset;
            }
        }

        /// <summary>
        /// Gets the date when the next full vacation day will be earned.
        /// </summary>
        public DateTime NextEarnedDayDate
        {
            get
            {
                if (AccrualRate > 0)
                {
                    double daysDouble = Math.Ceiling(HoursUntilNextFullDay / AccrualRate);
                    int daysToNextDay = 0;
                    try
                    {
                        daysToNextDay = Convert.ToInt32(daysDouble);
                    }
                    catch (OverflowException e)
                    {
                        Console.WriteLine("Caught Int Parse Exception");
                    }
                    return DateTime.Now.AddWorkdays(daysToNextDay);
                }
                return DateTime.Now;
            }
        }

        /// <summary>
        /// The earliest date that you could take your next vacation based on the nubmer of days you said you needed.
        /// Takes into account floating holidays.
        /// </summary>
        public DateTime NextVacationEarliestDate
        {
            get
            {
                if (DesiredDays <= AvailableVacationDaysWithFloating)
                {
                    return DateTime.Now;
                }
                // Figure out how many hours you need to get to the desired days, and then calculate days until next day and add
                int daysLeftToEarn = DesiredDays - AvailableVacationDaysWithFloating;
                double hoursLeftToEarn = HoursUntilNextFullDay + (daysLeftToEarn - 1) * 8;
                int daysUntilGoal = Convert.ToInt32(Math.Ceiling(hoursLeftToEarn / AccrualRate));
                return DateTime.Now.AddWorkdays(daysUntilGoal);
            }
        }

        /// <summary>
        /// Gets the date at which the vacation cap will be hit assuming constant accrual.
        /// </summary>
        public DateTime CapDate
        {
            get
            {
                double hoursToCap = CapHours - AvailableVacationHours;
                int daysUntilCapHit = Convert.ToInt32(Math.Floor(hoursToCap / AccrualRate));
                return DateTime.Now.AddWorkdays(daysUntilCapHit);
            }
        }
    }
}
