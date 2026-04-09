namespace TaskManager.WebApi.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "TaskManager";
    public string Audience { get; init; } = "TaskManager";
    public string SigningKey { get; init; } = "CHANGE_ME_DEV_ONLY_CHANGE_ME_DEV_ONLY";
    public int ExpirationMinutes { get; init; } = 60;
}

