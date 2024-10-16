namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class RoleByNormalizedName<TRole> : ICompiledQuery<TRole, TRole?>
    where TRole : MartenIdentityRole
{
    public string NormalizedName { get; init; } = default!;

    public Expression<Func<IMartenQueryable<TRole>, TRole?>> QueryIs()
    {
        return q => q
            .FirstOrDefault(r => r.NormalizedName == NormalizedName);
    }
}