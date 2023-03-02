using System.Text.Json;

namespace OpenJob;

public sealed class OpenJobClientBuilder
{
    internal string HttpEndpoint { get; private set; }

    private Func<HttpClient> HttpClientFactory { get; set; }

    internal JsonSerializerOptions JsonSerializerOptions { get; private set; }

    internal string DaprApiToken { get; private set; }

    public OpenJobClientBuilder()
    {
        this.HttpEndpoint = OpenJobDefaults.GetDefaultHttpEndpoint();

        this.JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        this.DaprApiToken = OpenJobDefaults.GetDefaultServerApiToken();
    }

    public OpenJobClientBuilder UseHttpEndpoint(string httpEndpoint)
    {
        ArgumentException.ThrowIfNullOrEmpty(httpEndpoint);
        this.HttpEndpoint = httpEndpoint;
        return this;
    }

    public OpenJobClientBuilder UseJsonSerializationOptions(JsonSerializerOptions options)
    {
        this.JsonSerializerOptions = options;
        return this;
    }

    public OpenJobClientBuilder UseDaprApiToken(string apiToken)
    {
        this.DaprApiToken = apiToken;
        return this;
    }

    // Internal for testing
    internal OpenJobClientBuilder UseHttpClientFactory(Func<HttpClient> factory)
    {
        this.HttpClientFactory = factory;
        return this;
    }

    public OpenJobClient Build()
    {
        var httpEndpoint = new Uri(this.HttpEndpoint);
        if (httpEndpoint.Scheme != "http" && httpEndpoint.Scheme != "https")
        {
            throw new InvalidOperationException("The HTTP endpoint must use http or https.");
        }

        var apiTokenHeader = OpenJobClient.GetApiTokenHeader(this.DaprApiToken);
        var httpClient = HttpClientFactory is not null ? HttpClientFactory() : new HttpClient();
        return new OpenJobClientHttp(httpClient, httpEndpoint, this.JsonSerializerOptions, apiTokenHeader);
    }
}
