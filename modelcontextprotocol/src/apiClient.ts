import SophtronBaseClient from "./apiClient.base";
import { logError } from "./utils";

const convertJobTypes = (jobTypes: string[]) => jobTypes.join("|");

export class SophtronClient extends SophtronBaseClient {

  async getCustomerByUniqueName(uniqueName: string) {
    const arr = await this.get(`/v2/customers?uniqueID=${uniqueName}`);
    return arr?.[0];
  }

  // createCustomer(uniqueName: string) {
  //   return this.post("/v2/customers", {
  //     UniqueID: uniqueName,
  //     Source: `MCP_SERVER`,
  //     Name: "MCP_SERVER_Customer",
  //   });
  // }

  // deleteCustomer(customerId: string) {
  //   return this.del(`/v2/customers/${customerId}`);
  // }

  // createMember(
  //   customerId: string,
  //   jobTypes: string[],
  //   username: string,
  //   password: string,
  //   institutionId: string,
  // ) {
  //   return this.post(
  //     `/v2/customers/${customerId}/members/${convertJobTypes(jobTypes)}`,
  //     {
  //       UserName: username,
  //       Password: password,
  //       InstitutionID: institutionId,
  //     }
  //   );
  // }

  // updateMember(
  //   customerId: string,
  //   memberId: string,
  //   jobTypes: string[],
  //   username: string,
  //   password: string,
  // ) {
  //   return this.put(
  //     `/v2/customers/${customerId}/members/${memberId}/${convertJobTypes(jobTypes)}`,
  //     {
  //       UserName: username,
  //       Password: password,
  //     }
  //   );
  // }

  // deleteMember(customerId: string, memberId: string) {
  //   return this.del(`/v2/customers/${customerId}/members/${memberId}`);
  // }

  // getCustomer(customerId: string) {
  //   return this.get(`v2/customers/${customerId}`)
  // }
  getCustomers() {
    return this.get(`v2/customers`)
  }
  // getMember(customerId: string, memberId: string) {
  //   return this.get(`v2/customers/${customerId}/members/${memberId}`)
  // }
  getMembers(customerId: string) {
    return this.get(`v2/customers/${customerId}/members`)
  }
  // getMemberAccounts(customerId: string, memberId: string) {
  //   return this.get(`v2/customers/${customerId}/members/${memberId}/accounts`)
  // }
  // getAccount(customerId: string, accountId: string) {
  //   return this.get(`v2/customers/${customerId}/accounts/${accountId}`)
  // }
  // getAccounts(customerId: string) {
  //   return this.get(`v2/customers/${customerId}/accounts`)
  // }
  getTransactions(customerId: string, accountId: string, startTime: Date, endTime: Date) {
    const path = `v2/customers/${customerId}/accounts/${accountId}/transactions?startDate=${startTime.toISOString().substring(0, 10)}&endDate=${endTime.toISOString().substring(0, 10)}`
    return this.get(path)
  }

  getIdentityV3(customerId: string, memberId: string) {
    return this.get(`v3/Customers/${customerId}/Members/${memberId}/identity`)
  }
  getAccountV3(customerId: string, memberId: string, accountId: string) {
    return this.get(`v3/customers/${customerId}/Members/${memberId}/accounts/${accountId}`)
  }
  getMemberAccountsV3(customerId: string, memberId: string) {
    return this.get(`v3/customers/${customerId}/Members/${memberId}/accounts`)
  }
  getAccountsV3(customerId: string) {
    return this.get(`v3/customers/${customerId}/accounts`)
  }
  getTransactionsV3(customerId: string, accountId: string, startTime: Date, endTime: Date) {
    const path = `v3/customers/${customerId}/accounts/${accountId}/transactions?startDate=${startTime.toISOString().substring(0, 10)}&endDate=${endTime.toISOString().substring(0, 10)}`
    return this.get(path)
  }
  // getJobInfo(jobId: string) {
  //   return this.get(`/v2/job/${jobId}`);
  // }

  // answerJobMfa(jobId: string, mfaType: any, answer: any) {
  //   return this.put(`/v2/job/${jobId}/challenge/${mfaType}`, {
  //     AnswerText: answer,
  //   });
  // }
}
