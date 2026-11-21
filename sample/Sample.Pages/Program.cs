
using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Pages;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
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

app.UseAuthorization();

app.MapLaunchPadHealthChecks();
app.MapRazorPages();

await app.RunAsync();

namespace Sample.Pages
{
    public partial class Program;
}
