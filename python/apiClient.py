import requests
import base64
import hmac
import hashlib
from datetime import date

def sophtron_auth_code(user_id: str, access_key: str, url: str, http_method: str):
    auth_path = url[url.rfind('/'):].lower()
    plain_key = '{}\n{}'.format(http_method.upper(), auth_path)
    access_key_bytes = base64.b64decode(access_key)
    signature = hmac.new(key=access_key_bytes,
                         msg=bytes(plain_key, 'ascii'),
                         digestmod=hashlib.sha256).digest()
    sig_b64_str = base64.b64encode(signature).decode('ascii')
    auth_str = 'FIApiAUTH:{}:{}:{}'.format(user_id, sig_b64_str, auth_path)
    return auth_str


class SophtronApiClient(object):
    def __init__(self, base_url, user_id, access_key):
        self.base_url = base_url
        self.user_id = user_id
        self.access_key = access_key

    def _get_headers(self, url, http_method) -> dict:
        return {
            'Authorization': sophtron_auth_code(self.user_id, self.access_key, url, http_method)
        }

    def _post(self, url_path, payload_data):
        url = '{}{}'.format(self.base_url, url_path)
        headers = self._get_headers(url, 'POST')
        if isinstance(payload_data, str):
            response = requests.post(
                url, headers=headers, data=payload_data, timeout=30
            )
        else:
            response = requests.post(
                url, headers=headers, json=payload_data, timeout=30
            )
        response.raise_for_status()
        return response.json()

    def getInstitutionByName(self, institution_name: str):
        return self._post('Institution/GetInstitutionByName',
                          {'InstitutionName': institution_name})

    def get_user_institutions_by_user(self, user_id: str):
        return self._post(
            'UserInstitution/GetUserInstitutionsByUser',
            {'UserID': user_id},
        )

    def create_user_institution(
        self,
        user_id: str,
        institution_id: str,
        username: str,
        password: str,
        pin: str = '',
    ):
        return self._post(
            'UserInstitution/CreateUserInstitution',
            {
                'UserID': user_id,
                'InstitutionID': institution_id,
                'UserName': username,
                'Password': password,
                'PIN': pin,
            },
        )

    def retry_adding_user_institution(self, user_institution_id: str):
        return self._post(
            'UserInstitution/RetryAddingUserInstitution',
            {'UserInstitutionID': user_institution_id},
        )

    def get_job_information_by_id(self, job_id: str):
        return self._post('Job/GetJobInformationByID', {'JobID': job_id})

    def update_job_security_answer(self, job_id: str, security_answer: str):
        return self._post(
            'Job/UpdateJobSecurityAnswer',
            {'JobID': job_id, 'SecurityAnswer': security_answer},
        )

    def update_job_captcha_input(self, job_id: str, captcha_input: str):
        return self._post(
            'Job/UpdateJobCaptcha',
            {'JobID': job_id, 'CaptchaInput': captcha_input},
        )

    def update_job_token_choice(self, job_id: str, token_choice: str):
        return self._post(
            'Job/UpdateJobTokenInput',
            {
                'JobID': job_id,
                'TokenChoice': token_choice,
                'TokenInput': None,
                'VerifyPhoneFlag': None,
            },
        )

    def update_job_token_input(self, job_id: str, token_input: str):
        return self._post(
            'Job/UpdateJobTokenInput',
            {
                'JobID': job_id,
                'TokenChoice': None,
                'TokenInput': token_input,
                'VerifyPhoneFlag': None,
            },
        )

    def update_job_token_phone_verify(self, job_id: str, verify_phone: bool):
        return self._post(
            'Job/UpdateJobTokenInput',
            {
                'JobID': job_id,
                'TokenChoice': None,
                'TokenInput': None,
                'VerifyPhoneFlag': verify_phone,
            },
        )

    def get_user_institution_accounts(self, user_institution_id: str):
        return self._post('UserInstitution/GetUserInstitutionAccounts',
                          {'UserInstitutionID': user_institution_id})

    def refresh_user_institution_account(self, account_id: str):
        return self._post(
            'UserInstitutionAccount/RefreshUserInstitutionAccount',
            {'AccountID': account_id},
        )

    def get_transactions_by_transaction_date(
        self,
        account_id: str,
        start_date: date,
        end_date: date,
    ):
        return self._post(
            'Transaction/GetTransactionsByTransactionDate',
            {
                'AccountID': account_id,
                'StartDate': start_date.isoformat(),
                'EndDate': end_date.isoformat(),
            },
        )