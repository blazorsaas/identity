using BlazorSaas.Identity.Marten.Persistence.Queries;

namespace BlazorSaas.Identity.Marten;

public class MartenUserStore<TUser, TRole>(
    IdentityErrorDescriber identityErrorDescriber,
    IDocumentSession session,
    IQueryableRoleStore<TRole> roleStore,
    ILogger<MartenUserStore<TUser, TRole>> logger)
    : UserStoreBase<TUser, TRole, Guid, MartenIdentityUserClaim, MartenIdentityUserRole, MartenIdentityUserLogin,
        MartenIdentityUserToken, MartenIdentityRoleClaim>(identityErrorDescriber)
    where TUser : MartenIdentityUser
    where TRole : MartenIdentityRole
{
    public override IQueryable<TUser> Users => session.Query<TUser>();

    public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = new())
    {
        try
        {
            session.Store(user);

            await session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user.");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = new())
    {
        try
        {
            session.Update(user);

            await session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user.");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = new())
    {
        try
        {
            session.Delete(user);

            await session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user.");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public override Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = new())
    {
        return session.LoadAsync<TUser>(Guid.Parse(userId), cancellationToken);
    }

    public override Task<TUser?> FindByNameAsync(string normalizedUserName,
        CancellationToken cancellationToken = new())
    {
        var query = new UserByNormalizedName<TUser>
        {
            NormalizedUserName = normalizedUserName
        };
        return session.QueryAsync(query, cancellationToken);
    }

    protected override Task<TUser?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return session.LoadAsync<TUser>(userId, cancellationToken);
    }

    protected override async Task<MartenIdentityUserLogin?> FindUserLoginAsync(Guid userId, string loginProvider,
        string providerKey, CancellationToken cancellationToken)
    {
        var query = new LoginByUserProviderAndKey
        {
            UserId = userId,
            LoginProvider = loginProvider,
            ProviderKey = providerKey
        };
        return await session.QueryAsync(query, cancellationToken);
    }

    protected override async Task<MartenIdentityUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey,
        CancellationToken cancellationToken)
    {
        var query = new LoginByProviderAndKey
        {
            LoginProvider = loginProvider,
            ProviderKey = providerKey
        };
        return await session.QueryAsync(query, cancellationToken);
    }

    public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        var query = new ClaimsByUserId
        {
            UserId = user.Id
        };
        var userClaims = await session.QueryAsync(query, cancellationToken);

        return userClaims
            .Select(c => c.ToClaim())
            .ToList();
    }

    public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);

        foreach (var claim in claims)
        {
            var userClaim = new MartenIdentityUserClaim
            {
                UserId = user.Id
            };
            userClaim.InitializeFromClaim(claim);
            session.Store(userClaim);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);

        var query = new ClaimsByUserIdAndExistingClaim
        {
            UserId = user.Id,
            ClaimType = claim.ValueType,
            ClaimValue = claim.Value
        };
        var userClaims = await session.QueryAsync(query, cancellationToken);

        foreach (var userClaim in userClaims)
        {
            userClaim.InitializeFromClaim(newClaim);
            session.Update(userClaim);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);

        var claimList = claims.ToList();

        var query = new ClaimsByUserId
        {
            UserId = user.Id
        };
        var userClaims = await session.QueryAsync(query, cancellationToken);
        foreach (var userClaim in userClaims)
        {
            if (claimList.Any(c => c.Value == userClaim.ClaimValue && c.ValueType == userClaim.ClaimType))
            {
                session.Delete(userClaim);
            }
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(claim);

        var query = new UsersByClaim<TUser>
        {
            ClaimType = claim.ValueType,
            ClaimValue = claim.Value
        };
        await session.QueryAsync(query, cancellationToken);

        return query.Users;
    }

    protected override async Task<MartenIdentityUserToken?> FindTokenAsync(TUser user, string loginProvider,
        string name,
        CancellationToken cancellationToken)
    {
        var query = new TokenByUserProviderAndName
        {
            UserId = user.Id,
            LoginProvider = loginProvider,
            Name = name
        };
        return await session.QueryAsync(query, cancellationToken);
    }

    protected override async Task AddUserTokenAsync(MartenIdentityUserToken token)
    {
        session.Store(token);
        await session.SaveChangesAsync();
    }

    protected override async Task RemoveUserTokenAsync(MartenIdentityUserToken token)
    {
        session.Delete(token);
        await session.SaveChangesAsync();
    }

    public override async Task AddLoginAsync(TUser user, UserLoginInfo login,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(login);

        var userLogin = new MartenIdentityUserLogin
        {
            UserId = user.Id,
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName,
            ProviderKey = login.ProviderKey
        };

        session.Store(userLogin);
        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(loginProvider);
        ArgumentNullException.ThrowIfNull(providerKey);

        session.DeleteWhere<MartenIdentityUserLogin>(x =>
            x.UserId == user.Id &&
            x.LoginProvider == loginProvider &&
            x.ProviderKey == providerKey);
        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        var query = new LoginsByUser
        {
            UserId = user.Id
        };
        var userLogins = await session.QueryAsync(query, cancellationToken);

        return userLogins
            .Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
            .ToList();
    }

    public override Task<TUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(normalizedEmail);

        var query = new UserByNormalizedEmail<TUser>
        {
            NormalizedEmail = normalizedEmail
        };
        return session.QueryAsync(query, cancellationToken);
    }

    public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(normalizedRoleName);

        var userRoles = await GetRolesAsync(user, cancellationToken);
        return userRoles.Contains(normalizedRoleName);
    }

    protected override Task<TRole?> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return roleStore.FindByNameAsync(normalizedRoleName, cancellationToken);
    }

    protected override async Task<MartenIdentityUserRole?> FindUserRoleAsync(Guid userId, Guid roleId,
        CancellationToken cancellationToken)
    {
        var query = new UserRoleByIds
        {
            UserId = userId,
            RoleId = roleId
        };
        return await session.QueryAsync(query, cancellationToken);
    }

    public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName,
        CancellationToken cancellationToken = new())
    {
        var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
        if (role == null)
        {
            return [];
        }

        var query = new UsersInRole<TUser>
        {
            RoleId = role.Id
        };
        await session.QueryAsync(query, cancellationToken);

        return query.Users;
    }

    public override async Task AddToRoleAsync(TUser user, string normalizedRoleName,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(normalizedRoleName);

        var role = await roleStore.FindByNameAsync(normalizedRoleName, cancellationToken);
        if (role == null)
        {
            logger.LogError("Role {RoleName} not found", normalizedRoleName);
            return;
        }

        var userRole = new MartenIdentityUserRole
        {
            RoleId = role.Id,
            UserId = user.Id
        };
        session.Store(userRole);

        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName,
        CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(normalizedRoleName);

        var role = await roleStore.FindByNameAsync(normalizedRoleName, cancellationToken);
        if (role == null)
        {
            logger.LogError("Role {RoleName} not found", normalizedRoleName);
            return;
        }

        session.DeleteWhere<MartenIdentityUserRole>(x =>
            x.UserId == user.Id &&
            x.RoleId == role.Id);

        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        var query = new RolesByUserId<TRole>
        {
            UserId = user.Id
        };
        await session.QueryAsync(query, cancellationToken);

        IList<string> roleNames = [];
        foreach (var role in query.Roles)
        {
            if (!string.IsNullOrEmpty(role.NormalizedName))
            {
                roleNames.Add(role.NormalizedName);
            }
        }

        return roleNames;
    }
}