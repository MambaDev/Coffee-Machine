using Microsoft.AspNetCore.Http;
using coffee.shared.Models;
using coffee.shared.Responses;
using System;
using System.Threading.Tasks;

namespace coffee.api.Services
{
    public interface IAuditService
    {
        /// <summary>
        /// Adds the audit entry for request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        Task AddAuditEntryForRequest(HttpRequest request, BaseResponse response,  AuditActionType type);
    }

    public class AuditService : IAuditService
    {
        /// <summary>
        /// The database context used to record usages.
        /// </summary>
        private readonly DatabaseContext _databaseContext;

        public AuditService(DatabaseContext databaseContext)
        {
            this._databaseContext = databaseContext;
        }

        /// <inheritdoc/>
        public async Task AddAuditEntryForRequest(HttpRequest request, BaseResponse response, AuditActionType type)
        {
           await this._databaseContext.AuditingActions.AddAsync(new AuditingActions
            {
                Result = response.Ok() ? AuditActionResult.Passed : AuditActionResult.Failed,
                Source = request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Type = type,
            }).ConfigureAwait(false);

            await this._databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
