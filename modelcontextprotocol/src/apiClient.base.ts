import { apiEndpoint } from "./consts";
import { buildSophtronAuthCode } from "./utils";
import HttpClient from "./httpClient";

export default class SophtronBaseClient {
  apiConfig: any;
  httpClient: HttpClient;

  constructor() {
    this.httpClient = new HttpClient();
  }

  getAuthHeaders(method: string, path: string) {
    return {
      Authorization: buildSophtronAuthCode(
        method,
        path,
        process.env.SOPHTRON_USER_ID || process.env.SophtronApiUserId || '',
        process.env.SOPHTRON_ACCESS_KEY || process.env.SophtronApiUserSecret || '',
      ),
    };
  }

  async post(path: string, data?: any) {
    const authHeader = this.getAuthHeaders("post", path);
    return await this.httpClient.post(apiEndpoint + path, data, authHeader);
  }

  async get(path: string) {
    const authHeader = this.getAuthHeaders("get", path);
    return await this.httpClient.get(apiEndpoint + path, authHeader);
  }

  async put(path: string, data: any) {
    const authHeader = this.getAuthHeaders("put", path);
    return await this.httpClient.put(apiEndpoint + path, data, authHeader);
  }

  async del(path: string) {
    const authHeader = this.getAuthHeaders("delete", path);
    return await this.httpClient.del(apiEndpoint + path, authHeader);
  }
}
