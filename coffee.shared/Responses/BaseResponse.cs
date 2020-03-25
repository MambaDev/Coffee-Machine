using Newtonsoft.Json;
using System.Net;

namespace coffee.shared.Responses
{
    public class BaseResponse
    {
        /// <summary>
        /// Gets or sets the message of the base request
        /// </summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get;  protected set; }

        /// <summary>
        /// Gets or sets the status of the base request.
        /// </summary>
       [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public int Status { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code repsonse of the request..</param>
        public BaseResponse(int statusCode = (int)HttpStatusCode.OK)
        {
            this.Status = statusCode;
        }

        /// <summary>
        /// If the given response is a good response or not.
        /// </summary>
        public bool Ok() => this.Status >= 200 && this.Status <= 299;
    }
}
