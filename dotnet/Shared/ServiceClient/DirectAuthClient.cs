using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SophtronClient
{
    public class DirectAuthClient : IAuthProvider
    {
        private string UserId = Environment.GetEnvironmentVariable("SophtronApiUserId");
        private string AccessKey = Environment.GetEnvironmentVariable("SophtronApiUserSecret");

        public Task<string> GetAuthPhrase(string httpMethod, string url)
        {
            var authPath = url.Substring(url.ToLower().LastIndexOf("/")).ToLower();
            var secret = Convert.FromBase64String(AccessKey);
            var plainKey = httpMethod.ToUpper() + '\n' + authPath;
            var hash = new HMACSHA256(secret);
            var hashedStr = hash.ComputeHash(Encoding.ASCII.GetBytes(plainKey));
            var b64Sig = Convert.ToBase64String(hashedStr);
            var authString = "FIApiAUTH:" + UserId + ":" + b64Sig + ":" + (authPath);
            return Task.FromResult(authString);
        }
    }
}
