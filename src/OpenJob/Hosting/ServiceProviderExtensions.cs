namespace System;

public static class ServiceProviderExtensions
{
    public static async Task ExecuteAsync(
        this IServiceProvider serviceProvider,
        Delegate @delegate,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var args = GetArguments(scope, @delegate, cancellationToken);
        var result = @delegate.DynamicInvoke(args.ToArray());
        if (result is Task task)
        {
            await task;
        }
    }

    public static void Execute(
        this IServiceProvider serviceProvider,
        Delegate @delegate)
    {
        using var scope = serviceProvider.CreateScope();
        var args = GetArguments(scope, @delegate);
        @delegate.DynamicInvoke(args);
    }

    private static object[] GetArguments(
        IServiceScope scope,
        Delegate @delegate,
        CancellationToken? cancellationToken = default)
    {
        var method = @delegate.Method;
        var parameters = method.GetParameters();

        var args = new List<object>(parameters.Length);
        foreach (var parameterInfo in parameters)
        {
            var type = parameterInfo.ParameterType;
            if (type == typeof(CancellationToken))
            {
                args.Add(cancellationToken);
            }
            else
            {
                var arg = scope.ServiceProvider.GetService(type);
                args.Add(arg);
            }
        }

        return args.ToArray();
    }
}
