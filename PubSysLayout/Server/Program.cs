using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<PubSysLayout.Shared.Model.LayoutDBContext>((serviceProvider, options) =>
{

    var httpRequest = serviceProvider.GetService<IHttpContextAccessor>().HttpContext.Request;

    var current = (httpRequest.Headers["constr"].FirstOrDefault() ?? "").ToString();

    if (String.IsNullOrEmpty(current))
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("PubSysLayoutContextDefault"));
    }
    else
    {
        options.UseSqlServer(String.Format(builder.Configuration.GetConnectionString("PubSysLayoutContext"), current));
    }
});

builder.Services.AddControllersWithViews().AddJsonOptions(
            options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        );

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();