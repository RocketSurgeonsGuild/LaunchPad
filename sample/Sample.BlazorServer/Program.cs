using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.BlazorServer;
using Sample.BlazorServer.Data;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// LaunchPad.Hosting still registers the Serilog request logging services with conventions that Dovetail does not run,
// until its Hosting joints replace them (RocketSurgeonsGuild/LaunchPad#3058)
builder.Services.AddSerilog(logger => logger.WriteTo.Console());

var app = await builder.ConfigureDovetail();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();

namespace Sample.BlazorServer
{
    public partial class Program;
}
