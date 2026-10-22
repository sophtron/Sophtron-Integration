# Requirements

Python3, pin and python3-venv installed on linux using<br>
`sudo apt install python3 python3-pip python3-venv -y`

# Setting Up the Environment

Run the commands below:

1. `python3 -m venv .venv`
2. `source .venv/bin/activate`
3. `pip install -r requirements.txt`

# Authenticate and Run

The example reads `SophtronApiUserId` and `SophtronApiUserSecret` from the
environment first. If either variable is not set, it loads values from a
`.env` file next to `try.py`. Existing environment variables take precedence.

For unattended runs, create `python/.env` with:

```dotenv
SophtronApiUserId=your_sophtron_user_id
SophtronApiUserSecret=your_sophtron_user_secret
```

The repository ignores `python/.env`; do not commit real credentials. Install
the dependencies with `pip install -r requirements.txt`, then run `python try.py`
from the `python` directory.

Alternatively, export both variables in the shell before running the script.
For example, use `read -rsp` for the secret to avoid displaying it as you type.

## Connecting a bank

Run `python try.py` from this directory. The example searches for Bank of
America, asks for the bank login in the terminal, and handles common MFA
challenges returned by the API. The password, PIN, security answers, and
verification codes are not stored in the script. The example asks before
submitting bank credentials to Sophtron and prints transaction details to the
terminal only if you choose to fetch them.

This terminal example cannot display CAPTCHA challenges. If the connection
requires one, use Sophtron's supported widget or contact Sophtron for the
recommended flow. Never commit API or bank credentials, and do not run this
example on a shared or untrusted machine.
