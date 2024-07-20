using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using NezChu.Shared.Models;

namespace NezChu.Client.AuthenticationStateSyncer;

/// <summary>
/// This class is the client counterpart of the PersistingRevalidatingAuthenticationStateProvider class of the server. 
/// It deserializes the user data from the persistent state by using the UserInfo class as its structure. 
/// It then creates a ClaimsPrincipal based on the user data and embeds it in the authentication state that is made 
/// available to the WebAssembly code running on the browser.
/// </summary>
/// <param name="persistentState"></param>
public class PersistentAuthenticationStateProvider(PersistentComponentState persistentState) : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> _unauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!persistentState.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return _unauthenticatedTask;
        }

        Claim[] claims = [
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
            new Claim(ClaimTypes.Name, userInfo.Name ?? string.Empty),
            new Claim(ClaimTypes.Email, userInfo.Email ?? string.Empty)];

        return Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }
}

