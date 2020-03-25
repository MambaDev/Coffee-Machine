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

  /**
   * Starts the making coffee process with number of espresso shots and if we are adding milk.
   * @param {number} numberOfShots The number of shots to add.
   * @param {*} addMilk If we are adding milk or not.
   */
  async makeCoffee(numberOfShots = 0, addMilk = true) {
    const body = {
      number_espresso_shots: numberOfShots,
      add_milk: addMilk
    };

    return this.apiCall({ path: `${this.path}/make`, method: "post", body });
  }
}
