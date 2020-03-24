import Api from "./api/index.js";

const state = {
  online: false,
  api: null
};

/**
 * Displays to the user that the given api is online or not.
 * @param {bool} online Is the given coffee machine online or not?
 */
function markPageAsOnline(online = false) {
  debugger;
  const onlineElement = document.getElementById("online-status");
  onlineElement.innerHTML = online ? "Online!" : "Offline";

  if (online) {
    onlineElement.classList.add("site-online");
    onlineElement.classList.remove("site-offline");
  } else {
    onlineElement.classList.remove("site-online");
    onlineElement.classList.add("site-offline");
  }
}

/**
 * Setups the api for use and basic setup.
 */
async function init() {
  state.api = new Api("http://localhost:8080/api");

  const health = await state.api.infrastructure.health();
  markPageAsOnline(health.online);
}

window.addEventListener("load", init);
