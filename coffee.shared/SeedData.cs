using coffee.shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace coffee.shared
{
    /// <summary>
    /// The class designed to provide seeding information for the testing process.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Populate the basic database information for performing the tests.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public static void PopulateTestData(ModelBuilder modelBuilder)
        {
            SeedMakeCoffeeAuditLogs(modelBuilder);
        }

        /// <summary>
        /// Seeds the make coffee audit logs.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        private static void SeedMakeCoffeeAuditLogs(ModelBuilder modelBuilder)
        {
            DateTimeOffset baseDate = DateTimeOffset.UtcNow.AddDays(-60);
            var seedingData = new List<AuditingActions>();

            for (var i = 1; i <= 140; i++)
            {
                if (i % 20 == 0) baseDate = baseDate.AddDays(1);
                baseDate = baseDate.AddMinutes(5 * i);

                seedingData.Add(new AuditingActions
                {
                    Id = i,
                    Result = AuditActionResult.Passed,
                    Type = AuditActionType.MakeCoffee,
                    CreatedDatetime = baseDate,
                    ModifiedDateTime = baseDate,
                    Source = ":8080",
                });
            }

            modelBuilder.Entity<AuditingActions>().HasData(seedingData);
        }
    }
}
