namespace BlazorSaas.Identity.Marten.Persistence.Queries;

public class LoginsByUser : ICompiledListQuery<MartenIdentityUserLogin>
{
    public Guid UserId { get; init; }

    public Expression<Func<IMartenQueryable<MartenIdentityUserLogin>, IEnumerable<MartenIdentityUserLogin>>> QueryIs()
    {
        return q => q
            .Where(x => x.UserId == UserId);
    }
}