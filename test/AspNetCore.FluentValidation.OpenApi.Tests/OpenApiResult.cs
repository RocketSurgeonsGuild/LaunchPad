namespace AspNetCore.FluentValidation.OpenApi.Tests;

public record OpenApiResult(string Path, Func<Task<string>> JsonFactory)
{
    public override string ToString() => Path;
}
