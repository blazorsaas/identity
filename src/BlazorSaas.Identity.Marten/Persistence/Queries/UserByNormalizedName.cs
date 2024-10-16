namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class UserByNormalizedName<TUser> : ICompiledQuery<TUser, TUser?>
    where TUser : MartenIdentityUser
{
    public string NormalizedUserName { get; init; } = default!;

    public Expression<Func<IMartenQueryable<TUser>, TUser?>> QueryIs()
    {
        return q => q
            .FirstOrDefault(x => x.NormalizedUserName == NormalizedUserName);
    }
}