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