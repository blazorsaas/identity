using BlazorSaas.Identity.Marten.Persistence.Queries;
using StoreOptions = Marten.StoreOptions;

namespace BlazorSaas.Identity.Marten.Extensions;

public static class StoreOptionsExtensions
{
    public static StoreOptions RegisterIdentityCompiledQueries<TUser, TRole>(this StoreOptions options)
        where TUser : MartenIdentityUser
        where TRole : MartenIdentityRole
    {
        Type[] types =
        [
            typeof(ClaimsByRoleId),
            typeof(ClaimsByUserId),
            typeof(ClaimsByUserIdAndExistingClaim),
            typeof(LoginByProviderAndKey),
            typeof(LoginByUserProviderAndKey),
            typeof(LoginsByUser),
            typeof(RolesByUserId<TRole>),
            typeof(RoleByNormalizedName<TRole>),
            typeof(TokenByUserProviderAndName),
            typeof(UserByNormalizedEmail<TUser>),
            typeof(UserByNormalizedName<TUser>),
            typeof(UserRoleByIds),
            typeof(UsersByClaim<TUser>),
            typeof(UsersInRole<TUser>)
        ];

        foreach (var type in types)
        {
            options.RegisterCompiledQueryType(type);
        }

        return options;
    }
}