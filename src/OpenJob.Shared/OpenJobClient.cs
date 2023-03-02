using OpenJob.Enums;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace OpenJob;

public abstract class OpenJobClient : IDisposable
{
    private bool disposed;

    public abstract JsonSerializerOptions JsonSerializerOptions { get; }

    internal static KeyValuePair<string, string>? GetApiTokenHeader(string apiToken)
    {
        if (string.IsNullOrWhiteSpace(apiToken))
        {
            return null;
        }

        return new KeyValuePair<string, string>("openjob-api-token", apiToken);
    }

    public HttpRequestMessage CreateInvokeMethodRequest(ServerType serverType, string methodName)
    {
        return CreateInvokeMethodRequest(HttpMethod.Post, serverType, methodName);
    }

    public abstract HttpRequestMessage CreateInvokeMethodRequest(HttpMethod httpMethod, ServerType serverType, string methodName);

    public HttpRequestMessage CreateInvokeMethodRequest<TRequest>(ServerType serverType, string methodName, TRequest data)
    {
        return CreateInvokeMethodRequest<TRequest>(HttpMethod.Post, serverType, methodName, data);
    }

    public abstract HttpRequestMessage CreateInvokeMethodRequest<TRequest>(HttpMethod httpMethod, ServerType serverType, string methodName, TRequest data);

    public abstract Task<HttpResponseMessage> InvokeMethodWithResponseAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);

    public abstract Task InvokeMethodAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);

    public abstract Task<TResponse> InvokeMethodAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken = default);

    public Task InvokeMethodAsync(
        ServerType serverType,
        string methodName,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest(serverType, methodName);
        return InvokeMethodAsync(request, cancellationToken);
    }

    public Task InvokeMethodAsync(
        HttpMethod httpMethod,
        ServerType serverType,
        string methodName,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest(httpMethod, serverType, methodName);
        return InvokeMethodAsync(request, cancellationToken);
    }

    public Task InvokeMethodAsync<TRequest>(
        ServerType serverType,
        string methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest<TRequest>(serverType, methodName, data);
        return InvokeMethodAsync(request, cancellationToken);
    }

    public Task InvokeMethodAsync<TRequest>(
        HttpMethod httpMethod,
        ServerType serverType,
        string methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest<TRequest>(httpMethod, serverType, methodName, data);
        return InvokeMethodAsync(request, cancellationToken);
    }

    public Task<TResponse> InvokeMethodAsync<TResponse>(
        ServerType serverType,
        string methodName,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest(serverType, methodName);
        return InvokeMethodAsync<TResponse>(request, cancellationToken);
    }

    public Task<TResponse> InvokeMethodAsync<TResponse>(
        HttpMethod httpMethod,
        ServerType serverType,
        string methodName,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest(httpMethod, serverType, methodName);
        return InvokeMethodAsync<TResponse>(request, cancellationToken);
    }

    public Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(
        ServerType serverType,
        string methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest<TRequest>(serverType, methodName, data);
        return InvokeMethodAsync<TResponse>(request, cancellationToken);
    }

    public Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(
        HttpMethod httpMethod,
        ServerType serverType,
        string methodName,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var request = CreateInvokeMethodRequest<TRequest>(httpMethod, serverType, methodName, data);
        return InvokeMethodAsync<TResponse>(request, cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!this.disposed)
        {
            Dispose(disposing: true);
            this.disposed = true;
        }
    }

    /// <summary>
    /// Disposes the resources associated with the object.
    /// </summary>
    /// <param name="disposing"><c>true</c> if called by a call to the <c>Dispose</c> method; otherwise false.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    protected static ProductInfoHeaderValue UserAgent()
    {
        var assembly = typeof(OpenJobClient).Assembly;
        string assemblyVersion = assembly
            .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
            .FirstOrDefault()?
            .InformationalVersion;

        return new ProductInfoHeaderValue("openjob-version", $"v{assemblyVersion}");
    }
}
