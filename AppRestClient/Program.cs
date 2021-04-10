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
