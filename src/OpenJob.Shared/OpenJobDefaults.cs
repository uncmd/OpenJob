namespace OpenJob;

internal static class OpenJobDefaults
{
    private static string httpEndpoint;
    private static string serverApiToken;
    private static string workerApiToken;

    public static string GetDefaultServerApiToken()
    {
        if (serverApiToken == null)
        {
            var value = Environment.GetEnvironmentVariable("SERVER_API_TOKEN");
            serverApiToken = (value == string.Empty) ? null : value;
        }

        return serverApiToken;
    }

    public static string GetDefaultWorkerApiToken()
    {
        if (workerApiToken == null)
        {
            var value = Environment.GetEnvironmentVariable("WORKER_API_TOKEN");
            workerApiToken = (value == string.Empty) ? null : value;
        }

        return workerApiToken;
    }

    public static string GetDefaultHttpEndpoint()
    {
        if (httpEndpoint == null)
        {
            var port = Environment.GetEnvironmentVariable("OPENJOB_HTTP_PORT");
            port = string.IsNullOrEmpty(port) ? "16666" : port;
            httpEndpoint = $"http://127.0.0.1:{port}";
        }

        return httpEndpoint;
    }
}
