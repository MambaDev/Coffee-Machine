using System.Net;

namespace coffee.shared.Responses.Coffee
{
    public class CoffeeMachineRunningErrorResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineRunningErrorResponse"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public CoffeeMachineRunningErrorResponse(string action) : base((int)HttpStatusCode.BadRequest)
        {
            this.Message = $"The coffee machine is currently descaling or making coffee so the action '{action}' cannot be performed.";
        }
    }
}
