namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class UsersByClaim<TUser> : ICompiledListQuery<MartenIdentityUserClaim>
    where TUser : MartenIdentityUser
{
    public string ClaimType { get; init; } = default!;
    public string ClaimValue { get; init; } = default!;

    public IList<TUser> Users { get; } = [];

    public Expression<Func<IMartenQueryable<MartenIdentityUserClaim>, IEnumerable<MartenIdentityUserClaim>>> QueryIs()
    {
        return q => q
            .Where(x => x.ClaimType == ClaimType && x.ClaimValue == ClaimValue)
            .Include(x => x.UserId, Users);
    }
}