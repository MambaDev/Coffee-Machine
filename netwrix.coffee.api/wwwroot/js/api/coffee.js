import Endpoint from "./endpoint.js";

export default class Coffee extends Endpoint {
  /**
   * Gets the current status of the different parts of the coffee machine.
   */
  async getStatusOfMachine() {
    return this.apiCall({
      path: `${this.path}/status`,
      method: "get"
    });
  }

  /**
   * Attempts to turn the given coffee machine on.
   */
  async turnOnMachine() {
    return this.apiCall({
      path: `${this.path}/status/online`,
      method: "post"
    });
  }

  /**
   * Attempts to turn the given coffee machine off.
   */
  async turnOffMachine() {
    return this.apiCall({
      path: `${this.path}/status/online`,
      method: "delete"
    });
  }

  /**
   * Starts the making coffee process with number of espresso shots and if we are adding milk.
   * @param {number} numberOfShots The number of shots to add.
   * @param {boolean} addMilk If we are adding milk or not.
   */
  async startMakingCoffee(numberOfShots = 0, addMilk = true) {
    return this.apiCall({
      path: `${this.path}/make`,
      method: "post",
      body: {
        number_espresso_shots: numberOfShots,
        add_milk: addMilk
      }
    });
  }

  /**
   * Starts the process of descaling the coffee machine.
   */
  async descaleCoffeeMachine() {
    return this.apiCall({
      path: `${this.path}/descale`,
      method: "post"
    });
  }
}
