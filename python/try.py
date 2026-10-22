import os
import json
import time
from pathlib import Path
from datetime import date, timedelta
from getpass import getpass

from dotenv import load_dotenv

from apiClient import SophtronApiClient


def parse_options(value):
    if isinstance(value, list):
        return value
    try:
        parsed = json.loads(value)
    except (TypeError, json.JSONDecodeError):
        return [value]
    return parsed if isinstance(parsed, list) else [parsed]


def wait_for_job(client, job_id, timeout_seconds=600):
    deadline = time.monotonic() + timeout_seconds
    handled_challenges = set()

    while time.monotonic() < deadline:
        job = client.get_job_information_by_id(job_id)
        if job.get("SuccessFlag") is True or job.get("LastStatus") == "AccountsReady":
            return job
        if job.get("SuccessFlag") is False and job.get("LastStatus") != "Completed":
            raise RuntimeError(
                "Sophtron reported that the connection job failed "
                f"(status: {job.get('LastStatus', 'unknown')})."
            )

        challenge_key = json.dumps(
            [
                job.get("LastStep"),
                job.get("LastStatus"),
                job.get("SecurityQuestion"),
                job.get("TokenMethod"),
                job.get("TokenSentFlag"),
                job.get("TokenRead"),
                job.get("CaptchaImage"),
            ],
            sort_keys=True,
            default=str,
        )
        if challenge_key not in handled_challenges:
            if job.get("SecurityQuestion"):
                questions = parse_options(job["SecurityQuestion"])
                answers = [
                    getpass(f"Bank security question '{question}': ")
                    for question in questions
                ]
                client.update_job_security_answer(job_id, json.dumps(answers))
                handled_challenges.add(challenge_key)
                continue

            if job.get("TokenMethod"):
                choices = parse_options(job["TokenMethod"])
                for index, choice in enumerate(choices, start=1):
                    print(f"{index}. {choice}")
                selection = input("Choose where to receive the verification code: ")
                if not selection.isdigit() or not 1 <= int(selection) <= len(choices):
                    raise ValueError("Choose a valid verification method number.")
                client.update_job_token_choice(job_id, choices[int(selection) - 1])
                handled_challenges.add(challenge_key)
                continue

            if job.get("TokenSentFlag") is True:
                token = getpass("Enter the bank verification code: ")
                client.update_job_token_input(job_id, token)
                handled_challenges.add(challenge_key)
                continue

            if job.get("TokenRead") is not None:
                verified = input(
                    "Complete the bank's phone verification, then type yes: "
                ).strip().lower()
                if verified != "yes":
                    raise RuntimeError("Phone verification was not confirmed.")
                client.update_job_token_phone_verify(job_id, True)
                handled_challenges.add(challenge_key)
                continue

            if job.get("CaptchaImage"):
                raise RuntimeError(
                    "This connection requires a CAPTCHA challenge that this "
                    "terminal example cannot display. Use Sophtron's supported "
                    "widget or contact Sophtron for the recommended flow."
                )

        time.sleep(5)

    raise TimeoutError(
        "Timed out waiting for Sophtron to complete the connection job."
    )


def choose_item(items, prompt):
    if not items:
        raise RuntimeError("Sophtron returned no matching items.")
    for index, item in enumerate(items, start=1):
        print(f"{index}. {item.get('InstitutionName') or item.get('AccountName')}")
    selection = input(prompt).strip()
    if not selection.isdigit() or not 1 <= int(selection) <= len(items):
        raise ValueError("Choose a valid item number.")
    return items[int(selection) - 1]


def main():
    load_dotenv(Path(__file__).resolve().with_name(".env"))
    user_id = os.getenv("SophtronApiUserId")
    user_secret = os.getenv("SophtronApiUserSecret")
    if not user_id or not user_secret:
        raise RuntimeError(
            "Set SophtronApiUserId and SophtronApiUserSecret in the environment "
            "or in python/.env."
        )

    client = SophtronApiClient(
        "https://api.sophtron.com/api/",
        user_id,
        user_secret,
    )
    institutions = client.getInstitutionByName("Bank of America")
    institution = choose_item(institutions, "Select your Bank of America institution: ")

    bank_username = input("Bank of America username: ")
    bank_password = getpass("Bank of America password: ")
    bank_pin = getpass("Bank PIN (press Enter if not required): ")
    confirmation = input(
        "This will send the bank credentials to Sophtron over HTTPS. Continue? [y/N] "
    ).strip().lower()
    if confirmation != "y":
        print("Cancelled; no bank credentials were submitted.")
        return

    tracker = client.create_user_institution(
        user_id,
        institution["InstitutionID"],
        bank_username,
        bank_password,
        bank_pin,
    )
    job_id = tracker["JobID"]
    user_institution_id = tracker["UserInstitutionID"]
    del bank_password, bank_pin

    print("Waiting for Sophtron to connect to the bank. MFA may be requested.")
    wait_for_job(client, job_id)
    accounts = client.get_user_institution_accounts(user_institution_id)
    if not accounts:
        print("The connection completed, but Sophtron returned no accounts.")
        return

    account = choose_item(accounts, "Select an account: ")
    account_number = str(account.get("AccountNumber") or "")
    if account_number:
        print(f"Selected account ending in {account_number[-4:]}.")

    if input("Fetch transactions from the last 30 days? [y/N] ").strip().lower() != "y":
        return

    refresh_job = client.refresh_user_institution_account(account["AccountID"])
    wait_for_job(client, refresh_job["JobID"])
    transactions = client.get_transactions_by_transaction_date(
        account["AccountID"],
        date.today() - timedelta(days=30),
        date.today(),
    )
    print(f"Retrieved {len(transactions)} transactions.")
    for transaction in transactions:
        print(
            transaction.get("TransactionDate") or transaction.get("Date"),
            transaction.get("Description") or transaction.get("Merchant"),
            transaction.get("Amount"),
            transaction.get("Currency"),
        )


if __name__ == "__main__":
    main()
