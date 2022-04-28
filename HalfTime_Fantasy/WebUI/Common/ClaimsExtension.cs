using System.Security.Claims;

namespace WebUI.Common
{
    public static class ClaimsExtension
    {
        public static T GetClaim<T>(this IEnumerable<Claim> claims, string claimType)
        {
            var claimValue = claims.Single(claim => claim.Type == claimType).Value;
            return (T)Convert.ChangeType(claimValue, typeof(T));
        }
    }
}
