const options = {
  timeout: 5000,
  classes: ["snackbar-active"]
};

/**
 * returns the current active container for the snacks, otherwise null if the snackbar container
 * does not exist.
 */
function getSnackbarContainer() {
  return document.querySelector("#snackbar-container");
}

/**
 * Creates a snackbar container if one does not already exist.
 */
function createSnackbarContainer() {
  let container = getSnackbarContainer();
  if (container != null) return container;

  container = document.createElement("div");
  container.id = "snackbar-container";

  document.body.appendChild(container);
  return container;
}

/**
 * Fills the snackbar div with the content required to have a valid snackbar.
 * @param {HTMLElement} snackbarElement The element that will be filled with the content.
 * @param {string} message The message that will be displayed.
 */
function fillSnackbar(snackbarElement, message, error) {
  snackbarElement.innerHTML = `<span>${message}</span>`;
  snackbarElement.classList.add("snackbar");
}

/**
 * Hides the snackbar element by adjusting css to transfer it out of the page.
 * @param {HTTPElement} snackbarElement The snackbar element
 */
function hideSnackbar(snackbarElement) {
  snackbarElement.classList.remove(options.class);

  setTimeout(() => {
    snackbarElement.style.transform = null;
    snackbarElement.style.opacity = "0";
  }, 150);
}

/**
 * Shows the snackbar element by adjusting the css to transfer it into the page.
 * @param {HTMLElement} snackbarElement The snackbar element to show.
 * @param {bool} error If its a error or not.
 */
function showSnackbar(snackbarElement, error) {
  snackbarElement.classList.add(...options.classes);

  if (error) snackbarElement.classList.add("snackbar-error");

  setTimeout(() => {
    snackbarElement.style.transform = "none";
  }, 150);

  setTimeout(() => {
    snackbarElement.style.opacity = "1";
  }, 200);
}

/**
 * Shows a given snack bar based on the message
 * @param {bool} error If its a error or not.
 * @param {string} message The message that is going to be shown on the bar.
 */
function showSnack(error, message) {
  const container = getSnackbarContainer() || createSnackbarContainer();
  let snackbar = document.createElement("div");

  if (container.childElementCount > 0) {
    snackbar = container.insertBefore(snackbar, container.firstElementChild);
  } else {
    snackbar = container.appendChild(snackbar);
  }

  fillSnackbar(snackbar, message);
  showSnackbar(snackbar, error);

  // setup the time outs to hide the snackbar and then follow up by removing it.
  setTimeout(hideSnackbar.bind(null, snackbar), options.timeout);
  setTimeout(() => snackbar.remove(), options.timeout + 400);
}
