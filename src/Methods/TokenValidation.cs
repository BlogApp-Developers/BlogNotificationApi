using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using BlogNotificationApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlogNotificationApi.Methods;

public class TokenValidation
{
    private readonly JwtOptions jwtOptions;

    public TokenValidation(IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot)
    {
        this.jwtOptions = jwtOptionsSnapshot.Value;
    }
    public bool ValidateToken(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        SecurityToken validatedToken;
        IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
        return true;
    }

    public TokenValidationParameters GetValidationParameters()
    {
       
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtOptions.KeyInBytes)
        };
    }
}