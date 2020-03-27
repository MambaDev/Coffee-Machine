using coffee.shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace coffee.api.test
{
    /// <summary>
    /// The class designed to provide seeding information for the testing process.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        ///  Populate the basic database information for performing the tests.
        /// </summary>
        /// <param name="databaseContext">The given database context</param>
        public static void PopulateTestData(DatabaseContext databaseContext)
        {
            if (databaseContext.AuditingActions.Any()) return;

            SeedMakeCoffeeAuditLogs(databaseContext);
        }

        /// <summary>
        /// Seeds the make coffee audit logs.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        private static void SeedMakeCoffeeAuditLogs(DatabaseContext databaseContext)
        {
            DateTimeOffset baseDate = DateTimeOffset.UtcNow.AddDays(-60);

            for (var i = 1; i <= 140; i++)
            {
                // increase the day every 10 entries
                if (i % 20 == 0) baseDate = baseDate.AddDays(1);
                baseDate = baseDate.AddMinutes(5 * i);

                var entry = new AuditingActions
                {
                    Result = i % 2 == 0 ? AuditActionResult.Failed : AuditActionResult.Passed,
                    Type = AuditActionType.MakeCoffee,
                    Source = ":8080",
                };

                var context =  databaseContext.AuditingActions.Add(entry);
                databaseContext.SaveChanges();

                var entryData = context.Entity;

                databaseContext.Entry(entryData).State = EntityState.Modified; 

                entryData.CreatedDatetime = baseDate; 
                entryData.ModifiedDateTime = baseDate;

                databaseContext.Update(entryData);
                databaseContext.SaveChanges();
            }
        }
    }
}
