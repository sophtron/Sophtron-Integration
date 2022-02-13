using System;
using SophtronClient;

namespace QuickExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pinging Api with direct auth:");
            var directClient = new ApiClient(new DirectAuthClient());
            var ret = directClient.HealthCheck().Result;
            Console.WriteLine(ret);

            Console.WriteLine("Pinging Api with oauth:");
            var oauthClient = new ApiClient(new OauthClient());
            ret = oauthClient.HealthCheck().Result;
            Console.WriteLine(ret);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
