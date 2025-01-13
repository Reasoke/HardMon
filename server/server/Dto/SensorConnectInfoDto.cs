namespace server;

public class SensorConnectInfoDto {
    public Guid AuthToken { get; set; }
    public int SensorType { get; set; }
    public string RemoteIp { get; set; }
    public DateTime Registered { get; set; }
}