namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class UserByNormalizedEmail<TUser> : ICompiledQuery<TUser, TUser?>
    where TUser : MartenIdentityUser
{
    public string NormalizedEmail { get; init; } = default!;

    public Expression<Func<IMartenQueryable<TUser>, TUser?>> QueryIs()
    {
        return q => q
            .FirstOrDefault(x => x.NormalizedEmail == NormalizedEmail);
    }
}