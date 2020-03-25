using Newtonsoft.Json;

namespace coffee.shared.Types
{
    [JsonObject("database")]
    public class DatabaseConfiguration
    {
        /// <summary>
        /// Gets or sets the connection string for the mysql database
        /// </summary>
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }
    }
}
