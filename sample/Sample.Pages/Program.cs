
using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Pages;

var builder = WebApplication.CreateBuilder(args);
// The rocket pages run operations whose policies require a caller, so they ask for one: an anonymous visitor is sent to sign in.
builder.Services.AddRazorPages(options => options.Conventions.AuthorizeFolder("/Rockets"));
builder.Services.AddSampleSignIn(builder.Configuration);
builder.Services.AddControllersWithViews();

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

app.MapLaunchPadHealthChecks();
app.MapSampleSignIn();
app.MapRazorPages();

await app.RunAsync();

namespace Sample.Pages
{
    public partial class Program;
}
