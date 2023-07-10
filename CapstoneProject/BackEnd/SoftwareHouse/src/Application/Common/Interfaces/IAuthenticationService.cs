namespace Application.Common.Interfaces;

public class IAuthenticationService
{
    Task<string> CreateUserAsync(CreateUserDto createUserDto,CancellationToken cancellationToken);
    Task<string> GenerateActivationTokenAsync(string userId,CancellationToken cancellationToken);
    Task<bool> CheckIfUserExists(string email, CancellationToken cancellationToken);

}