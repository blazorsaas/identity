namespace BlazorSaas.Identity.Marten;

public class MartenIdentityUserRole : IdentityUserRole<Guid>
{
    public Guid Id { get; set; }
}