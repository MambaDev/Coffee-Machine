using coffee.api;
using coffee.api.Services;
using coffee.api.test;
using coffee.shared.Models;
using coffee.shared.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Mamba.Cloud.Api.test
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // remove the existing context configuration, coffee and audit implementation.
                services.Remove(services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>)));
                services.Remove(services.SingleOrDefault(d => d.ServiceType == typeof(ICoffeeMachine)));

                // Create a new service provider.
                ServiceProvider serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContextPool<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("coffee-in-memory");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // add the test stub
                services.AddSingleton<ICoffeeMachine>(new CoffeeMachineTestStub());

                // Build the service provider.
                ServiceProvider sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using IServiceScope scope = sp.CreateScope();

                IServiceProvider scopedServices = scope.ServiceProvider;
                DatabaseContext appDb = scopedServices.GetRequiredService<DatabaseContext>();

                ILogger<CustomWebApplicationFactory<TStartup>> logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                appDb.Database.EnsureCreated();

                try
                {
                    SeedData.PopulateTestData(appDb);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {ex.Message}");
                }
            });
        }
    }
}
