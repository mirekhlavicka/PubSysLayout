using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.AspNetCore.DataProtection;

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

    //!!!!!! loging on !!!!!!
    //options.LogTo(Console.WriteLine);
});

builder.Services.AddControllersWithViews().AddJsonOptions(
            options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        );

builder.Services.AddRazorPages();

var keysDirectoryName = "Keys";
var keysDirectoryPath = Path.Combine(builder.Environment.ContentRootPath, keysDirectoryName);
if (!Directory.Exists(keysDirectoryPath))
{
    Directory.CreateDirectory(keysDirectoryPath);
}
builder.Services.AddDataProtection()
      .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath))
      .SetApplicationName("PubSysLayout");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(90);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
        options.LoginPath = "/Login";
    });

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html", new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        // !!!!!!!!!!!!!!  never cache index.html !!!!!!!!!!!!!!!!!
        if (context.File.Name == "index.html")
        {
            context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            context.Context.Response.Headers.Add("Expires", "-1");
        }
    }
});

app.Run();