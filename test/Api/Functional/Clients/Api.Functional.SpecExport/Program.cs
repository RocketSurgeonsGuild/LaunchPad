using Api.Functional.MediatR.Server;
using Microsoft.AspNetCore.TestHost;

// The specs clients are generated from, as the generated servers expose them: the OpenAPI document of the Minimal API endpoints, the GraphQL
// schema and the gRPC service's .proto. A file is only rewritten when it changes, so an unchanged spec doesn't regenerate its client.
if (args is not [var directory]) throw new ArgumentException("Pass the directory to write the specs to.");
Directory.CreateDirectory(directory);

await using (var server = await ServerApp.StartAsync())
{
    await Write("openapi.json", await server.GetTestClient().GetStringAsync(new Uri("openapi/v1.json", UriKind.Relative)));
}

await using (var server = await ServerApp.StartAsync(graphql: true))
{
    await Write("schema.graphql", await server.GetTestClient().GetStringAsync(new Uri("graphql?sdl", UriKind.Relative)));
}

await Write("rockets.proto", RocketsService.GetProto());

async Task Write(string name, string content)
{
    var path = Path.Combine(directory, name);
    if (!File.Exists(path) || await File.ReadAllTextAsync(path) != content) await File.WriteAllTextAsync(path, content);
}
