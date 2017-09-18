using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MainOAuthDemo.Models
{
    public class ClientInfo
    {
        public readonly string ClientId;
        public readonly string ClientSecret;

        public ClientInfo()
        {
            ClientId = ConfigurationManager.AppSettings["clientId"];
            ClientSecret = ConfigurationManager.AppSettings["clientSecret"];
        }
        public ClientInfo(string client_id, string client_secret)
        {
            ClientId = client_id;
            ClientSecret = client_secret;
        }
    }

    public class ServerInfo
    {
        /// <summary>
        /// AuthorizationServer project should run on this URL
        /// </summary>
        public const string AuthorizationServerBaseAddress = "https://sophtron.com";

        /// <summary>
        /// ResourceServer project should run on this URL
        /// </summary>
        public const string ResourceServerBaseAddress = "https://api.sophtron.com";

        public const string AuthorizePath = "/OAuth/Authorize";
        public const string TokenPath = "/OAuth/Token";
    }

}