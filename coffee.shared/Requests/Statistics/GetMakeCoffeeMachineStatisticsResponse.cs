using coffee.shared.Responses;
using System.Collections.Generic;
using coffee.shared.Types;
using Newtonsoft.Json;

namespace coffee.shared.Requests.Statistics
{
    public class GetMakeCoffeeMachineStatisticsResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the make coffee days.
        /// </summary>
       [JsonProperty("make_coffee_days")]
        public IEnumerable<AuditCoffeeDay> MakeCoffeeDays { get; set; }
    }
}
