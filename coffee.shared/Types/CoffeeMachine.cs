using System;
using System.Threading;
using System.Threading.Tasks;

namespace coffee.shared.Types
{
    public enum State
    {
        Okay = 0,
        Alert = 1
    }

    public struct CoffeeCreationOptions
    {
        public int NumEspressoShots { get; set; }
        public bool AddMilk { get; set; }
    }

    public interface ICoffeeMachine
    {
        bool IsOn { get; }
        bool IsMakingCoffee { get; }
        bool IsDescaling { get; }
        State WaterLevelState { get; }
        State BeanFeedState { get; }
        State WasteCoffeeState { get; }
        State WaterTrayState { get; }
        State DescaleState { get; }
        Task TurnOnAsync();
        Task TurnOffAsync();
        Task DescaleAsync();
        Task MakeCoffeeAsync(CoffeeCreationOptions options);
    }

    /// <summary>
    /// Stub used during testing to allow adjustment during excution to ensure its in the correct
    /// test state.
    /// </summary>
    /// <seealso cref="coffee.shared.Types.ICoffeeMachine" />
    public class CoffeeMachineTestStub : ICoffeeMachine
    {
        public bool ShouldSleep { get; set; }

        public bool IsOn { get; set; } = true;
        public bool IsMakingCoffee { get; set; }
        public bool IsDescaling { get; set; }
        public State WaterLevelState { get; set; }
        public State BeanFeedState { get; set; }
        public State WasteCoffeeState { get; set; }
        public State WaterTrayState { get; set; }
        public State DescaleState { get; set; }

        public bool IsInAlertState => this.WaterLevelState == State.Alert ||
            this.BeanFeedState == State.Alert || this.WasteCoffeeState == State.Alert ||
            this.WaterTrayState == State.Alert || this.DescaleState == State.Alert;

        public void Reset()
        {
            this.WaterLevelState = State.Okay;
            this.BeanFeedState = State.Okay;
            this.WasteCoffeeState = State.Okay;
            this.WaterTrayState = State.Okay;
            this.DescaleState = State.Okay;
            this.ShouldSleep = false;
            this.IsMakingCoffee = false;
            this.IsDescaling = false;
            this.IsOn = true;
        }

        public Task TurnOnAsync()
        {
            if (this.IsOn) throw new InvalidOperationException("Invalid state");
            this.IsOn = true;
            return Task.CompletedTask;
        }

        public Task TurnOffAsync()
        {
            if (!this.IsOn || this.IsMakingCoffee || this.IsDescaling)
                throw new InvalidOperationException("Invalid state");

            this.IsOn = false;
            return Task.CompletedTask;
        }

        public Task DescaleAsync()
        {
            if (!this.IsOn || this.IsMakingCoffee || this.IsDescaling || this.DescaleState == State.Okay)
                throw new InvalidOperationException("Invalid state");

            this.IsDescaling = true;
            // [Descale Machine]

            if (this.ShouldSleep) Thread.Sleep(3000);

            this.IsDescaling = false;
            this.DescaleState = State.Okay;

            return Task.CompletedTask;
        }

        public Task MakeCoffeeAsync(CoffeeCreationOptions options)
        {
            if (!this.IsOn || this.IsMakingCoffee || this.IsDescaling || this.IsInAlertState)
                throw new InvalidOperationException("Invalid state");

            this.IsMakingCoffee = true;

            if (this.ShouldSleep) Thread.Sleep(3000);

            this.IsMakingCoffee = false;
            return Task.CompletedTask;
        }
    }

    public class CoffeeMachineStub : ICoffeeMachine
    {
        public bool IsOn { get; private set; }
        public bool IsMakingCoffee { get; private set; }
        public bool IsDescaling { get; private set; }
        public State WaterLevelState { get; private set; }
        public State BeanFeedState { get; private set; }
        public State WasteCoffeeState { get; private set; }
        public State WaterTrayState { get; private set; }
        public State DescaleState { get; private set; }

        private bool IsInAlertState => this.WaterLevelState == State.Alert ||
            this.BeanFeedState == State.Alert || this.WasteCoffeeState == State.Alert ||
            this.WaterTrayState == State.Alert || this.DescaleState == State.Alert;

        private readonly Random _randomStateGenerator;

        public CoffeeMachineStub()
        {
            this._randomStateGenerator = new Random();
        }

        public Task TurnOnAsync()
        {
            if (this.IsOn) throw new InvalidOperationException("Invalid state");

            // Generate sample state for testing
            this.WaterLevelState = this.GetRandomState();
            this.BeanFeedState = this.GetRandomState();
            this.WasteCoffeeState = this.GetRandomState();
            this.WaterTrayState = this.GetRandomState();
            this.DescaleState = this.GetRandomState();

            // [Machine turned on]
            this.IsOn = true;
            return Task.CompletedTask;
        }

        public Task TurnOffAsync()
        {
            if (!this.IsOn || this.IsMakingCoffee || this.IsDescaling)
                throw new InvalidOperationException("Invalid state");

            // [Machine turned off]
            this.IsOn = false;
            return Task.CompletedTask;
        }

        public Task DescaleAsync()
        {
            if (!this.IsOn || this.IsMakingCoffee || this.IsDescaling || this.DescaleState ==
           State.Okay)
                throw new InvalidOperationException("Invalid state");
            this.IsDescaling = true;
            // [Descale Machine]
            Thread.Sleep(30000);
            this.IsDescaling = false;
            this.DescaleState = State.Okay;
            return Task.CompletedTask;
        }

        public Task MakeCoffeeAsync(CoffeeCreationOptions options)
        {
            if (!this.IsOn || this.IsMakingCoffee || this.IsDescaling || this.IsInAlertState)
                throw new InvalidOperationException("Invalid state");

            this.IsMakingCoffee = true;
            // [Make the coffee]

            Thread.Sleep(10000);

            this.IsMakingCoffee = false;
            return Task.CompletedTask;
        }

        // Randomly create a state for testing. This can be replaced as required.
        private State GetRandomState()
        {
            return this._randomStateGenerator.Next(1, 10) == 9 ? State.Alert : State.Okay;
        }
    }
}
