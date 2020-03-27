using coffee.shared.Requests.Coffee;
using coffee.shared.Responses.Coffee;
using coffee.shared.Types;
using Mamba.Cloud.Api.test;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace coffee.api.test
{
    public class CoffeeControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        /// <summary>
        /// The factory used to access the clients, server, and services.
        /// </summary>
        private readonly CustomWebApplicationFactory<Startup> _factory;

        private readonly ITestOutputHelper _testOutputHelper;

        /// <summary>
        /// The base url of the authentication endpoint.
        /// </summary>
        private readonly string _baseUrl = "/api/coffee";

        /// <summary>
        /// The default client
        /// </summary>
        private readonly HttpClient Client;

        /// <summary>
        ///  Creates a new instance of the authentication controller integration tests.
        /// </summary>
        /// <param name="factory">The factory of the tests.</param>
        /// <param name="testOutputHelper">Helper for performing logs</param>
        public CoffeeControllerIntegrationTest(CustomWebApplicationFactory<Startup> factory,
            ITestOutputHelper testOutputHelper)
        {
            this._factory = factory;
            this._testOutputHelper = testOutputHelper;

            this.Client = this._factory.CreateClient();
        }

        /// <summary>
        /// Nots the allowed to make coffee during alert.
        /// </summary>
        [Theory]
        [InlineData(State.Okay, State.Okay, State.Okay, State.Okay, State.Alert, false)]
        [InlineData(State.Okay, State.Okay, State.Okay, State.Alert, State.Okay, false)]
        [InlineData(State.Okay, State.Okay, State.Alert, State.Okay, State.Okay, false)]
        [InlineData(State.Okay, State.Alert, State.Okay, State.Okay, State.Okay, false)]
        [InlineData(State.Alert, State.Okay, State.Okay, State.Okay, State.Okay, false)]
        [InlineData(State.Okay, State.Okay, State.Okay, State.Okay, State.Okay, true)]
        public async Task Not_Allowed_To_MakeCoffee_DuringAlert(State water, State bean, State waste, State tray, State descale, bool canMake)
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.WaterLevelState = water;
            coffeeMachine.BeanFeedState = bean;
            coffeeMachine.WasteCoffeeState = waste;
            coffeeMachine.WaterTrayState = tray;
            coffeeMachine.DescaleState = descale;

            var body = JsonConvert.SerializeObject(new MakeCoffeeRequest());
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/make", content).ConfigureAwait(false);
            var resultBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            HttpStatusCode result = canMake ? HttpStatusCode.Created : HttpStatusCode.Conflict;
            this._testOutputHelper.WriteLine(resultBody);

            Assert.Equal(coffeeMachine.IsInAlertState, !canMake);
            Assert.Equal(result, response.StatusCode);
        }

        [Fact]
        public async Task Not_Allowed_Descale_When_Not_DescaleAlert()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/descale", null).ConfigureAwait(false);
            var resultBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        /// <summary>
        /// Allows the descale when descale alert and on.
        /// </summary>
        [Fact]
        public async Task Allowed_Descale_When_DescaleAlert()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.DescaleState = State.Alert;

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/descale", null).ConfigureAwait(false);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // machine should be in a good state again
            Assert.Equal(State.Okay, coffeeMachine.DescaleState);
            Assert.False(coffeeMachine.IsDescaling);
        }

        /// <summary>
        /// Nots the allowed descale or make when off.
        /// </summary>
        [Fact]
        public async Task Not_Allowed_Descale_Or_Make_When_Off()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();
            coffeeMachine.IsOn = false;

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/descale", null).ConfigureAwait(false);

            Assert.False(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            var body = JsonConvert.SerializeObject(new MakeCoffeeRequest());
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            response = await this.Client.PostAsync($"{this._baseUrl}/make", content).ConfigureAwait(false);

            Assert.False(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        /// <summary>
        /// Not the allowed descale or make when already performing the action.
        /// </summary>
        [Fact]
        public async Task Not_Allowed_Descale_Or_Make_When_Already_Performing()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.IsDescaling = true;
            coffeeMachine.DescaleState = State.Alert;

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/descale", null).ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            coffeeMachine.IsDescaling = false;
            coffeeMachine.DescaleState = State.Okay;
            coffeeMachine.IsMakingCoffee = true;

            var body = JsonConvert.SerializeObject(new MakeCoffeeRequest());
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            response = await this.Client.PostAsync($"{this._baseUrl}/make", content).ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        /// <summary>
        /// Nots the allowed making when descaling.
        /// </summary>
        [Fact]
        public async Task Not_Allowed_Making_When_Descaling()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.IsDescaling = true;
            coffeeMachine.DescaleState = State.Alert;

            var body = JsonConvert.SerializeObject(new MakeCoffeeRequest());
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/make", content).ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        /// <summary>
        /// Nots the allowed descale when making.
        /// </summary>
        [Fact]
        public async Task Not_Allowed_Descale_When_Making()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.IsMakingCoffee = true;
            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/descale", null).ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }


        /// <summary>
        /// Descalings the removes descale alert.
        /// </summary>
        [Fact]
        public async Task Descaling_Removes_DescaleAlert()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.IsDescaling = false;
            coffeeMachine.DescaleState = State.Alert;

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/descale", null).ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal(State.Okay, coffeeMachine.DescaleState);
            Assert.False(coffeeMachine.IsInAlertState);
        }

        /// <summary>
        /// Descalings the or making marks active.
        /// </summary>
        [Fact]
        public async Task Descaling_Or_Making_Marks_Active()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.IsMakingCoffee = true;
            HttpResponseMessage response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            var resultBody = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Active", resultBody["current_state"].ToString());

            coffeeMachine.Reset();

            coffeeMachine.IsDescaling = true;
            coffeeMachine.DescaleState = State.Alert;

            response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            resultBody = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Active", resultBody["current_state"].ToString());

            coffeeMachine.Reset();

            response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            resultBody = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Idle", resultBody["current_state"].ToString());
        }

        /// <summary>
        /// Alertings the state of the marks alert.
        /// </summary>
        [Fact]
        public async Task Alerting_Marks_AlertState()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.BeanFeedState = State.Alert;
            HttpResponseMessage response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            var resultBody = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Alert", resultBody["current_state"].ToString());


            coffeeMachine.WaterTrayState = State.Alert;

            response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            resultBody = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Alert", resultBody["current_state"].ToString());
        }

        /// <summary>
        /// Idlings the state of the marks idle.
        /// </summary>
        [Fact]
        public async Task Idling_Marks_IdleState()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            HttpResponseMessage response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            var resultBody = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Idle", resultBody["current_state"].ToString());
        }

        /// <summary>
        /// Offs the state of the marks off.
        /// </summary>
        [Fact]
        public async Task Off_Marks_OffState()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();
            coffeeMachine.IsOn = false;

            HttpResponseMessage response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            var resultBody = (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("Off", resultBody["current_state"].ToString());
        }

        /// <summary>
        /// Nots the allowed turning off during action.
        /// </summary>
        [Fact]
        public async Task Not_Allowed_TurningOff_During_Action()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.IsMakingCoffee = true;
            HttpResponseMessage response = await this.Client.DeleteAsync($"{this._baseUrl}/status/online").ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            coffeeMachine.Reset();

            coffeeMachine.IsDescaling = true;
            coffeeMachine.DescaleState = State.Alert;
            response = await this.Client.DeleteAsync($"{this._baseUrl}/status/online").ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Determines whether this instance [can turn off during alert].
        /// </summary>
        [Fact]
        public async Task Can_TurnOff_DuringAlert()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.WasteCoffeeState = State.Alert;
            HttpResponseMessage response = await this.Client.DeleteAsync($"{this._baseUrl}/status/online").ConfigureAwait(false);

            Assert.False(coffeeMachine.IsOn);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Determines whether this instance [can turn on when off].
        /// </summary>
        [Fact]
        public async Task Can_TurnOn_When_Off()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();
            coffeeMachine.IsOn = false;

            HttpResponseMessage response = await this.Client.PostAsync($"{this._baseUrl}/status/online", null).ConfigureAwait(false);

            Assert.True(coffeeMachine.IsOn);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Gets the status matches machine status.
        /// </summary>
        [Fact]
        public async Task GetStatus_Matches_MachineStatus()
        {
            using IServiceScope scope = this._factory.Services.CreateScope();
            var coffeeMachine = (CoffeeMachineTestStub)scope.ServiceProvider.GetRequiredService<ICoffeeMachine>();

            coffeeMachine.Reset();

            coffeeMachine.WasteCoffeeState = State.Alert;
            coffeeMachine.IsOn = true;

            HttpResponseMessage response = await this.Client.GetAsync($"{this._baseUrl}/status").ConfigureAwait(false);
            CoffeeMachineStatusResponse resultBody = JsonConvert.DeserializeObject<CoffeeMachineStatusResponse>(await response.Content.ReadAsStringAsync());

            Assert.True(response.IsSuccessStatusCode);

            Assert.Equal(coffeeMachine.IsOn, resultBody.IsOn);
            Assert.Equal(coffeeMachine.WasteCoffeeState, resultBody.WasteCoffeeState);
        }
    }
}
