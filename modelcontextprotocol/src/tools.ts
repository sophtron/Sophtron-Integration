import {
  Tool,
  ToolSchema,
  ListToolsRequestSchema,
  CallToolRequestSchema
} from "@modelcontextprotocol/sdk/types.js";
import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { z } from "zod";
import { zodToJsonSchema } from "zod-to-json-schema";
import { SophtronClient } from "./apiClient"

const sophtron_client = new SophtronClient();

const ToolInputSchema = ToolSchema.shape.inputSchema;
type ToolInput = z.infer<typeof ToolInputSchema>;

const tools: {
  [key: string] : {
    description: string,
    schema: z.Schema,
    fn: Function
  }
} = {
  GETCUSTOMERS: {
    description: "Get customers that belong to this client",
    schema: z.object({
    }),
    fn: async function(zodResult: any){
      const ret = sophtron_client.getCustomers()
      return ret
    }
  },
  GETIDENTITY : {
    description: "Get identity from a connected member",
    schema: z.object({
      customerId: z.string().describe("Customer Id"),
      memberId: z.string().describe("Member Id"),
    }),
    fn: async function(zodResult: any){
      const ret = await sophtron_client.getIdentityV3(zodResult.customerId, zodResult.memberId)
      return ret
    }
  },
  GETMEMBERS: {
    description: "Get members that belong to a customer",
    schema: z.object({
      customerId: z.string().describe("Customer Id"),
    }),
    fn: function(zodResult: any){
      const ret = sophtron_client.getMembers(zodResult.customerId)
      return ret
    }
  },
  GETACCOUNTS: {
    description: "Get accounts that belong to a member",
    schema: z.object({
      customerId: z.string().describe("Customer Id"),
      memberId: z.string().describe("Member Id"),
    }),
    fn: function(zodResult: any){
      const ret = sophtron_client.getMemberAccountsV3(zodResult.customerId, zodResult.memberId)
      return ret
    }
  },
  GETACCOUNT: {
    description: "Get account that belong to a member by account id",
    schema: z.object({
      customerId: z.string().describe("Customer Id"),
      memberId: z.string().describe("Member Id"),
      accountId: z.string().describe("Account Id"),
    }),
    fn: function(zodResult: any){
      const ret = sophtron_client.getAccountV3(zodResult.customerId, zodResult.memberId, zodResult.accountId)
      return ret
    }
  },
  GETTRANSACTIONS: {
    description: "Get recent transactions that belong to an account",
    schema: z.object({
      customerId: z.string().describe("Customer Id"),
      accountId: z.string().describe("Account Id"),
    }),
    fn: function(zodResult: any){
      const start = new Date("2020-05-05");
      const ret = sophtron_client.getTransactionsV3(zodResult.customerId, zodResult.accountId, start, new Date())
      return ret
    }
  }
}

export function useListToolsHandler(server: Server) {

  server.setRequestHandler(ListToolsRequestSchema, async () => {
    return {
      tools: Object.entries(tools).map(item => ({
        name: item[0],
        description: item[1].description,
        inputSchema: zodToJsonSchema(item[1].schema)
      }))
    };
  });
}

export function useCallToolHandler(server: Server){
  server.setRequestHandler(CallToolRequestSchema, async (request) => {
    const { name, arguments: args } = request.params;
    const handler = tools[name.toUpperCase()];
    if(handler){
      const validatedArgs = handler.schema.parse(args)
      const ret = await handler.fn(validatedArgs)
      return {
        content: [
          {
            type: 'text',
            text: JSON.stringify(ret)
          }
        ]
      }
    }
    return {
      content: []
    }
  });
}