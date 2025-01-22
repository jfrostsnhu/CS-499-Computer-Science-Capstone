using System.Security.Claims;
using InventoryManagerApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Routing
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        // Mapping for Logout Endpoint
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            var accountGroup = endpoints.MapGroup("/Account");

            // Log out user
            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user,
                SignInManager<ApplicationUser> signInManager,
                [FromForm] string returnUrl) =>
            {
                await signInManager.SignOutAsync();
                return TypedResults.LocalRedirect("/Login");
            });

            var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

            return accountGroup;
        }
    }
}
