using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class StringFormatExtensions
    {
        public static string JoinUrl(this string url, string path)
        {
            return $@"{(url.EndsWith("/") ? url : $@"{url}/")}{(path.StartsWith("/") ? path.Substring(1) : path)}";
        }
    }
}
