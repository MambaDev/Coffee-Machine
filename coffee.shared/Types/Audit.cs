using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace coffee.shared.Types
{
    public class AuditCoffeeDay
    {
        /// <summary>
        /// The minimum time a coffee was made on the given day.
        /// </summary>
        [JsonProperty("min")]
        public DateTimeOffset Min { get; set; }

        /// <summary>
        /// The latest time a coffee was made on the given day.
        /// </summary>
        [JsonProperty("max")]
        public DateTimeOffset Max { get; set; }

        /// <summary>
        /// The given average of coffees made on the given day over the lifetime.
        /// </summary>
        [JsonProperty("average")]
        public double Average { get; set; }

        /// <summary>
        /// The day of the week the coffee was made.
        /// </summary>
        [JsonProperty("day")]
        public DayOfWeek Day { get; set; }

        /// <summary>
        /// The hours of the day which coffees are being made.
        /// </summary>
        [JsonProperty("hours")]
        public List<AuditCoffeeHour> Hours { get; set; } = new List<AuditCoffeeHour>();
    }

    public class AuditCoffeeHour
    {
        /// <summary>
        /// Gets or sets the average of the coffees made that hour and day.
        /// </summary>
        [JsonProperty("average")]
        public double Average { get; set; }

        /// <summary>
        /// Gets the hour of the day sample.
        /// </summary>
        [JsonProperty("hour")]
        public int Hour { get; set; }

        /// <summary>
        /// Gets the day of the week related to the hour.
        /// </summary>
        [JsonProperty("day")]
        public DayOfWeek Day { get; set; }
    };
}
