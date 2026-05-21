using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Protoscend;
using Protoscend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// ── Email settings ────────────────────────────────────────────────────────────
// For Blazor WebAssembly, SMTP must be called via an API endpoint (server-side).
// Register the settings here; wire the actual HTTP call in ContactService below.
// See: Services/ContactApiService.cs for the HttpClient-based approach.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<ContactApiService>();

await builder.Build().RunAsync();
