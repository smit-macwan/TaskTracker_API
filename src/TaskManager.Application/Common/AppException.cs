namespace TaskManager.Application.Common;

public abstract class AppException(string message) : Exception(message)
{
    public virtual string ErrorCode => GetType().Name;
}

