#!/usr/bin/env node

import express from "express";
import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
// import { SSEServerTransport } from "@modelcontextprotocol/sdk/server/sse";
import { useCallToolHandler, useListToolsHandler } from "./tools";

const server = new Server(
  {
    name: "sophtron/sophtron-mcp-server",
    version: "0.1.0",
  },
  {
    capabilities: {
      resources: {},
      tools: {},
    },
  },
);

useListToolsHandler(server)
useCallToolHandler(server)

async function stdio() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  process.on("SIGINT", async () => {
    await server.close();
    process.exit(0);
  });
}



let main = stdio
// main = async function sse(){
//   const app = express();
//   let transport: SSEServerTransport | null = null;
//   app.get("/sse", (req, res) => {
//     console.log('/sse')
//     transport = new SSEServerTransport("/messages", res);
//     server.connect(transport);
//   });
//   app.post("/messages", (req, res) => {
//     console.log('/messages')
//     if (transport) {
//       transport.handlePostMessage(req, res);
//     }
//   });
//   app.listen(3000);
// }

main().catch((error) => {
  console.error("Server error:", error);
  process.exit(1);
});