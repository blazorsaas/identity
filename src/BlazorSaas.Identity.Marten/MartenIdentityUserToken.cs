namespace BlazorSaas.Identity.Marten;

public class MartenIdentityUserToken : IdentityUserToken<Guid>
{
    public Guid Id { get; set; }
}