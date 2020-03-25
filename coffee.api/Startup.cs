using coffee.api.Services;
using coffee.shared.Models;
using coffee.shared.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace coffee.api
{
    public class Startup
    {
        /// <summary>
        /// Gets the configuration file based on execution mode (debug or release)
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DatabaseConfiguration>(this.Configuration.GetSection("database"));
            DatabaseConfiguration database = this.Configuration.GetSection("database").Get<DatabaseConfiguration>();

            services.AddEntityFrameworkMySql().AddDbContextPool<DatabaseContext>(options =>
            {
                options.UseMySql(database.ConnectionString);
                options.EnableSensitiveDataLogging(false);
            });

            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Formatting = Formatting.None;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            // since we don't work with many different machines, we can create a singleton for
            // usage. The sub would be created here or during first execution.
            services.AddSingleton<ICoffeeMachine>(new CoffeeMachineStub());

            services.AddScoped<ICoffeeMachineService, CoffeeMachineService>();
            services.AddScoped<IAuditService, AuditService>();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            // ensure that the database is created for the sake of the demo
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<DatabaseContext>().Database.EnsureCreated();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}
