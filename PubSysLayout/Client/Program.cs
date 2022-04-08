using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using PubSysLayout.Client;
using PubSysLayout.Client.AuthProviders;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<LocalStorage>();
builder.Services.AddSingleton<Clipboard>();
builder.Services.AddSingleton<CurrentDB>();

builder.Services.AddTransient/*Scoped*/(sp =>
{
    var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

    var db = sp.GetService<CurrentDB>().Current ?? "";

    http.DefaultRequestHeaders.Add("constr", db);

    return http;
});

builder.Services.AddSingleton<DBList>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    //config.SnackbarConfiguration.PreventDuplicates = false;
    //config.SnackbarConfiguration.NewestOnTop = false;
    //config.SnackbarConfiguration.ShowCloseIcon = true;
    //config.SnackbarConfiguration.VisibleStateDuration = 10000;
    //config.SnackbarConfiguration.HideTransitionDuration = 500;
    //config.SnackbarConfiguration.ShowTransitionDuration = 500;
    //config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

//builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthStateProvider>());

await builder.Build().RunAsync();