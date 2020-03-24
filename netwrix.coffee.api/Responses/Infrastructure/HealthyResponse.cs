using Newtonsoft.Json;

namespace netwrix.coffee.api.Responses.Infrastructure
{
    public class HealthyResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HealthyResponse"/> is online.
        /// </summary>
       [JsonProperty("online")]
        public bool Online { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthyResponse"/> class.
        /// </summary>
        /// <param name="online">if set to <c>true</c> [online].</param>
        public HealthyResponse(bool online)
        {
            this.Online = online;
        }
    }
}
