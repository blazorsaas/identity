namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class UsersInRole<TUser> : ICompiledListQuery<MartenIdentityUserRole>
    where TUser : MartenIdentityUser
{
    public Guid RoleId { get; init; }

    public IList<TUser> Users { get; } = [];

    public Expression<Func<IMartenQueryable<MartenIdentityUserRole>, IEnumerable<MartenIdentityUserRole>>> QueryIs()
    {
        return q => q
            .Where(x => x.RoleId == RoleId)
            .Include(x => x.UserId, Users);
    }
}