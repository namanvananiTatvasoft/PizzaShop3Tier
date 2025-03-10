using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace PizzashopRMS.Helpers;

public class CustomAuthoriseAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;
    public CustomAuthoriseAttribute(string[] roles)
    {
        _roles = roles;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Cookies["AuthToken"];
        if (token == null)
        {
            context.Result = new RedirectToRouteResult(new { controller = "Home", action = "Login" });
            return;
        }
        var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
        var handler = new JwtSecurityTokenHandler();
        var JWT = configuration.GetSection("jwt");
        var key = Encoding.UTF8.GetBytes(JWT["key"]);
        try
        {
            var ValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = JWT["issuer"],
                ValidateAudience = true,
                ValidAudience = JWT["audience"],
                ValidateLifetime = true
            };

            var ClaimsPrincipal = handler.ValidateToken(token, ValidationParameters, out SecurityToken validatedToken);

            var roleClaim = ClaimsPrincipal.Claims.FirstOrDefault(Claim => Claim.Type == ClaimTypes.Role)?.Value;
            if (roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new { controller = "Home", action = "Login" });
                return;
            }
            if (!_roles.Contains(roleClaim))
            {
                context.HttpContext.Response.Cookies.Delete("AuthToken");
                context.HttpContext.Response.Cookies.Delete("UserName");
                context.Result = new RedirectToRouteResult(new { Controller = "Home", action = "Hello" });
            }


        }
        catch (Exception ex)
        {
            context.HttpContext.Response.Cookies.Delete("AuthToken");
            context.HttpContext.Response.Cookies.Delete("UserName");
            context.Result = new RedirectToRouteResult(new { controller = "Home", action = "Login" });
        }
    }
}