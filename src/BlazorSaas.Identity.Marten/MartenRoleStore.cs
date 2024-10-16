using BlazorSaas.Identity.Marten.Persistence.Queries;

namespace BlazorSaas.Identity.Marten;

internal class MartenRoleStore<TRole>(
    IdentityErrorDescriber identityErrorDescriber,
    IDocumentSession session,
    ILogger<MartenRoleStore<TRole>> logger)
    : RoleStoreBase<TRole, Guid, MartenIdentityUserRole, MartenIdentityRoleClaim>(identityErrorDescriber)
    where TRole : MartenIdentityRole
{
    public override IQueryable<TRole> Roles => session.Query<TRole>();

    public override async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = new())
    {
        try
        {
            session.Store(role);

            await session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public override async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = new())
    {
        try
        {
            session.Update(role);

            await session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public override async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = new())
    {
        try
        {
            session.Delete(role);

            await session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public override Task<TRole?> FindByIdAsync(string id, CancellationToken cancellationToken = new())
    {
        return session.LoadAsync<TRole>(Guid.Parse(id), cancellationToken);
    }

    public override Task<TRole?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = new())
    {
        var query = new RoleByNormalizedName<TRole>
        {
            NormalizedName = normalizedName
        };
        return session.QueryAsync(query, cancellationToken);
    }

    public override async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);

        var query = new ClaimsByRoleId
        {
            RoleId = role.Id
        };
        var claims = await session.QueryAsync(query, cancellationToken);

        return claims
            .Select(c => c.ToClaim())
            .ToList();
    }

    public override async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(claim);

        var roleClaim = new MartenIdentityRoleClaim
        {
            RoleId = role.Id
        };

        roleClaim.InitializeFromClaim(claim);
        session.Store(roleClaim);

        await session.SaveChangesAsync(cancellationToken);
    }

    public override async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(claim);

        session.DeleteWhere<MartenIdentityRoleClaim>(x =>
            x.RoleId == role.Id &&
            x.ClaimType == claim.Type &&
            x.ClaimValue == claim.Value);

        await session.SaveChangesAsync(cancellationToken);
    }
}