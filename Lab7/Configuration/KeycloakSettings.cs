namespace Lab7.Configuration;

public class KeycloakSettings
{
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public bool RequireHttpsMetadata { get; set; } 
}
