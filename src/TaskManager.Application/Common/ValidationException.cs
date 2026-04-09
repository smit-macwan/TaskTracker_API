namespace TaskManager.Application.Common;

public sealed class ValidationException(string message) : AppException(message);

