using System.Net;

namespace coffee.shared.Responses.Coffee
{
    public class CoffeeMachineOnlineErrorResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineOnlineErrorResponse"/> class.
        /// </summary>
        /// <param name="action">The action that was attempted but cannot occur since its already online.</param>
        public CoffeeMachineOnlineErrorResponse(string action) : base((int)HttpStatusCode.BadRequest)
        {
            this.Message = $"The coffee machine is currently online and the action '{action}' cannot be performed.";
        }
    }
}
