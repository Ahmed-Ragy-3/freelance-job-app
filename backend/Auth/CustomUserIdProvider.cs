using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace backend.Auth
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var claim = connection.User?.FindFirst(ClaimTypes.NameIdentifier)
                     ?? connection.User?.FindFirst("id")
                     ?? connection.User?.FindFirst("userId");

            return claim?.Value;
        }
    }
}
