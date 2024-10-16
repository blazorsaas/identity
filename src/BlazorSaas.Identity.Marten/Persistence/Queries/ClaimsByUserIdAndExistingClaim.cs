namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class ClaimsByUserIdAndExistingClaim : ICompiledListQuery<MartenIdentityUserClaim>
{
    public Guid UserId { get; init; }
    public string ClaimType { get; init; } = default!;
    public string ClaimValue { get; init; } = default!;

    public Expression<Func<IMartenQueryable<MartenIdentityUserClaim>, IEnumerable<MartenIdentityUserClaim>>> QueryIs()
    {
        return q => q
            .Where(x => x.UserId == UserId && x.ClaimType == ClaimType && x.ClaimValue == ClaimValue);
    }
}