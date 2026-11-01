namespace RoleLith.Api.Features.Auth.Contracts;

public sealed record OrganizationResponse(Guid Id, string Name);

public sealed record CurrentUserResponse(
    Guid Id,
    string FullName,
    string Email,
    bool EmailConfirmed,
    OrganizationResponse Organization,
    IReadOnlyList<string> Roles);

public sealed record RegistrationResponse(Guid UserId, Guid OrganizationId);

// Returned to the Next.js server only, never forwarded to browser code or logged.
public sealed class TokenResponse
{
    public required string AccessToken { get; init; }
    public required DateTimeOffset AccessTokenExpiresAt { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset RefreshTokenExpiresAt { get; init; }
    public string TokenType => "Bearer";
}
