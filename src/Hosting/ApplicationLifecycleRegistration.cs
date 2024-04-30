namespace Rocket.Surgery.LaunchPad.Hosting;

record ApplicationLifecycleRegistration(string Method, Func<IServiceProvider, CancellationToken, Task> Action);
