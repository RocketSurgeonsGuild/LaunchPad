using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sdk.Tests.Scaffolding;

/// <summary>
///     The API the clients of the OpenAPI client tests are generated from and call, served from the test process on a port of the loopback
///     interface: its OpenAPI document at <c>/openapi.json</c> (a client generated from a URL downloads it from there), and one rocket at
///     <c>/rockets/{id}</c>. No test reaches the network.
/// </summary>
internal sealed class RocketsApi : IAsyncDisposable
{
    /// <summary>
    ///     The id of the rocket the clients ask for.
    /// </summary>
    public const string RocketId = "6f9619ff-8b86-d011-b42d-00cf4fc964ff";

    /// <summary>
    ///     The OpenAPI document of the API: one operation, GetRocket, which returns a Rocket.
    /// </summary>
    public const string Document = """
        {
          "openapi": "3.1.1",
          "info": { "title": "Rockets", "version": "1.0.0" },
          "paths": {
            "/rockets/{id}": {
              "get": {
                "operationId": "GetRocket",
                "parameters": [ { "name": "id", "in": "path", "required": true, "schema": { "type": "string", "format": "uuid" } } ],
                "responses": {
                  "200": { "description": "OK", "content": { "application/json": { "schema": { "$ref": "#/components/schemas/Rocket" } } } }
                }
              }
            }
          },
          "components": {
            "schemas": {
              "Rocket": {
                "type": "object",
                "properties": {
                  "id": { "type": "string", "format": "uuid" },
                  "serialNumber": { "type": "string" }
                }
              }
            }
          }
        }
        """;

    private RocketsApi(HttpListener listener, Uri baseAddress)
    {
        _listener = listener;
        BaseAddress = baseAddress;
        _serving = Task.Run(Serve);
    }

    /// <summary>
    ///     The address the API listens on, with a trailing slash.
    /// </summary>
    public Uri BaseAddress { get; }

    /// <summary>
    ///     The address of the OpenAPI document.
    /// </summary>
    public Uri DocumentAddress => new(BaseAddress, "openapi.json");

    /// <summary>
    ///     How many times the OpenAPI document was downloaded.
    /// </summary>
    public int DocumentRequests => _documentRequests;

    /// <summary>
    ///     Starts the API on a free port.
    /// </summary>
    /// <returns>The API.</returns>
    public static RocketsApi Start()
    {
        // HttpListener cannot listen on port 0: take a port the system says is free, and another when a test running at the same time took
        // it first
        for (var attempt = 1; ; attempt++)
        {
            using var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            var port = ( (IPEndPoint)probe.LocalEndpoint ).Port;
            probe.Stop();

            var baseAddress = new Uri($"http://127.0.0.1:{port}/");
            var listener = new HttpListener();
            listener.Prefixes.Add(baseAddress.ToString());
            try
            {
                listener.Start();
                return new(listener, baseAddress);
            }
            catch (HttpListenerException) when (attempt < 10)
            {
                listener.Close();
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _listener.Stop();
        _listener.Close();
        try
        {
            await _serving;
        }
        catch (HttpListenerException) { }
        catch (ObjectDisposedException) { }
    }

    private async Task Serve()
    {
        while (_listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await _listener.GetContextAsync();
            }
            catch (HttpListenerException)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            var path = context.Request.Url!.AbsolutePath;
            var (status, body) = path switch
            {
                "/openapi.json" => (200, Document),
                _ when path.StartsWith("/rockets/", StringComparison.Ordinal) => (200, $$"""{ "id": "{{path["/rockets/".Length..]}}", "serialNumber": "FALCON-9" }"""),
                _ => (404, ""),
            };
            if (path == "/openapi.json") Interlocked.Increment(ref _documentRequests);

            var bytes = Encoding.UTF8.GetBytes(body);
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = bytes.Length;
            await context.Response.OutputStream.WriteAsync(bytes);
            context.Response.Close();
        }
    }

    private readonly HttpListener _listener;
    private readonly Task _serving;
    private int _documentRequests;
}
