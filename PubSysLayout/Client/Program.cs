using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PubSysLayout.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<CurrentDB>();
builder.Services.AddSingleton<LocalStorage>();

builder.Services.AddTransient/*Scoped*/(sp =>
{
    var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

    var db = sp.GetService<CurrentDB>().Current ?? "";

    http.DefaultRequestHeaders.Add("constr", db);

    return http;
});

builder.Services.AddMudServices();

await builder.Build().RunAsync();