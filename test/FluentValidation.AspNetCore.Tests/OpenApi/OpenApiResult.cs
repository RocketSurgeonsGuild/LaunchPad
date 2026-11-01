namespace FluentValidation.AspNetCore.Tests.OpenApi;

public record OpenApiResult(string Path, Func<Task<string>> JsonFactory)
{
    public override string ToString() => Path;
}
