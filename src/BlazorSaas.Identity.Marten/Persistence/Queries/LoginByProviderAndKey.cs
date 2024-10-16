namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class LoginByProviderAndKey : ICompiledQuery<MartenIdentityUserLogin, MartenIdentityUserLogin?>
{
    public string LoginProvider { get; init; } = default!;
    public string ProviderKey { get; init; } = default!;

    public Expression<Func<IMartenQueryable<MartenIdentityUserLogin>, MartenIdentityUserLogin?>> QueryIs()
    {
        return q => q
            .SingleOrDefault(x => x.LoginProvider == LoginProvider && x.ProviderKey == ProviderKey);
    }
}