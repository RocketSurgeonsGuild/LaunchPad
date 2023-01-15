using System.Net;
using Alba;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample.Graphql.Tests.Helpers;

[Obsolete("Remove once https://github.com/ChilliCream/hotchocolate/issues/5684 is fixed!")]
internal class TemporaryGraphQlExtension : IAlbaExtension
{
    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task Start(IAlbaHost host)
    {
        return Task.CompletedTask;
    }

    public IHostBuilder Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(z => z.AddHttpResponseFormatter<MyHttpResponseFormatter>());
        return builder;
    }
    
    [Obsolete("Remove once https://github.com/ChilliCream/hotchocolate/issues/5684 is fixed!")]
    internal class MyHttpResponseFormatter : DefaultHttpResponseFormatter
    {
        public MyHttpResponseFormatter() : base(true)
        {
        }

        protected override HttpStatusCode GetStatusCode(
            IResponseStream responseStream, FormatInfo format,
            HttpStatusCode? proposedStatusCode
        )
        {
            var code = base.GetStatusCode(responseStream, format, proposedStatusCode);
            return code == HttpStatusCode.InternalServerError ? HttpStatusCode.OK : code;
        }

        protected override HttpStatusCode GetStatusCode(
            IQueryResult result, FormatInfo format,
            HttpStatusCode? proposedStatusCode
        )
        {
            var code = base.GetStatusCode(result, format, proposedStatusCode);
            return code == HttpStatusCode.InternalServerError ? HttpStatusCode.OK : code;
        }
    }
}
