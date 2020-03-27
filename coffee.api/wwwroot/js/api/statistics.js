import Endpoint from "./endpoint.js";

export default class Statistics extends Endpoint {
  /**
   * Gets current base coffee machine statistics related to the coffee making.
   */
  async getCoffeeMachineStatistics() {
    return this.apiCall({
      path: `${this.path}`,
      method: "get"
    });
  }
}
