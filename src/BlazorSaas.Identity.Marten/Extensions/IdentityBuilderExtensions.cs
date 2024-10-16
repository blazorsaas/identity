using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorSaas.Identity.Marten.Extensions;

public static class IdentityBuilderExtensions
{
    public static IdentityBuilder AddMartenStores(this IdentityBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var userType = builder.UserType;
        var roleType = builder.RoleType;

        if (userType is null)
        {
            throw new InvalidOperationException(
                $"Must provide an identity user of type {typeof(MartenIdentityUser).FullName} or one that extends this type.");
        }

        if (!typeof(MartenIdentityUser).IsAssignableFrom(userType))
        {
            throw new InvalidOperationException($"{userType.Name} must extend {typeof(MartenIdentityUser).FullName}.");
        }

        if (roleType is null)
        {
            roleType = typeof(MartenIdentityRole);
        }
        else if (!typeof(MartenIdentityRole).IsAssignableFrom(roleType))
        {
            throw new InvalidOperationException($"{roleType.Name} must extend {typeof(MartenIdentityRole).FullName}.");
        }

        var userStoreType = typeof(MartenUserStore<,>).MakeGenericType(userType, roleType);
        var roleStoreType = typeof(MartenRoleStore<>).MakeGenericType(roleType);

        builder.Services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserLoginStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserClaimStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserPasswordStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserSecurityStampStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserEmailStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserLockoutStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserPhoneNumberStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IQueryableUserStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserTwoFactorStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserAuthenticationTokenStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserAuthenticatorKeyStore<>).MakeGenericType(userType), userStoreType);
        builder.Services.TryAddScoped(typeof(IUserTwoFactorRecoveryCodeStore<>).MakeGenericType(userType),
            userStoreType);
        builder.Services.TryAddScoped(typeof(IUserRoleStore<>).MakeGenericType(userType), userStoreType);

        builder.Services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
        builder.Services.TryAddScoped(typeof(IRoleClaimStore<>).MakeGenericType(roleType), roleStoreType);
        builder.Services.TryAddScoped(typeof(IQueryableRoleStore<>).MakeGenericType(roleType), roleStoreType);

        return builder;
    }
}