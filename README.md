# How to enable a Blazor application as a client of a third-party REST API

If we want a Blazor client server system, we usually program a Blazor application hosted on ASP.NET Core. In the case that the REST server is a separate application, be it Asp.NET Core, Django, NodeJS, others, we can perfectly enable our Blazor Wasm application and use the REST services of that server. We need the following:

1. The REST server allows the URL of the Blazor application in CORS

2. Configure HTTP service for Injection with Server URL

3. Install Microsoft.Extensions.Http on the client

This last step is not required if we use the default client service that only points to the server's URL. I suggest using IHttpClientFactory, since it gives identity to the service under a name, and we can use more than one REST server.

> Additionally, when consuming a service we define classes or registers that decode the data we need. To maintain the pure dynamism and virtues of REST it is not desirable to make data contracts.

After installing `Microsoft.Extensions.Http`, we configure the REST application service as follows:

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AppRestClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // The server must allow the client url in its CORS configuration
            builder.Services.AddHttpClient("AspNetApi", _ => _.BaseAddress = new Uri("https://localhost:44382"));

            // by example
            builder.Services.AddHttpClient("DjangoApi", _ => _.BaseAddress = new Uri("https://localhost:5000"));

            await builder.Build().RunAsync();
        }
    }
}
```

To use in a Blazor component we inject `IHttpClientFactory`, instantiate an `HttpClient`, define the type that can be obtained from the REST server in question, and invoke with `GetFromJsonAsync` to obtain the decoded response.

```csharp
@page "/fetchdata"
@inject IHttpClientFactory _clientFactory

<h1>Weather forecast</h1>

<p>This component demonstrates fetching data from the server.</p>

@if (forecasts == null) {
    <p><em>Loading...</em></p>
}
else {
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in forecasts) {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
    <br />
    <button class="btn btn-info" @onclick="Update">Update</button>
}

@code {
    record WeatherForecast(DateTime Date, int TemperatureC, string Summary, int TemperatureF);

    HttpClient client;

    WeatherForecast[] forecasts;

    protected override async Task OnInitializedAsync() {
        client = _clientFactory.CreateClient("AspNetApi");
        await Update();
    }

    async Task Update() => forecasts = await client.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
}

```

> This is the classic example of the default ASP.NET Core Web API template. Note that I use a register type to decode the read data. Of course it is more effect than doing it with a class.

---

`Source: (Blazor Spread)[https://www.blazorspread.net]`
