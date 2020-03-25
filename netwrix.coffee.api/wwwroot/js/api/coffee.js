import Endpoint from "./endpoint.js";

export default class Coffee extends Endpoint {
  /**
   * Gets the current status of the different parts of the coffee machine.
   */
  async status() {
    return this.apiCall({ path: `${this.path}/status`, method: "get" });
  }

  /**
   * Attempts to turn the given coffee machine on.
   */
  async turnOn() {
    return this.apiCall({ path: `${this.path}/status/online`, method: "post" });
  }

  /**
   * Attempts to turn the given coffee machine off.
   */
  async turnOff() {
    return this.apiCall({
      path: `${this.path}/status/offline`,
      method: "post"
    });
  }
}
