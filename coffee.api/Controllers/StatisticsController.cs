using coffee.shared.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace coffee.api.Controllers
{
    public class AuditDayResult
    {
        [JsonProperty("min")]
        public DateTimeOffset Min { get; set; }

        [JsonProperty("max")]
        public DateTimeOffset Max { get; set; }

        [JsonProperty("average")]
        public double Average { get; set; }

        [JsonProperty("day")]
        public DayOfWeek Day => this.Min.DayOfWeek;

        [JsonProperty("hours")]
        public List<object> Hours { get; set; } = new List<object>();
    }

    public class AuditHourResult
    {
        [JsonProperty("average")]
        public double Average { get; set; }

        [JsonProperty("hour")]
        public int Hour => this.DaySample.Hour;

        [JsonIgnore]
        public DateTimeOffset DaySample { get; set; }

        [JsonIgnore]
        public int Day => (int)this.DaySample.DayOfWeek;
    };

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
        public IEnumerable<AuditDayResult> GetCoffeeMachineStatistics()
        {
            DateTimeOffset minDay = this._databaseContext.AuditingActions.Min(e => e.CreatedDatetime);
            DateTimeOffset maxDay = this._databaseContext.AuditingActions.Max(e => e.CreatedDatetime);

            var weeksPast = Math.Ceiling((maxDay - minDay).TotalDays / 7);

            IQueryable<AuditHourResult> hoursQuery = this._databaseContext.AuditingActions
                .Where(e => e.Type == AuditActionType.MakeCoffee).GroupBy(e => new
                {
                    day = this._databaseContext.WeekDay(e.CreatedDatetime),
                    hour = this._databaseContext.Hour(e.CreatedDatetime)
                })
                .Select(e => new AuditHourResult
                {
                    Average = e.Count() / weeksPast,
                    DaySample = e.Max(a => a.CreatedDatetime),
                });

            IQueryable<AuditDayResult> daysQuery = this._databaseContext.AuditingActions
                .Where(e => e.Type == AuditActionType.MakeCoffee).GroupBy(e => this._databaseContext.WeekDay(e.CreatedDatetime))
                .Select(e => new AuditDayResult
                {
                    Max = e.Max(a => a.CreatedDatetime),
                    Min = e.Min(a => a.CreatedDatetime),
                    Average = e.Count() / weeksPast,
                });

            var hourResults = hoursQuery.ToList();
            var dayResults = daysQuery.ToList();

            foreach (AuditHourResult hour in hourResults)
                dayResults.First(e => e.Max.DayOfWeek == (DayOfWeek)hour.Day).Hours.Add(hour);

            return dayResults;
        }
    }
}
