using System.Data;
using System.Net;
using Dapper;
using server.Classes;

namespace server.Repository;

public interface ISensorsRepository {
    Task<IEnumerable<SensorTypeDto>> GetSensorTypes(int accountId);
    Task<IEnumerable<SensorDto>> GetSensors(int deviceId);
    Task AddSensor(int deviceId, AddSensorRequest request);
    Task DeleteSensor(int sensorId);
    Task<SensorDto> GetSensorDetails(int sensorId);
    Task ModifySensorDetails(int sensorId, ModifySensorRequest request);

    Task<IEnumerable<SensorDataDto>> GetSensorData(int sensorId,
        int take, int skip, DateTime? from, DateTime? to);

    Task AddSensorData(Guid authToken, string request);
    Task AddAlert(Guid authToken, AddAlertRequest request);

    Task<IEnumerable<AlertDto>> GetAlerts(int sensorId,
        int take, int skip, DateTime? from, DateTime? to);

    Task<AlertDto> GetAlertDetails(int alertId);
    Task ModifyAlertDetails(int alertId, ModifyAlertRequest request);
    Task CheckSensorExist(int sensorId, int accountId);
    Task<string> GetSensorConfig(Guid guid);
}

public class SensorsRepository : BaseRepository, ISensorsRepository {

    public SensorsRepository(ISettingsProvider settingsProvider) : base(settingsProvider) {
    }

    public async Task<IEnumerable<SensorTypeDto>> GetSensorTypes(int accountId) {
        return await GetConnection().QueryAsync<SensorTypeDto>(
            @"SELECT ST.sensorTypeId as Id, ST.typeName as Name, ST.iconName
FROM Account A
     JOIN dbo.Subscription S on A.subscriptionId = S.subscriptionId
     JOIN SubscriptionSensorType SST on S.subscriptionId = SST.subscriptionTypeId
     JOIN dbo.SensorType ST on SST.sensorTypeId = ST.sensorTypeId
WHERE a.accountId = @accountId",
            new { accountId });
    }

    public async Task<IEnumerable<SensorDto>> GetSensors(int deviceId) {
        return await GetConnection().QueryAsync<SensorDto>(
            @"SELECT S.sensorId as Id, S.deviceId, S.sensorTypeId as SensorTypeId, ST.typeName as SensorType, S.config, S.authToken
FROM Sensor as S
JOIN SensorType as ST ON S.sensorTypeId = ST.sensorTypeId
WHERE deviceId = @deviceId
ORDER BY sensorId",
            new { deviceId });
    }

    public async Task AddSensor(int deviceId, AddSensorRequest request) {
        await GetConnection().ExecuteAsync(
            @"INSERT INTO Sensor (sensorTypeId, deviceId, config) VALUES (@sensorTypeId, @deviceId, @config)",
            new { sensorTypeId = request.SensorTypeId, deviceId, config = request.Config });
    }

    public async Task DeleteSensor(int sensorId) {
        await GetConnection().ExecuteAsync(
            @"DELETE FROM Sensor WHERE sensorId = @sensorId",
            new { sensorId });
    }

    public async Task<SensorDto> GetSensorDetails(int sensorId) {
        return await GetConnection().QueryFirstOrDefaultAsync<SensorDto>(
            @"SELECT S.sensorId as Id, S.deviceId, S.sensorTypeId as SensorTypeId, ST.typeName as SensorType, S.config, S.authToken
FROM Sensor as S
JOIN SensorType as ST ON S.sensorTypeId = ST.sensorTypeId
WHERE sensorId = @sensorId",
            new { sensorId });
    }

    public async Task ModifySensorDetails(int sensorId, ModifySensorRequest request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"UPDATE Sensor
SET  config = @config, authToken = @authToken
WHERE sensorId = @sensorId",
            new { sensorId, config = request.Config, authToken = request.AuthToken });

        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Sensor is not found");
        }
    }

    public async Task<IEnumerable<SensorDataDto>> GetSensorData(int sensorId,
        int take, int skip, DateTime? from, DateTime? to) {
        return await GetConnection().QueryAsync<SensorDataDto>(
            @"SELECT sensorDataId as Id, sensorId, created, value
FROM SensorData
WHERE sensorId = @sensorId and (@from IS NULL OR created >= @from) AND (@to IS NULL OR created <= @to)
ORDER BY created DESC
OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
            new { sensorId, take, skip, from, to });
    }

    public async Task AddSensorData(Guid authToken, string request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"INSERT INTO SensorData (sensorId, value) 
SELECT S.sensorId, @value FROM Sensor as S WHERE S.authToken = @authToken",
            new { authToken, value = request });

        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Sensor is not found");
        }
    }

    public async Task AddAlert(Guid authToken, AddAlertRequest request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"INSERT INTO SensorAlerts (sensorId, alert) 
SELECT S.sensorId, @alert FROM Sensor as S WHERE S.authToken = @authToken",
            new { authToken, alert = request.AlertMessage });

        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Sensor is not found");
        }
    }

    public async Task<IEnumerable<AlertDto>> GetAlerts(int sensorId,
        int take, int skip, DateTime? from, DateTime? to) {
        return await GetConnection().QueryAsync<AlertDto>(
                @"SELECT sensorAlertId as Id, sensorId, created, alert as AlertMessage, checked as IsChecked
FROM SensorAlerts
WHERE sensorId = @sensorId and (@from IS NULL OR created >= @from) AND (@to IS NULL OR created <= @to)
ORDER BY created DESC
OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
            new { sensorId, take, skip, from, to });
    }

    public async Task<AlertDto> GetAlertDetails(int alertId) {
        return await GetConnection().QueryFirstOrDefaultAsync<AlertDto>(
            @"SELECT sensorAlertId as Id, sensorId, created, alert as AlertMessage, checked as IsChecked
FROM SensorAlerts
WHERE sensorAlertId = @alertId",
            new { alertId });
    }

    public async Task ModifyAlertDetails(int alertId, ModifyAlertRequest request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"UPDATE SensorAlerts
SET  checked = @isChecked
WHERE sensorAlertId = @alertId",
            new { alertId, isChecked = request.IsChecked });

        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Sensor is not found");
        }
    }

    public async Task CheckSensorExist(int sensorId, int accountId) {
        var found = await GetConnection().QueryFirstOrDefaultAsync<int>(
            @"SELECT 1
FROM Sensor as S
JOIN Device as D ON S.deviceId = D.deviceId
WHERE S.sensorId = @sensorId and D.accountId = @accountId",
            new { sensorId, accountId });

        if (found == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Sensor is not found");
        }
    }

    public async Task<string> GetSensorConfig(Guid guid) {
        var sensorDetails = await GetConnection().QueryFirstOrDefaultAsync<SensorDto>(
        @"SELECT S.sensorId as Id, S.deviceId, S.sensorTypeId as SensorTypeId, ST.typeName as SensorType, S.config, S.authToken
FROM Sensor as S
JOIN SensorType as ST ON S.sensorTypeId = ST.sensorTypeId
WHERE authToken = @authToken",
        new { authToken = guid });
        //if (sensorDetails == null) {
        //    throw new DomainException(HttpStatusCode.NotFound, "Sensor is not found");
        //}
        return sensorDetails?.Config;
    }
}