namespace server {

    public class AccountDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public AccountUserType UserType { get; set; }
        public AccountSubscription Subscription { get; set; }
    }
}

