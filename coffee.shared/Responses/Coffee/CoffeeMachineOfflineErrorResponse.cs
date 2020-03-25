using System.Net;

namespace coffee.shared.Responses.Coffee
{
    public class CoffeeMachineOfflineErrorResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineOfflineErrorResponse"/> class.
        /// </summary>
        /// <param name="action">The action that was being performed that resulted in the exception.</param>
        public CoffeeMachineOfflineErrorResponse(string action) : base((int)HttpStatusCode.Conflict)
        {
            this.Message = $"The coffee machine is currently offline and the action '{action}' cannot be performed.";
        }
    }
}
