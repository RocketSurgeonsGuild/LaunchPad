using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.BlazorServer;
using Sample.BlazorServer.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSampleSignIn(builder.Configuration);
builder.Services.AddCascadingAuthenticationState();
// A component dispatches its operations from the circuit, where there is no HTTP request: the caller is the circuit's signed-in user.
builder.Services.AddScoped<ZeroAlloc.Authorization.ISecurityContext, CircuitSecurityContext>();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapSampleSignIn();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();

namespace Sample.BlazorServer
{
    public partial class Program;
}
