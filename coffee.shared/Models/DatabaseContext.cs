using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace coffee.shared.Models
{
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Gets or sets the auditing actions database set.
        /// </summary>
        public DbSet<AuditingActions> AuditingActions { get; set; }

        /// <summary>
        /// Weeks the day.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int? WeekDay(DateTimeOffset? date)
        {
            throw new Exception();
        }

        public int? Hour(DateTimeOffset? date)
        {
            throw new Exception();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Since the core implementation does not support weekDay or hour, we have teo bind a method to
            // the sql function directly on model creation. doing so allows us to perfrom a week day
            // query dirctly on the database instead of loading the entire data locally first.
            MethodInfo methodInfoWeekDay = typeof(DatabaseContext).GetRuntimeMethod(nameof(WeekDay), new[] { typeof(DateTimeOffset) });
            modelBuilder.HasDbFunction(methodInfoWeekDay, b => b.HasTranslation(e =>
           {
               SqlExpression[] arguments = new[] { e.ElementAt(0) };
               return SqlFunctionExpression.Create(nameof(WeekDay), arguments, typeof(int?), null);
           }));

            MethodInfo methodInfoHour = typeof(DatabaseContext).GetRuntimeMethod(nameof(Hour), new[] { typeof(DateTimeOffset) });
            modelBuilder.HasDbFunction(methodInfoHour, b => b.HasTranslation(e =>
           {
               SqlExpression[] arguments = new[] { e.ElementAt(0) };
               return SqlFunctionExpression.Create(nameof(Hour), arguments, typeof(int?), null);
           }));

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// <para>
        /// Override this method to configure the database (and other options) to be used for this context.
        /// This method is called for each instance of the context that is created.
        /// The base implementation does nothing.
        /// </para>
        /// <para>
        /// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
        /// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
        /// the options have already been set, and skip some or all of the logic in
        /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
        /// </para>
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context. Databases (and other extensions)
        /// typically define extension methods on this object that allow you to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>
        /// The number of state entries written to the database.
        /// </returns>
        /// <remarks>
        /// This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        /// changes to entity instances before saving to the underlying database. This can be disabled via
        /// <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        /// </remarks>
        public override int SaveChanges()
        {
            var added = this.ChangeTracker.Entries<IAuditableModel>().Where(e => e.State == EntityState.Added).ToList();

            added.ForEach(e =>
            {
                e.Property(x => x.CreatedDatetime).CurrentValue = DateTimeOffset.UtcNow;
                e.Property(x => x.ModifiedDateTime).CurrentValue = DateTimeOffset.UtcNow;
                e.Property(x => x.CreatedDatetime).IsModified = true;
                e.Property(x => x.ModifiedDateTime).IsModified = true;
            });

            var modified = this.ChangeTracker.Entries<IAuditableModel>().Where(e => e.State == EntityState.Modified)
                .ToList();

            modified.ForEach(e =>
            {
                e.Property(x => x.ModifiedDateTime).CurrentValue = DateTimeOffset.UtcNow;
                e.Property(x => x.ModifiedDateTime).IsModified = true;

                e.Property(x => x.CreatedDatetime).CurrentValue = e.Property(x => x.CreatedDatetime).OriginalValue;
                e.Property(x => x.CreatedDatetime).IsModified = false;
            });

            return base.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database. Ensuring that if
        /// changes are made or items are created to update the given modified and creation date
        /// time.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task result contains the
        /// number of state entries written to the database.
        /// </returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var added = this.ChangeTracker.Entries<IAuditableModel>().Where(e => e.State == EntityState.Added).ToList();

            added.ForEach(e =>
            {
                e.Property(x => x.CreatedDatetime).CurrentValue = DateTimeOffset.UtcNow;
                e.Property(x => x.ModifiedDateTime).CurrentValue = DateTimeOffset.UtcNow;
                e.Property(x => x.CreatedDatetime).IsModified = true;
                e.Property(x => x.ModifiedDateTime).IsModified = true;
            });

            var modified = this.ChangeTracker.Entries<IAuditableModel>().Where(e => e.State == EntityState.Modified)
                .ToList();

            modified.ForEach(e =>
            {
                e.Property(x => x.ModifiedDateTime).CurrentValue = DateTimeOffset.UtcNow;
                e.Property(x => x.ModifiedDateTime).IsModified = true;

                e.Property(x => x.CreatedDatetime).CurrentValue = e.Property(x => x.CreatedDatetime).OriginalValue;
                e.Property(x => x.CreatedDatetime).IsModified = false;
            });

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
