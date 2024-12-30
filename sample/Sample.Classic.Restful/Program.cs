using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;

var builder = WebApplication
   .CreateBuilder(args);
builder.Services.AddControllers().AddControllersAsServices();
var app = await builder.ConfigureRocketSurgery();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapOpenApi();
app
   .UseSwaggerUI()
   .UseReDoc();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class Program
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            return ToString();
        }
    }
}