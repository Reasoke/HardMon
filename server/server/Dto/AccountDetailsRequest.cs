namespace server;

public class AccountDetailsRequest {
    public int Id { get; set; }
    public string Name { get; set; }
    public AccountSubscription Subscription { get; set; }
}