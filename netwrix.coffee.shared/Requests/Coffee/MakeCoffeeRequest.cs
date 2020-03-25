using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace netwrix.coffee.shared.Requests.Coffee
{
    /// <summary>
    /// The make coffee request is required when excuting / making coffee.
    /// </summary>
    public class MakeCoffeeRequest
    {
        /// <summary>
        /// Gets or sets the number of espresso shots in the coffee
        /// </summary>
        [Range(0, 10)]
       [JsonProperty("number_espresso_shots")]
        public int NumberOfEspressoShots { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether milk should be added or not.
        [JsonProperty("add_milk")]
        public bool AddMilk { get; set; }
    }
}
