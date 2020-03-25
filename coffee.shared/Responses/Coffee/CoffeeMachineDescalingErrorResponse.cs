using System.Net;

namespace coffee.shared.Responses.Coffee
{
    public class CoffeeMachineDescalingErrorResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineDescalingErrorResponse"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public CoffeeMachineDescalingErrorResponse(string action) : base((int)HttpStatusCode.Conflict)
        {
            this.Message = $"The coffee machine is currently descaling or not ready for descaling, the following action '{action}' cannot be performed.";
        }
    }
}
