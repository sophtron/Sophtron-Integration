import os
import json
import requests
from .auth import sophtron_auth_code


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

    def get_user_institution_accounts(self, user_institution_id: str):
        return self._post('UserInstitution/GetUserInstitutionAccounts',
                          {'UserInstitutionID': user_institution_id})