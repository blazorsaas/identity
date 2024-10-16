namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class TokenByUserProviderAndName : ICompiledQuery<MartenIdentityUserToken, MartenIdentityUserToken?>
{
    public Guid UserId { get; init; }
    public string LoginProvider { get; init; } = default!;
    public string Name { get; init; } = default!;

    public Expression<Func<IMartenQueryable<MartenIdentityUserToken>, MartenIdentityUserToken?>> QueryIs()
    {
        return q => q
            .SingleOrDefault(x => x.UserId == UserId && x.LoginProvider == LoginProvider && x.Name == Name);
    }
}