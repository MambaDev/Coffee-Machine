const state = {
  machine: {
    online: false,
    status: null
  },
  timers: {
    statusTimer: null,
    statusTimerTiming: 30000
  }
};

/**
 * Shows a message to the user as a form of a notification.
 * @param {bool} error If its a error message or not.
 * @param {string} message The message to be shown.
 */
function notifyUser(error, message) {
  console.log(error, message);
}

/**
 * Based on the status of the coffee machine, change the button text to on or off.
 * @param {string} state The updated status of the coffee machine.
 */
function updateTurnOnOffButtonText(state) {
  const button = document.getElementById("button-on-off");

  if (state.toLowerCase() === "off") button.innerText = "Turn On";
  else button.innerText = "Turn Off";
}

/**
 * Tells the server to start descaling the coffee machine, the server will
 * respond with how long to wait, after waiting the client will update the
 * states, additionally if a error occurs, the user would be notified.
 */
async function descaleCoffeeMachine() {
  const button = document.getElementById("button-descale");
  button.disabled = true;

  const response = await window.api.coffee.descaleCoffeeMachine();

  if (!response.ok) {
    button.disabled = false;
    return notifyUser(true, response.data.message);
  }

  const refresh = async () => {
    button.disabled = false;
    await getCoffeeMachineStates();

    notifyUser(true, "Coffee machine descaled! ☕");
  };

  setTimeout(refresh, response.data.seconds_until_completion * 1000);
  notifyUser(false, response.data.message);
  await getCoffeeMachineStates();
}

/**
 * Tells the server to start making the coffee machine, the server will
 * respond with how long to wait, after waiting the client will update the
 * states, additionally if a error occurs, the user would be notified.
 */
async function makeCoffee() {
  const button = document.getElementById("button-make");
  button.disabled = true;

  const addMilk = document.getElementById("check-milk").checked;
  const shots = document.getElementById("coffee-shots").valueAsNumber;

  const response = await window.api.coffee.startMakingCoffee(shots, addMilk);

  if (!response.ok) {
    button.disabled = false;
    return notifyUser(true, response.data.message);
  }

  const refresh = async () => {
    button.disabled = false;
    await getCoffeeMachineStates();

    notifyUser(true, "Coffee should be ready! ☕");
  };

  setTimeout(refresh, response.data.seconds_until_completion * 1000);
  notifyUser(false, response.data.message);
  await getCoffeeMachineStates();
}

/**
 * Tells the server to turn the coffee machine off, this will also update the
 * stats and status within the site after the response, additionally if a error
 * occurs, the user would be notified.
 */
async function turnOffCoffeeMachine() {
  const response = await window.api.coffee.turnOffMachine();
  if (!response.ok) return notifyUser(true, response.data.message);
  state.machine.status = response.data;

  updateTurnOnOffButtonText(state.machine.status.current_state);
  updateCoffeeMachineStates(state.machine.status);
}

/**
 * Tells the server to turn the coffee machine on, this will also update the
 * stats and status within the site after the response, additionally if a error
 * occurs, the user would be notified.
 */
async function turnOnCoffeeMachine() {
  const response = await window.api.coffee.turnOnMachine();
  if (!response.ok) return notifyUser(true, response.data.message);
  state.machine.status = response.data;

  updateTurnOnOffButtonText(state.machine.status.current_state);
  updateCoffeeMachineStates(state.machine.status);
}

/**
 * Tells the server to get the coffee machines states, this will also update the
 * stats and status within the site after the response, additionally if a error
 * occurs, the user would be notified.
 */
async function getCoffeeMachineStates() {
  const response = await window.api.coffee.getStatusOfMachine();
  if (!response.ok) return notifyUser(true, response.data.message);

  state.machine.status = response.data;
  updateRelatedButtonSate(state.machine.status);

  if (state.machine.status.current_state.toLowerCase() !== "off")
    updateCoffeeMachineStates(state.machine.status);
}

/**
 * Handles the click case for turning off and on the coffee machine.
 * @param {event} event The click event object.
 */
async function handleOnOffButtonPressed(event) {
  if (state.machine.status.current_state.toLowerCase() === "off")
    return await turnOnCoffeeMachine();

  if (state.machine.status.current_state.toLowerCase() !== "active")
    return await turnOffCoffeeMachine();

  return notifyUser(
    true,
    "You cannot change the state of the coffee machine while its running"
  );
}

/**
 * Updates the current coffee machines overview state.
 * @param {string} status The current overview state of the coffee machine.
 */
function updateCoffeeMachineCurrentState(status, making, descaling) {
  const statusElement = document.getElementById("online-status");
  statusElement.innerText = status;

  if (making) statusElement.innerText += " - Making Coffee...";
  else if (descaling) statusElement.innerText += " - Descaling Machine...";

  if (status.toLowerCase() === "off" || status.toLowerCase() == "alert") {
    statusElement.classList.add("offline");
    statusElement.classList.remove("online");
  } else {
    statusElement.classList.add("online");
    statusElement.classList.remove("offline");
  }
}

/**
 * Updates the buttons state based on the coffee machines state.
 * @param {string} status The current overview state of the coffee machine.
 */
function updateRelatedButtonSate(states) {
  document.getElementById("button-on-off").disabled =
    states.is_descaling || states.is_making_coffee;

  document.getElementById("button-descale").disabled =
    states.is_descaling || states.descale_state.toLowerCase() === "okay";

  document.getElementById("button-make").disabled =
    states.current_state.toLowerCase() !== "idle";
}

function updateCoffeeMachineStates(states) {
  const table = document.getElementById("machine-status").firstElementChild;
  const children = table.children;

  children[0].innerText = states.water_level_state == "Okay" ? "✔" : "❌";
  children[1].innerText = states.water_tray_state === "Okay" ? "✔" : "❌";
  children[2].innerText = states.bean_feed_state === "Okay" ? "✔" : "❌";
  children[3].innerText = states.waste_coffee_state === "Okay" ? "✔" : "❌";
  children[4].innerText = states.descale_state === "Okay" ? "✔" : "❌";

  updateRelatedButtonSate(states);
  updateCoffeeMachineCurrentState(
    states.current_state,
    states.is_making_coffee,
    states.is_descaling
  );
}

/**
 * Until the api has a form of websocket implemented, the platform will be
 * required to poll status updates to ensure we are always aware of the current
 * state.
 */
function setupStatusUpdateTimer() {
  state.timers.statusTimer = setInterval(async () => {
    state.machine.status = (await window.api.coffee.getStatusOfMachine()).data;
    updateCoffeeMachineStates(state.machine.status);
  }, state.timers.statusTimerTiming);
}

/**
 * Setup the on screen buttons to allow performing there tasks. including:
 * turning off/on, make and descale.
 */
function setupButtonEvents() {
  document
    .getElementById("button-on-off")
    .addEventListener("click", handleOnOffButtonPressed);

  document.getElementById("button-make").addEventListener("click", makeCoffee);

  document
    .getElementById("button-descale")
    .addEventListener("click", descaleCoffeeMachine);
}

/**
 * Setups the api for use and basic setup.
 */
async function init() {
  await getCoffeeMachineStates();

  if (state.machine.status !== null)
    updateTurnOnOffButtonText(state.machine.status.current_state);

  setupStatusUpdateTimer();
  setupButtonEvents();
}

document.addEventListener("api-ready", init);
