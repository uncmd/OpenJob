using OpenJob.Enums;

namespace OpenJob;

public class InvocationException : OpenJobException
{
    public InvocationException(ServerType serverType, string methodName, Exception innerException, HttpResponseMessage response)
        : base(FormatExceptionForFailedRequest(serverType, methodName), innerException)
    {
        this.ServerType = serverType;
        this.MethodName = methodName ?? "unknown";
        this.Response = response;
    }

    public ServerType ServerType { get; }

    public string MethodName { get; }

    public HttpResponseMessage Response { get; }

    private static string FormatExceptionForFailedRequest(ServerType serverType, string methodName)
    {
        return $"An exception occurred while invoking method: '{methodName}' on server-type: '{serverType}'";
    }
}
