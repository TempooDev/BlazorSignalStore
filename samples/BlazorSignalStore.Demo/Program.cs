using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorSignalStore.Demo;
using BlazorSignalStore;
using BlazorSignalStore.Demo.Store;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSignalStore<CounterStore>();
builder.Services.AddSignalStore<ShoppingCartStore>();
builder.Services.AddSignalStore<ApiDataStore>();

await builder.Build().RunAsync();