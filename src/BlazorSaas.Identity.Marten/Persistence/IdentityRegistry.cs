using Marten.Schema;

namespace BlazorSaas.Identity.Marten.Persistence;

public abstract class IdentityRegistry<TUser, TRole>
    : MartenRegistry
    where TUser : MartenIdentityUser
    where TRole : MartenIdentityRole
{
    protected IdentityRegistry()
    {
        For<TUser>()
#pragma warning disable CS8603 // Possible null reference return.
            .UniqueIndex(UniqueIndexType.Computed, x => x.NormalizedUserName, x => x.NormalizedEmail);
#pragma warning restore CS8603 // Possible null reference return.

        For<MartenIdentityUserClaim>()
            .ForeignKey<TUser>(x => x.UserId);

        For<MartenIdentityUserLogin>()
            .Index(x => new { x.LoginProvider, x.ProviderKey })
            .ForeignKey<TUser>(x => x.UserId);

        For<MartenIdentityUserToken>()
            .Index(x => new { x.UserId, x.LoginProvider, x.Name })
            .ForeignKey<TUser>(x => x.UserId);

        For<MartenIdentityUserRole>()
            .Index(x => new { x.UserId, x.RoleId })
            .ForeignKey<TUser>(x => x.UserId)
            .ForeignKey<TRole>(x => x.RoleId);

        For<TRole>()
#pragma warning disable CS8603 // Possible null reference return.
            .UniqueIndex(UniqueIndexType.Computed, x => x.NormalizedName);
#pragma warning restore CS8603 // Possible null reference return.

        For<MartenIdentityRoleClaim>()
            .ForeignKey<TRole>(x => x.RoleId);
    }
}