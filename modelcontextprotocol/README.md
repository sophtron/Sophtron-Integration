# Sophtron MCP Server
Connect to your bank, credit card, utilities accounts to retrieve account balances and transactions


## Setup

### Usage with Claude Desktop
To use this with Claude Desktop, add the following to your `claude_desktop_config.json`:

#### NPX

```json
{
  "mcpServers": {
    "sophtron": {
      "command": "npx",
      "args": [
        "-y",
        "@sophtron/server-sophtron"
      ],
      "env": {
        "SOPHTRON_USER_ID": "<YOUR_USER_ID>",
        "SOPHTRON_ACCESS_KEY": "<YOUR_ACCESS_KEY>"
      }
    }
  }
}
```

