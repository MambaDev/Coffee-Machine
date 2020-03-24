import Endpoint from "./endpoint.js";

export default class Coffee extends Endpoint {
  /**
   * Gets the current status of the different parts of the coffee machine.
   */
  async status() {
    return this.apiCall({ path: `${this.path}/status`, method: "get" });
  }
}
