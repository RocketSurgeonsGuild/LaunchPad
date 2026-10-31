using System.Buffers;
using System.Reflection;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Rivets;
using Rocket.Surgery.LaunchPad.Foundation;

namespace AspNetCore.NewtonsoftJson.Tests;

public class NewtonsoftJsonJointTests
{
    [Test]
    public async Task Adds_The_Newtonsoft_Json_Formatters_To_Mvc()
    {
        var provider = await Register(new ServiceCollection());

        provider.GetRequiredService<IOptions<MvcOptions>>().Value.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().ShouldNotBeEmpty();
    }

    [Test]
    public async Task Adds_The_Validation_Problem_Details_Converter()
    {
        var provider = await Register(new ServiceCollection());
        var settings = provider.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;

        settings.Converters.OfType<ValidationProblemDetailsNewtonsoftJsonConverter>().ShouldHaveSingleItem();
    }

    [Test]
    public async Task Applies_The_Registered_Json_Serializer_Settings_To_The_Mvc_Settings()
    {
        var marker = new MarkerConverter();
        // Unnamed configuration applies to every name, which is how Foundation.NewtonsoftJson registers its own.
        var provider = await Register(
            new ServiceCollection().AddSingleton<IConfigureOptions<JsonSerializerSettings>>(
                new ConfigureNamedOptions<JsonSerializerSettings>(null, settings => settings.Converters.Add(marker))
            )
        );

        provider.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings.Converters.ShouldContain(marker);
    }

    // The convention took FoundationOptions in its constructor but never read them, so the options a context carries change nothing.
    [Test]
    public async Task Registers_The_Same_Services_Whatever_Foundation_Options_The_Context_Has()
    {
        var withoutOptions = new ServiceCollection();
        new NewtonsoftJsonJoint().Register(await JointContext(), withoutOptions);

        var withOptions = new ServiceCollection();
        new NewtonsoftJsonJoint().Register(
            await JointContext(context => context.Set(new FoundationOptions { MediatorLifetime = ServiceLifetime.Singleton, ValidatorLifetime = ServiceLifetime.Transient })),
            withOptions
        );

        withOptions.Select(x => (x.ServiceType, x.Lifetime)).ShouldBe(withoutOptions.Select(x => (x.ServiceType, x.Lifetime)));
    }

    [Test]
    public void Is_Exported_As_An_Application_Joint_Without_Ordering_Or_Gating()
    {
        var export = NewtonsoftJsonExports.Joints().Single(x => x.Joint is NewtonsoftJsonJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Application);
        typeof(NewtonsoftJsonJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    private static async Task<IServiceProvider> Register(IServiceCollection services)
    {
        // The host provides these; the Joint only adds the formatters on top of them.
        services.AddLogging().AddSingleton(ArrayPool<char>.Shared);
        new NewtonsoftJsonJoint().Register(await JointContext(), services);
        return services.BuildServiceProvider();
    }

    private static async Task<IDovetailContext> JointContext(Action<IDovetailContext>? configure = null)
    {
        var context = await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
        configure?.Invoke(context);
        return context;
    }

    private sealed class MarkerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => false;

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => throw new NotSupportedException();

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
