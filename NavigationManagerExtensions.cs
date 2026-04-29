using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace QueueApp;

public static class NavigationManagerExtensions
{
    public static string? QueryString(this NavigationManager navigationManager, string key)
    {
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var value))
        {
            return value;
        }
        return null;
    }
}
