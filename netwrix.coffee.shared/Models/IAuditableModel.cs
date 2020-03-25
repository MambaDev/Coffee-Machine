using System;

namespace netwrix.coffee.shared.Models
{
    public interface IAuditableModel
    {
        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        DateTimeOffset ModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the created datetime.
        /// </summary>
        /// <value>
        /// The created datetime.
        /// </value>
        DateTimeOffset CreatedDatetime { get; }
    }
}