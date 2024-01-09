using AgroScan.Infrastructure.Identity;

namespace AgroScan.WebAPI.Endpoints.Users;
public static class Users
{
    public static void MapUsers(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/v1/users")
           .MapIdentityApi<ApplicationUser>();
    }

}
