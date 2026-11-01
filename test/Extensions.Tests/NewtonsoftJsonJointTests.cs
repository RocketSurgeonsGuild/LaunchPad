using System.Reflection;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;
using Rocket.Surgery.LaunchPad.Foundation.Rivets;

namespace Extensions.Tests;

public class NewtonsoftJsonJointTests
{
    [Test]
    public async Task Writes_Enums_As_Camel_Case_Strings()
    {
        var settings = await Settings();

        JsonConvert.SerializeObject(DayOfWeek.Monday, settings).ShouldBe("\"monday\"");
        JsonConvert.DeserializeObject<DayOfWeek>("\"tuesday\"", settings).ShouldBe(DayOfWeek.Tuesday);
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NewtonsoftJsonExports.Joints().Single(x => x.Joint is NewtonsoftJsonJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        // The generated export metadata does not carry the category in Dovetail 0.3.0, so the attribute is what declares it.
        typeof(NewtonsoftJsonJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    private static async Task<JsonSerializerSettings> Settings()
    {
        // AddOptions comes from the Primitives DefaultJoint in an application.
        var services = new ServiceCollection().AddOptions();
        new NewtonsoftJsonJoint().Register(await JointContext.Create(), services);
        return services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerSettings>>().Value;
    }
}
