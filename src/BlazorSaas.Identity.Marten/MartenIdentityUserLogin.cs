namespace BlazorSaas.Identity.Marten;

public class MartenIdentityUserLogin : IdentityUserLogin<Guid>
{
    public Guid Id { get; set; }
}