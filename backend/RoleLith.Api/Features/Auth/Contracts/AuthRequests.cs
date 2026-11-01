namespace RoleLith.Api.Features.Auth.Contracts;


public sealed class RegistrationCodeRequest
{
    public required string Email { get; init; }
}

public sealed class RegisterRequest
{
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string OrganizationName { get; init; }
    public required string Password { get; init; }
    public required string VerificationCode { get; init; }
}

public sealed class LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public bool RememberMe { get; init; }
}

public sealed class RefreshRequest
{
    public required string RefreshToken { get; init; }
}

public sealed class LogoutRequest
{
    public required string RefreshToken { get; init; }
}

public sealed class ForgotPasswordRequest
{
    public required string Email { get; init; }
}

public sealed class ValidatePasswordResetRequest
{
    public required string Email { get; init; }
    public required string Token { get; init; }
}

public sealed class ResetPasswordRequest
{
    public required string Email { get; init; }
    public required string Token { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmPassword { get; init; }
}

public sealed class ChangePasswordRequest
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmPassword { get; init; }
}
