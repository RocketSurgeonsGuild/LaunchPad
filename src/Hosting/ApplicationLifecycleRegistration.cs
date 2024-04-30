namespace Rocket.Surgery.LaunchPad.Hosting;

internal record ApplicationLifecycleRegistration(string Method, Func<IServiceProvider, CancellationToken, Task> Action);