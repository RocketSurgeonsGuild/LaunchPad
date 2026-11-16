// Nothing runs: the project exists to be published, so ILLink and the AOT compiler analyze the LaunchPad assemblies it roots.
var builder = WebApplication.CreateSlimBuilder(args);
await using var app = builder.Build();
