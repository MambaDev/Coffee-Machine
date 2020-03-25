using Newtonsoft.Json;
using System.Net;

namespace coffee.shared.Responses.Coffee
{
    public class MakeCoffeeResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the seconds until the coffee machine is expected to be complete.
        /// </summary>
        [JsonProperty("seconds_until_completion")]
        public int SecondsUntilCompletion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MakeCoffeeResponse"/> class.
        /// </summary>
        /// <param name="secondsUntilCompletion">The seconds until completion of the coffee.</param>
        public MakeCoffeeResponse(int secondsUntilCompletion) : base((int)HttpStatusCode.Created)
        {
            this.SecondsUntilCompletion = secondsUntilCompletion;

            this.Message = $"The coffee machine has started making coffee, " +
                $"completion expected in {this.SecondsUntilCompletion} seconds";
        }
    }
}
