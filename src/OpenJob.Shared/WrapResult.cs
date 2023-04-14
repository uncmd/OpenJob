namespace OpenJob;

public class WrapResult
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public object Data { get; set; }

    public static WrapResult OK()
        => new WrapResult { Success = true };

    public static WrapResult OK(string message)
        => new WrapResult { Message = message, Success = true };

    public static WrapResult OK(object data)
        => new WrapResult { Data = data, Success = true };

    public static WrapResult Fail(string message)
        => new WrapResult { Message = message, Success = false };

    public static WrapResult Result(object data, bool success, string message = null)
        => new WrapResult { Data = data, Success = success, Message = message };
}
