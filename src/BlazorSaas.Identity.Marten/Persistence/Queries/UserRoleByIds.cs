namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class UserRoleByIds : ICompiledQuery<MartenIdentityUserRole, MartenIdentityUserRole?>
{
    public Guid RoleId { get; init; }
    public Guid UserId { get; init; }

    public Expression<Func<IMartenQueryable<MartenIdentityUserRole>, MartenIdentityUserRole?>> QueryIs()
    {
        return q => q
            .SingleOrDefault(r => r.RoleId == RoleId && r.UserId == UserId);
    }
}