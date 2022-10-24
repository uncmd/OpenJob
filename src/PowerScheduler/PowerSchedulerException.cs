using System.Runtime.Serialization;
using Volo.Abp;

namespace PowerScheduler;

public class PowerSchedulerException : AbpException
{
    public PowerSchedulerException()
    {

    }

    public PowerSchedulerException(string message)
        : base(message)
    {

    }

    public PowerSchedulerException(string message, Exception innerException)
        : base(message, innerException)
    {

    }

    public PowerSchedulerException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {

    }
}
