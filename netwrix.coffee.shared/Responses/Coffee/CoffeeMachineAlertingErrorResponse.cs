using System.Net;

namespace netwrix.coffee.shared.Responses.Coffee
{
    public class CoffeeMachineAlertingErrorResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineAlertingErrorResponse"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public CoffeeMachineAlertingErrorResponse(string action) : base((int)HttpStatusCode.Conflict)
        {
            this.Message = $"The coffee machine is currently in a alert state and the action '{action}' cannot be performed.";
        }
    }
}
