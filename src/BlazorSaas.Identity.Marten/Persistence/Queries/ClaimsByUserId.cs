namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class ClaimsByUserId : ICompiledListQuery<MartenIdentityUserClaim>
{
    public Guid UserId { get; init; }

    public Expression<Func<IMartenQueryable<MartenIdentityUserClaim>, IEnumerable<MartenIdentityUserClaim>>> QueryIs()
    {
        return q => q
            .Where(x => x.UserId == UserId);
    }
}