using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NezChu.Client.AuthenticationStateSyncer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region Auth0
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
#endregion

await builder.Build().RunAsync();
