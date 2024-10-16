namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class RolesByUserId<TRole> : ICompiledListQuery<MartenIdentityUserRole>
    where TRole : MartenIdentityRole
{
    public Guid UserId { get; init; }

    public IList<TRole> Roles { get; } = [];

    public Expression<Func<IMartenQueryable<MartenIdentityUserRole>, IEnumerable<MartenIdentityUserRole>>> QueryIs()
    {
        return q => q
            .Where(x => x.UserId == UserId)
            .Include(x => x.RoleId, Roles);
        ;
    }
}