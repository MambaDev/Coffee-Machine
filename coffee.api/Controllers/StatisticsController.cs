using coffee.shared.Models;
using coffee.shared.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace coffee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly DatabaseContext _databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public StatisticsController(DatabaseContext context)
        {
            this._databaseContext = context;
        }

        /// <summary>
        /// Gets the make coffee machine statistics.
        /// </summary>
        /// <response code="200">
        /// <para>
        /// returns a array of days of the week containing the average, averages per hour and first
        /// and last coffees overall
        /// </para>
        /// </response>
        [HttpGet]
        public ActionResult<IEnumerable<AuditCoffeeDay>> GetCoffeeMachineStatistics()
        {
            // Used in determining the current number of weeks between first and last coffee.
            DateTimeOffset minDay = this._databaseContext.AuditingActions.Min(e => e.CreatedDatetime);
            DateTimeOffset maxDay = this._databaseContext.AuditingActions.Max(e => e.CreatedDatetime);

            var weeksPast = Math.Ceiling((maxDay - minDay).TotalDays / 7);
            var currrentWeekDay = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.UtcNow, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            // Gather all the audit events that are related to coffee making and group them by the
            // hour and day they have taken place on. And determine the average number of coffee's
            // made on that given hour on that given day based on the total number of weeks that
            // have passed since the first coffee was made.
            IQueryable<AuditCoffeeHour> hoursQuery = this._databaseContext.AuditingActions
                .Where(e => e.Type == AuditActionType.MakeCoffee).GroupBy(e => new
                {
                    Day = this._databaseContext.WeekDay(e.CreatedDatetime),
                    Hour = this._databaseContext.Hour(e.CreatedDatetime)
                })
                .Select(e => new AuditCoffeeHour
                {
                    Average = e.Count() / weeksPast,
                    Day = (DayOfWeek)e.Key.Day + 1,
                    Hour = (int)e.Key.Hour
                });

            // Gather all audit events that are related to coffee making and group them by the day
            // of the week they occured on, determining the days average and the earliest and latest
            // coffee of that given day of the week.
            IQueryable<AuditCoffeeDay> daysQuery = this._databaseContext.AuditingActions
                .Where(e => e.Type == AuditActionType.MakeCoffee)
                .GroupBy(e => this._databaseContext.WeekDay(e.CreatedDatetime))
                .Select(e => new AuditCoffeeDay
                {
                    Max = e.Max(a => a.CreatedDatetime),
                    Min = e.Min(a => a.CreatedDatetime),
                    Average = e.Count() / weeksPast,
                    Day = (DayOfWeek)e.Key + 1,
                });

            var hourResults = hoursQuery.ToList();
            var dayResults = daysQuery.ToList();

            // bind all the given hours into the related day they occured on. 
            foreach (AuditCoffeeHour hour in hourResults)
                dayResults.First(e => e.Day == hour.Day).Hours.Add(hour);

            return this.StatusCode((int)HttpStatusCode.OK, dayResults);
        }
    }
}
