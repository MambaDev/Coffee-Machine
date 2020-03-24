import Infrastructure from "./infrastructure.js";

export default class Api {
  /**
   *  Creates a new instance of the core coffee api.
   * @param {string} apiUri The root url for the api endpoints.
   */
  constructor(apiUri) {
    this.apiUri = apiUri;

    this.infrastructure = new Infrastructure(this.apiUri, "infrastructure");
  }
}
