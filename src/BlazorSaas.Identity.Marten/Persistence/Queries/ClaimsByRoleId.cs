namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class ClaimsByRoleId : ICompiledListQuery<MartenIdentityRoleClaim, MartenIdentityRoleClaim>
{
    public Guid RoleId { get; init; }

    public Expression<Func<IMartenQueryable<MartenIdentityRoleClaim>, IEnumerable<MartenIdentityRoleClaim>>> QueryIs()
    {
        return q => q
            .Where(x => x.RoleId == RoleId);
    }
}