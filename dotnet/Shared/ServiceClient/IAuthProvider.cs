using System;
using System.Threading.Tasks;

namespace SophtronClient
{
    public interface IAuthProvider
    {
        Task<string> GetAuthPhrase(string httpMethod, string url);
    }
}
