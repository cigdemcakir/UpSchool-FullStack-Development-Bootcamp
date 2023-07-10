namespace Application.Common.Interfaces;

public class IJwtService
{
    JwtDto Generate(string userId, string email, string firstName, string lastName, List<string> roles = null);
}