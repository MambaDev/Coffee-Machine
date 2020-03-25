import Endpoint from "./endpoint.js";

export default class Infrastructure extends Endpoint {
  /**
   * Returns fi the master instance is online or not.
   */
  async health() {
    return this.apiCall({ path: this.path, method: "get" });
  }
}
