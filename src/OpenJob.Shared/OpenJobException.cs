using System.Runtime.Serialization;

namespace OpenJob;

public class OpenJobException : Exception
{
    public OpenJobException()
    {

    }

    public OpenJobException(string message)
        : base(message)
    {

    }

    public OpenJobException(string message, Exception innerException)
        : base(message, innerException)
    {

    }

    public OpenJobException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {

    }
}
