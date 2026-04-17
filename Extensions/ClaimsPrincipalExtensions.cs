using System.Security.Claims;

namespace SmartCareApp.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetPortalUserId(this ClaimsPrincipal user)
    {
        var idValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(idValue, out var parsedId)
            ? parsedId
            : null;
    }
}
