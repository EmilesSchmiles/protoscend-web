using System.Net.Http.Json;

namespace Protoscend.Services;

/// <summary>
/// Called from the Blazor WASM frontend.
/// Posts the contact form to your ASP.NET Core backend API endpoint.
///
/// SETUP STEPS:
///   1. Add an ASP.NET Core (or Azure Function / Minimal API) backend project.
///   2. Create POST /api/contact that receives ContactFormModel and calls EmailService.
///   3. Set BaseApiUrl below to your deployed API URL.
///
/// For a quick serverless option use an Azure Function — see README_EMAIL.txt.
/// </summary>
public class ContactApiService
{
    private readonly HttpClient _http;

    // e.g. "https://api.protoscend.co.za" or "https://yourfunc.azurewebsites.net"
    private const string BaseApiUrl = "https://protoscend-api.onrender.com";

    public ContactApiService(HttpClient http) => _http = http;

    public async Task<(bool success, string message)> SubmitAsync(ContactFormModel model)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{BaseApiUrl}/api/contact", model);
            if (response.IsSuccessStatusCode)
                return (true, "Message sent! We'll be in touch shortly.");

            return (false, "Server error — please try again or email us directly.");
        }
        catch
        {
            return (false, "Connection error — please check your connection and retry.");
        }
    }
}
