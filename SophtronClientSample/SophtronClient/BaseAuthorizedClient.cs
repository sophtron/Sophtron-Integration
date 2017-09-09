using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SophtronClient
{
    public abstract class BaseAuthorizedClient : BaseClient
    {
        public abstract string AuthUserId { get; protected set; }
        public abstract string AuthUserAccessKey { get; protected set; }

        protected BaseAuthorizedClient()
        {
            this.SetDefaultAccessKeyAuthFunc();
        }

        #region base authorization
        protected List<KeyValuePair<string, string>> DefaultHeaderFunc(string url, string method)
        {
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization", CreateAccessKeyAuthCode(AuthUserId, AuthUserAccessKey, url, method))
            };
            return headers;
        }

        public void SetDefaultAccessKeyAuthFunc()
        {
            this._headerFunc = DefaultHeaderFunc;
        }

        private static string CreateAccessKeyAuthCode(string userId, string accessKey, string url, string httpMethod)
        {
            var authPath = url.Substring(url.ToLower().LastIndexOf("/")).ToLower();
            var secret = Convert.FromBase64String(accessKey);
            var plainKey = httpMethod.ToUpper() + '\n' + authPath;
            var hash = new HMACSHA256(secret);
            var hashedStr = hash.ComputeHash(Encoding.ASCII.GetBytes(plainKey));
            var b64Sig = Convert.ToBase64String(hashedStr);
            var authString = "FIApiAUTH:" + userId + ":" + b64Sig + ":" + (authPath);
            return authString;
        }
        #endregion

        #region get post with auth
        protected TRet PostWithAuth<TReq, TRet>(string urlPath, Func<TReq> payloadFunc)
        {
            var payload = payloadFunc();
            var endpoint = BaseEndpoint.JoinUrl(urlPath);
            var data = JsonConvert.SerializeObject(payload);
            var headers = this._headerFunc(endpoint, "POST");
            var result = Post(endpoint, data, null, headers);
            result = result == null ? string.Empty : result;
            return JsonConvert.DeserializeObject<TRet>(result);
        }

        protected TRet GetWithAuth<TRet>(string urlPath)
        {
            var endpoint = BaseEndpoint.JoinUrl(urlPath);
            var headers = this._headerFunc(endpoint, "GET");
            var result = Get(endpoint, headers);
            result = result == null ? string.Empty : result;
            return JsonConvert.DeserializeObject<TRet>(result);
        }
        #endregion

    }
}
