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
        [System.Text.Json.Serialization.JsonIgnore]
        public int DesiredDays { get; set; }

        /// <summary>
        /// Gets the vacation cap in hours
        /// </summary>
        public int CapHours { get; private set; }

        /// <summary>
        /// Gets the vacation cap in days
        /// </summary>
        public int CapDays
        {
            get
            {
                return CapHours / 8;
            }
        }

        /// <summary>
        /// Gets the maximum hours that can be earned in a year.
        /// </summary>
        public int MaxEarnedHoursPerYear { get; private set; }

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
                        MaxEarnedHoursPerYear = 120;
                        break;
                    case 2:
                        AccrualRate = 8.0 / 11.25;
                        CapHours = 200;
                        MaxEarnedHoursPerYear = 160;
                        break;
                    case 3:
                        AccrualRate = 8.0 / 9.0;
                        CapHours = 256;
                        MaxEarnedHoursPerYear = 200;
                        break;
                }
            }
        }

        /// <summary>
        /// The rate at which the user accrues vacation in vacation hours-per-day
        /// </summary>
        public double AccrualRate { get; private set; }

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
                if (AccrualRate <= 0)
                {
                    return 0;
                }
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
                    return DateTime.Now.AddVacationEarningDays(daysToNextDay);
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
                return DateTime.Now.AddVacationEarningDays(daysUntilGoal);
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
                return DateTime.Now.AddVacationEarningDays(daysUntilCapHit);
            }
        }

        /// <summary>
        /// Gets the date when the desired vacation total will be hit
        /// </summary>
        /// <param name="startDate">Date to start the calculation</param>
        /// <param name="startingHours">Starting number of hours</param>
        /// <param name="targetHours">Desired/ending number of hours</param>
        /// <returns>Date when the vacation total will be greater than or equal to the desired count</returns>
        public DateTime GetDateOfEarnedVacationHours(DateTime startDate, double startingHours, double targetHours)
        {
            double hoursToEarn = targetHours - startingHours;
            DateTime newDate = startDate;
            while (hoursToEarn > 0)
            {
                newDate = newDate.AddVacationEarningDays(1);
                hoursToEarn -= AccrualRate;
            }
            return newDate;
        }

        #region Trip Management

        public int ScheduledTripVacationDays
        {
            get
            {
                return Trips.Sum(t => t.RequiredVacationDays);
            }
        }

        /// <summary>
        /// Gets the duration of all scheduled trips
        /// </summary>
        public int ScheduledTripTotalDays
        {
            get
            {
                return Trips.Sum(t => t.Duration);
            }
        }

        public List<TripViewModel> Trips { get; set; } = new List<TripViewModel>();

        /// <summary>
        /// Add a trip to the VacationManager.
        /// </summary>
        /// <remarks>Takes a copy of the provided TripViewModel, and does not retain references to the provided TripViewModel</remarks>
        /// <param name="trip">The trip to add.</param>
        public void AddTrip(TripViewModel trip)
        {
            Trips.Add(new TripViewModel(trip));
        }

        /// <summary>
        /// Clears all of the trips being tracked by the TripManager.
        /// </summary>
        public void ClearTrips()
        {
            Trips.Clear();
        }

        #endregion
    }
}
