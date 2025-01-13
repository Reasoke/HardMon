namespace server;

public class AccountDetailsDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<UserAccountDto> AccountUsers { get; set; }
    public AccountSubscription Subscription { get; set; }
}