using System;
using System.DirectoryServices;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PAM.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public static class ClaimExtensions
    {
        public static string GetClaim(this ClaimsIdentity identity, string claimType)
        {
            var claim = identity.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }

        public static int GetClaimAsInt(this ClaimsIdentity identity, string claimType)
        {
            var claim = identity.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value != null ? Int32.Parse(claim.Value) : -1;
        }
    }

    public static class SearchResultExtensions
    {
        public static string GetProperty(this SearchResult result, string name)
        {
            return result.Properties.Contains(name) ? result.Properties[name][0].ToString() : null;
        }
    }
}
