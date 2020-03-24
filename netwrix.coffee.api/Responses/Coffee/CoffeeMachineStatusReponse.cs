using netwrix.coffee.api.Types;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace netwrix.coffee.api.Responses.Coffee
{
    public class CoffeeMachineStatusResponse : BaseResponse
    {
        /// <summary>
        /// Gets a value indicating whether this coffee machine is in a alert state or not.
        /// </summary>
        [JsonProperty("is_alerting")]
        public bool IsAlerting { get; set; }

        /// <summary>
        /// Gets a value indicating whether this coffee machine is on.
        /// </summary>
        [JsonProperty("is_on")]
        public bool IsOn { get; set; }

        /// <summary>
        /// Gets a value indicating whether this coffee machine is making coffee.
        /// </summary>
        [JsonProperty("is_making_coffee")]
        public bool IsMakingCoffee { get; set; }

        /// <summary>
        /// Gets a value indicating whether this coffee machine is descaling.
        /// </summary>
        [JsonProperty("is_descaling")]
        public bool IsDescaling { get; set; }

        /// <summary>
        /// Gets the state of the water level.
        /// </summary>
        [JsonProperty("water_level_state")]
        public State WaterLevelState { get; set; }

        /// <summary>
        /// Gets the state of the bean feed.
        /// </summary>
        [JsonProperty("bean_feed_state")]
        public State BeanFeedState { get; set; }

        /// <summary>
        /// Gets the state of the waste coffee.
        /// </summary>
        [JsonProperty("waste_coffee_state")]
        public State WasteCoffeeState { get; set; }

        /// <summary>
        /// Gets the state of the water tray.
        /// </summary>
        [JsonProperty("water_tray_state")]
        public State WaterTrayState { get; set; }

        /// <summary>
        /// Gets the state of the descale.
        /// </summary>
        [JsonProperty("descale_state")]
        public State DescaleState { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineStatusResponse"/> class.
        /// </summary>
        /// <param name="coffeeMachine">The coffee machine used to pull the state from.</param>
        public CoffeeMachineStatusResponse(ICoffeeMachine coffeeMachine)
        {
            this.IsOn = coffeeMachine.IsOn;
            this.IsMakingCoffee = coffeeMachine.IsMakingCoffee;
            this.IsDescaling = coffeeMachine.IsDescaling;
            this.WaterLevelState = coffeeMachine.WaterLevelState;
            this.BeanFeedState = coffeeMachine.BeanFeedState;
            this.WasteCoffeeState = coffeeMachine.WaterLevelState;
            this.WaterTrayState = coffeeMachine.WaterTrayState;
            this.DescaleState = coffeeMachine.DescaleState;

            this.IsAlerting = new List<State>
            {
                this.WaterLevelState,
                this.BeanFeedState,
                this.WasteCoffeeState,
                this.WaterTrayState,
                this.DescaleState
            }.Any(e => e == State.Alert);
        }
    }
}
