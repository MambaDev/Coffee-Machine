import Api from "./api/index.js";

const state = {
  api: null,

  machine: {
    online: false,
    status: null
  },
  timers: {
    statusTimer: null,
    statusTimerTiming: 3000
  }
};

/**
 * Displays to the user that the given api is online or not.
 * @param {bool} online Is the given coffee machine online or not?
 */
function markPageAsOnline(online = false) {
  const onlineElement = document.getElementById("online-status");
  onlineElement.innerText = online ? "Online" : "Offline";

  if (online) {
    onlineElement.classList.add("site-online");
    onlineElement.classList.remove("site-offline");
  } else {
    onlineElement.classList.remove("site-online");
    onlineElement.classList.add("site-offline");
  }
}

/**
 * Until the api has a form of websocket implemented, the platform will be required to poll status
 * updates to ensure we are always aware of the current state.
 */
function setupStatusUpdateTimer() {
  state.timers.statusTimer = setInterval(async () => {
    state.machine.status = await state.api.coffee.status();
  }, state.timers.statusTimerTiming);
}

/**
 * Setups the api for use and basic setup.
 */
async function init() {
  state.api = new Api("/api");

  const health = await state.api.infrastructure.health();
  markPageAsOnline(health.online);

  state.machine.status = await state.api.coffee.status();
  setupStatusUpdateTimer();
}

window.addEventListener("load", init);
