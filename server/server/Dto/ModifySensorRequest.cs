namespace server;

public class ModifySensorRequest {
    public string Config { get; set; }
    public Guid AuthToken { get; set; }
}