using coffee.shared.Models;
using System;

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
            SeedMakeCoffeeAuditLogs(databaseContext);
        }

        /// <summary>
        /// Seeds the make coffee audit logs.
        /// </summary>
        /// <param name="databaseContext">The database context.</param>
        private static void SeedMakeCoffeeAuditLogs(DatabaseContext databaseContext)
        {
            var baseDate = DateTimeOffset.UtcNow;

            for (int i = 0; i < 140; i++)
            {
                // increase the day every 10 entries
                if (i % 10 == 0) baseDate.AddDays(1);

                var entry = new AuditingActions
                {
                    Result = i % 2 == 0 ? AuditActionResult.Failed : AuditActionResult.Passed,
                    Type = AuditActionType.MakeCoffee,
                    CreatedDatetime = baseDate,
                    Source = ":8080",
                };

                databaseContext.AuditingActions.Add(entry);
                databaseContext.SaveChanges();
            } 
        }
    }
}
