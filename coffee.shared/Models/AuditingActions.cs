using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace coffee.shared.Models
{
    public enum AuditActionType
    {
        TurnOffMachine = 1,
        TurnOnMachine = 2,
        DescaleMachine = 3,
        MakeCoffee = 4,
        GetMachineState = 5,
    }

    public enum AuditActionResult
    {
        Unknown = 1,
        Failed = 2,
        Passed = 3,
    }

    [Table("auditing_actions")]
    public class AuditingActions : IAuditableModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        [Required]
        [JsonProperty("id")]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the source (the ip address of the source that triggered the action)
        /// </summary>
        /// <remarks>Ensure to have a max length of 40 characters for support of IPv6 addresses.</remarks>
        [Required]
        [JsonProperty("source")]
        [Column("source")]
        [MaxLength(40)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the result of the action that occured.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [Required]
        [JsonProperty("result")]
        [Column("result")]
        public AuditActionResult Result { get; set; }

        /// <summary>
        /// Gets or sets the type of the audit occuring.
        /// </summary>
        [Required]
        [JsonProperty("type")]
        [Column("type")]
        public AuditActionType Type { get; set; }

        /// <summary>
        /// Gets or sets the created date-time.
        /// </summary>
        /// <value>
        /// The created date-time.
        /// </value>
        [Column("created_datetime")]
        [JsonProperty("created_datetime")]
        public DateTimeOffset CreatedDatetime { get; set; }

        /// <summary>
        /// Gets or sets the modified date time.
        /// </summary>
        /// <value>
        /// The modified date time.
        /// </value>
        [Column("modified_datetime")]
        [JsonProperty("modified_datetime")]
        public DateTimeOffset ModifiedDateTime { get; set; }
    }
}
