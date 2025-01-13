namespace server;

public class SensorDto {
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public int SensorTypeId { get; set; }
    public string SensorType { get; set; }
    public string Config { get; set; }
    public Guid AuthToken { get; set; }
}