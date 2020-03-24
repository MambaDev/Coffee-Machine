export default class Endpoint {
  /**
   * Creates a new instance of the endpoint class to provide single funnel of requests.
   * @param {string} apiUrl The api root url.
   * @param {string} path  The path of the given endpoint.
   */
  constructor(apiUrl, path) {
    this.apiUrl = apiUrl;
    this.path = path;
  }

  /**
   * A single point to ensure that if a request does not support a body, to not send the body.
   * @param {string} method The method/kind of request being performed.
   * @param {any} body The given body that could be sent.
   */
  static getBodyContent(method, body) {
    if (body == null || Object.keys(body).length === 0) return undefined;
    return ["get", "head"].includes(method.toLowerCase())
      ? undefined
      : JSON.stringify(body);
  }

  /**
   * Makes a given api call to the current api url and returns the response as json.
   * @param {object} request object containing the path, body and method.
   */
  async apiCall({ path, body, method }) {
    const response = await fetch(`${this.apiUrl}/${path}`, {
      method: method,
      body: Endpoint.getBodyContent(method, body)
    });

    const { status } = response;

    if (response.ok) {
      const content = await response.json();
      return { content, status };
    }

    // if for any reason we did fail the request, lets go and ensure we can get anything from it. If
    // for any reason we fail to parse the given text to JSON, just default with a simple response.

    try {
      const text = await response.text();
      const json = text == null || text === "" ? {} : JSON.parse(text);
      return Object.assign({ headers: response.headers, status }, json);
    } catch (error) {
      return {
        error: "unknown",
        description: response.statusText,
        status: response.status
      };
    }
  }
}
