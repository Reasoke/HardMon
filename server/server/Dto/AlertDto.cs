namespace server;

public class AlertDto {
    public string Id { get; set; }
    public int SensorId { get; set; }
    public DateTime Created { get; set; }
    public string AlertMessage { get; set; }
    public bool IsChecked { get; set; }
}