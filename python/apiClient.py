import os
import json
import requests
import base64
import hmac
import hashlib

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

    def _post(self, url_path, payload_data: [str, object]):
        url = '{}{}'.format(self.base_url, url_path)
        headers = self._get_headers(url, 'POST')
        headers.update({'Content-Type': 'application/json'})
        payload = payload_data if isinstance(payload_data, str) else json.dumps(payload_data)
        resp = requests.request('POST', url, headers=headers, data=payload, timeout=30)
        return json.loads(resp.text)

    def getInstitutionByName(self, institution_name: str):
        return self._post('Institution/GetInstitutionByName',
                          {'InstitutionName': institution_name})

    def get_user_institution_accounts(self, user_institution_id: str):
        return self._post('UserInstitution/GetUserInstitutionAccounts',
                          {'UserInstitutionID': user_institution_id})