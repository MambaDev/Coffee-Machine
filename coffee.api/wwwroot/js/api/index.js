import Infrastructure from "./infrastructure.js";
import Coffee from "./coffee.js";

export default class Api {
  /**
   *  Creates a new instance of the core coffee api.
   * @param {string} apiUri The root url for the api endpoints.
   */
  constructor(apiUri) {
    this.apiUri = apiUri;

    this.infrastructure = new Infrastructure(this.apiUri, "infrastructure");
    this.coffee = new Coffee(this.apiUri, "coffee");
  }
}
