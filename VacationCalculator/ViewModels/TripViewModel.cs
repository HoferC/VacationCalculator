using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationCalculator;
using System.ComponentModel.DataAnnotations;

namespace VacationCalculator.ViewModels
{
    public class TripViewModel
    {
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Name must be non-empty")]
        public string Name { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the duration of the trip in days
        /// </summary>
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365")]
        public int Duration { get; set; } = 1;

        /// <summary>
        /// Gets the number of vacation days required to take this trip.
        /// </summary>
        public int RequiredVacationDays
        {
            get
            {
                int dayCount = 0;
                DateTime newDate = StartDate;
                for (int i = 0; i < Duration; i++)
                {
                    if (newDate.IsWorkday())
                    {
                        dayCount++;
                    }
                    newDate = newDate.AddDays(1);
                }
                return dayCount;
            }
        }

        public DateTime EndDate 
        { 
            get
            {
                return StartDate.AddDays(Duration-1);
            } 
        }

        public DateTime CalendarEndDate
        {
            get
            {
                return EndDate;
                //return EndDate.AddDays(1);
            }
        }

        public DateTime ReturnToWorkDate
        {
            get
            {
                return EndDate.AddDays(1).NextWorkday();
            }
        }

        public TripViewModel()
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new TripViewModel from another one, copying its state.
        /// </summary>
        /// <param name="original">The existing TripViewModel to clone from.</param>
        public TripViewModel(TripViewModel original)
        {
            this.Name = original.Name;
            this.StartDate = original.StartDate;
            this.Duration = original.Duration;
        }

        public void Reset()
        {
            Name = string.Empty;
            StartDate = DateTime.Now;
            Duration = 1;
        }
    }
}
