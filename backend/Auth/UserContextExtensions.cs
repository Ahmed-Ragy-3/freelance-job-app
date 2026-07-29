using System.Security.Claims;

namespace backend.Auth
{
    public static class UserContextExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                     ?? user.FindFirst("id")
                     ?? user.FindFirst("userId");

            if (claim != null && int.TryParse(claim.Value, out var userId))
            {
                return userId;
            }

            return null;
        }

        public static string? GetRawBearerToken(this HttpRequest request)
        {
            var authHeader = request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                return null;
            }

            const string bearerPrefix = "Bearer ";
            return authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
                ? authHeader[bearerPrefix.Length..].Trim()
                : null;
        }
    }
}
