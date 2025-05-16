# Sophtron MCP Server

## Setup

### Usage with Claude Desktop
To use this with Claude Desktop, add the following to your `claude_desktop_config.json`:

#### Docker
```json
{
  "mcpServers": {
    "sophtron": {
      "command": "docker",
      "args": [
        "run",
        "-i",
        "--rm",
        "-e",
        "SOPHTRON_USER_ID",
        "SOPHTRON_ACCESS_KEY",
        "mcp/sophtron"
      ],
      "env": {
        "SOPHTRON_USER_ID": "<YOUR_USER_ID>",
        "SOPHTRON_ACCESS_KEY": "<YOUR_ACCESS_KEY>"
      }
    }
  }
}
```

#### NPX

```json
{
  "mcpServers": {
    "sophtron": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-sophtron"
      ],
      "env": {
        "SOPHTRON_USER_ID": "<YOUR_USER_ID>",
        "SOPHTRON_ACCESS_KEY": "<YOUR_ACCESS_KEY>"
      }
    }
  }
}
```

#### Docker

```json
{
  "mcp": {
    "inputs": [
      {
        "type": "promptString",
        "id": "sophtron_user_id",
        "description": "Sophtron UserId",
        "password": true
      },
      {
        "type": "promptString",
        "id": "sophtron_access_key",
        "description": "Sophtron Access Key",
        "password": true
      }
    ],
    "servers": {
      "sophtron": {
        "command": "docker",
        "args": ["run", "-i", "--rm", "mcp/sophtron"],
        "env": {
          "SOPHTRON_USER_ID": "${input:sophtron_user_id}",
          "SOPHTRON_ACCESS_KEY": "${input:sophtron_access_key}"
        }
      }
    }
  }
}
```

#### NPX

```json
{
  "mcp": {
    "inputs": [
      {
        "type": "promptString",
        "id": "sophtron_user_id",
        "description": "Sophtron UserId",
        "password": true
      },
      {
        "type": "promptString",
        "id": "sophtron_access_key",
        "description": "Sophtron Access Key",
        "password": true
      }
    ],
    "servers": {
      "sophtron": {
        "command": "npx",
        "args": [
          "-y",
          "@modelcontextprotocol/server-sophtron"
        ],
        "env": {
          "SOPHTRON_USER_ID": "${input:sophtron_user_id}",
          "SOPHTRON_ACCESS_KEY": "${input:sophtron_access_key}"
        }
      }
    }
  }
}
```

## Build

Docker build:

```bash
docker build -t mcp/sophtron -f ./Dockerfile .
```

