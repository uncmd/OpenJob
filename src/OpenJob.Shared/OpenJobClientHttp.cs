using OpenJob.Enums;
using System.Net.Http.Json;
using System.Text.Json;

namespace OpenJob;

internal class OpenJobClientHttp : OpenJobClient
{
    private const string ServerTypeKey = "serverType";
    private const string MethodNameKey = "methodName";

    private readonly Uri httpEndpoint;
    private readonly HttpClient httpClient;
    private readonly KeyValuePair<string, string>? apiTokenHeader;

    private readonly JsonSerializerOptions jsonSerializerOptions;

    public override JsonSerializerOptions JsonSerializerOptions => jsonSerializerOptions;

    internal OpenJobClientHttp(
        HttpClient httpClient,
        Uri httpEndpoint,
        JsonSerializerOptions jsonSerializerOptions,
        KeyValuePair<string, string>? apiTokenHeader)
    {
        this.httpClient = httpClient;
        this.httpEndpoint = httpEndpoint;
        this.jsonSerializerOptions = jsonSerializerOptions;
        this.apiTokenHeader = apiTokenHeader;

        this.httpClient.DefaultRequestHeaders.UserAgent.Add(UserAgent());
    }

    public override HttpRequestMessage CreateInvokeMethodRequest(HttpMethod httpMethod, ServerType serverType, string methodName)
    {
        ArgumentNullException.ThrowIfNull(httpMethod);
        ArgumentNullException.ThrowIfNull(serverType);
        ArgumentNullException.ThrowIfNull(methodName);

        var path = $"/v1.0/invoke/{serverType}/method/{methodName.TrimStart('/')}";
        var request = new HttpRequestMessage(httpMethod, new Uri(this.httpEndpoint, path));
        request.Options.TryAdd(ServerTypeKey, serverType);
        request.Options.TryAdd(MethodNameKey, methodName);

        if (this.apiTokenHeader is not null)
        {
            request.Headers.TryAddWithoutValidation(this.apiTokenHeader.Value.Key, this.apiTokenHeader.Value.Value);
        }

        return request;
    }

    public override HttpRequestMessage CreateInvokeMethodRequest<TRequest>(HttpMethod httpMethod, ServerType serverType, string methodName, TRequest data)
    {
        ArgumentNullException.ThrowIfNull(httpMethod);
        ArgumentNullException.ThrowIfNull(serverType);
        ArgumentNullException.ThrowIfNull(methodName);

        var request = CreateInvokeMethodRequest(httpMethod, serverType, methodName);
        request.Content = JsonContent.Create<TRequest>(data, options: this.JsonSerializerOptions);
        return request;
    }

    public override async Task InvokeMethodAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await InvokeMethodWithResponseAsync(request, cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            request.Options.TryGetValue(new HttpRequestOptionsKey<ServerType>(ServerTypeKey), out var serverType);
            request.Options.TryGetValue(new HttpRequestOptionsKey<string>(MethodNameKey), out var methodName);

            throw new InvocationException(
                serverType: serverType,
                methodName: methodName,
                innerException: ex,
                response: null);
        }
    }

    public override async Task<TResponse> InvokeMethodAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await InvokeMethodWithResponseAsync(request, cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            request.Options.TryGetValue(new HttpRequestOptionsKey<ServerType>(ServerTypeKey), out var serverType);
            request.Options.TryGetValue(new HttpRequestOptionsKey<string>(MethodNameKey), out var methodName);

            throw new InvocationException(
                serverType: serverType,
                methodName: methodName,
                innerException: ex,
                response: null);
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<TResponse>(this.jsonSerializerOptions, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            request.Options.TryGetValue(new HttpRequestOptionsKey<ServerType>(ServerTypeKey), out var serverType);
            request.Options.TryGetValue(new HttpRequestOptionsKey<string>(MethodNameKey), out var methodName);

            throw new InvocationException(
                serverType: serverType,
                methodName: methodName,
                innerException: ex,
                response: null);
        }
        catch (JsonException ex)
        {
            request.Options.TryGetValue(new HttpRequestOptionsKey<ServerType>(ServerTypeKey), out var serverType);
            request.Options.TryGetValue(new HttpRequestOptionsKey<string>(MethodNameKey), out var methodName);

            throw new InvocationException(
                serverType: serverType,
                methodName: methodName,
                innerException: ex,
                response: null);
        }
    }

    public override async Task<HttpResponseMessage> InvokeMethodWithResponseAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!this.httpEndpoint.IsBaseOf(request.RequestUri))
        {
            throw new InvalidOperationException("The provided request URI is not a OpenJob service invocation URI.");
        }

        try
        {
            return await this.httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            request.Options.TryGetValue(new HttpRequestOptionsKey<ServerType>(ServerTypeKey), out var serverType);
            request.Options.TryGetValue(new HttpRequestOptionsKey<string>(MethodNameKey), out var methodName);

            throw new InvocationException(
                serverType: serverType,
                methodName: methodName,
                innerException: ex,
                response: null);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.httpClient.Dispose();
        }
    }
}
