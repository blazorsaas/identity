namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class LoginByUserProviderAndKey : ICompiledQuery<MartenIdentityUserLogin, MartenIdentityUserLogin?>
{
    public Guid UserId { get; init; }
    public string LoginProvider { get; init; } = default!;
    public string ProviderKey { get; init; } = default!;

    public Expression<Func<IMartenQueryable<MartenIdentityUserLogin>, MartenIdentityUserLogin?>> QueryIs()
    {
        return q => q
            .SingleOrDefault(
                x => x.UserId == UserId && x.LoginProvider == LoginProvider && x.ProviderKey == ProviderKey);
    }
}