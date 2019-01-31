using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PAM.Extensions
{
    public static class PAMExtensions
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
            return (claim != null) ? claim.Value : null;
        }
    }
}
