using System.IdentityModel.Tokens.Jwt;

namespace CivicaShoppingAppClient.Infrastructure
{
    public interface IJwtTokenHandler
    {
        JwtSecurityToken ReadJwtToken(string token);
    }
}
