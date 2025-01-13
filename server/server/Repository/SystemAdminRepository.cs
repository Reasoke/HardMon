using System.Net;
using Dapper;
using server.Classes;

namespace server.Repository;

public interface ISystemAdminRepository {
    Task<IEnumerable<SensorTypeDto>> GetSensorTypes();
    Task<SensorTypeDto> GetSensorTypeDetails(int sensorTypeId);
    Task AddSensorType(SensorTypeRequest request);
    Task DeleteSensorType(int sensorId);
    Task ModifySensorTypeDetails(int sensorTypeId, SensorTypeRequest request);
    Task<IEnumerable<SubscriptionDto>> GetSubscriptions();
    Task<SubscriptionDto> GetSubscriptionDetails(int subscriptionId);
    Task AddSubscription(SubscriptionRequest request);
    Task ModifySubscriptionDetails(int subscriptionId, SubscriptionRequest request);
    Task DeleteSubscription(int subscriptionId);
    Task<IEnumerable<SubscriptionSensorTypeDto>> GetSubscriptionSensorTypes();
    Task AddSubscriptionSensorType(SubscriptionSensorTypeRequest request);
    Task<SubscriptionSensorTypeDto> GetSubscriptionSensorTypeDetails(int subscriptionId, int sensorTypeId);
    Task DeleteSubscriptionSensorType(int subscriptionId, int sensorTypeId);
}

public class SystemAdminRepository : BaseRepository, ISystemAdminRepository {
        
    public SystemAdminRepository(ISettingsProvider settingsProvider) : base(settingsProvider) {
    }

    public async Task<IEnumerable<SensorTypeDto>> GetSensorTypes() {
        return await GetConnection().QueryAsync<SensorTypeDto>(
            @"SELECT sensorTypeId as Id, typeName as Name, iconName
FROM  SensorType");
    }
    
    public async Task<SensorTypeDto> GetSensorTypeDetails(int sensorTypeId) {
        return await GetConnection().QueryFirstOrDefaultAsync<SensorTypeDto>(
            @"SELECT sensorTypeId as Id, typeName as Name, iconName
FROM  SensorType
WHERE sensorTypeId = @sensorTypeId",
        new {sensorTypeId});
    }

    public async Task AddSensorType(SensorTypeRequest request) {
        await GetConnection().ExecuteAsync(
            @"INSERT INTO SensorType (typeName, iconName) values (@name, @iconName)",
            new {name = request.Name, iconName = request.IconName});
    }

    public async Task DeleteSensorType(int sensorId) {
        await GetConnection().ExecuteAsync(
            @"DELETE FROM  SensorType
WHERE sensorTypeId = @sensorId",
            new {sensorId});
    }

    public async Task ModifySensorTypeDetails(int sensorTypeId, SensorTypeRequest request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"UPDATE SensorType
SET  typeName = @name, iconName = @iconName
WHERE sensorTypeId = @sensorTypeId", 
            new { sensorTypeId, name = request.Name, iconName = request.IconName});
        
        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "SensorType is not found");
        }
    }

    
    
    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptions() {
        return await GetConnection().QueryAsync<SubscriptionDto>(
            @"SELECT subscriptionId as Id, subscriptionName as Name, price
FROM  Subscription");
    }

    public async Task<SubscriptionDto> GetSubscriptionDetails(int subscriptionId) {
        return await GetConnection().QueryFirstOrDefaultAsync<SubscriptionDto>(
            @"SELECT subscriptionId as Id, subscriptionName as Name, price
FROM  Subscription
WHERE subscriptionId = @subscriptionId",
            new {subscriptionId});
    }

    public async Task AddSubscription(SubscriptionRequest request) {
        await GetConnection().ExecuteAsync(
            @"INSERT INTO Subscription (subscriptionName, price) values (@name, @price)",
            new {name = request.Name, price = request.Price});
    }

    public async Task ModifySubscriptionDetails(int subscriptionId, SubscriptionRequest request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"UPDATE Subscription
SET  subscriptionName = @name, price = @price
WHERE subscriptionId = @subscriptionId", 
            new { subscriptionId, name = request.Name, price = request.Price});
        
        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Subscription is not found");
        }
    }

    public async Task DeleteSubscription(int subscriptionId) {
        await GetConnection().ExecuteAsync(
            @"DELETE FROM  Subscription
WHERE subscriptionId = @subscriptionId",
            new {subscriptionId});
    }

    
    
    public async Task<IEnumerable<SubscriptionSensorTypeDto>> GetSubscriptionSensorTypes() {
        return await GetConnection().QueryAsync<SubscriptionSensorTypeDto>(
            @"SELECT S.subscriptionId as SubscriptionId, S.subscriptionName as SubscriptionName,
       ST.sensorTypeId as SensorTypeId, ST.typeName as SensorTypeName
FROM SubscriptionSensorType SST
JOIN Subscription S ON S.subscriptionId = SST.subscriptionTypeId
JOIN SensorType ST ON ST.sensorTypeId = SST.sensorTypeId");
    }

    public async Task AddSubscriptionSensorType(SubscriptionSensorTypeRequest request) {
        await GetConnection().ExecuteAsync(
            @"INSERT INTO SubscriptionSensorType (subscriptionTypeId, sensorTypeId) values (@subscriptionTypeId, @sensorTypeId)",
            new {subscriptionTypeId = request.SubscriptionId, sensorTypeId = request.SensorTypeId});
    }

    public async Task<SubscriptionSensorTypeDto> GetSubscriptionSensorTypeDetails(int subscriptionId, int sensorTypeId) {
        return await GetConnection().QueryFirstOrDefaultAsync<SubscriptionSensorTypeDto>(
            @"SELECT S.subscriptionId as SubscriptionId, S.subscriptionName as SubscriptionName,
       ST.sensorTypeId as SensorTypeId, ST.typeName as SensorTypeName
FROM SubscriptionSensorType SST
JOIN Subscription S ON S.subscriptionId = SST.subscriptionTypeId
JOIN SensorType ST ON ST.sensorTypeId = SST.sensorTypeId
WHERE SST.subscriptionTypeId = @subscriptionId and SSt.sensorTypeId = @sensorTypeId",
            new {subscriptionId, sensorTypeId});
    }

    public async Task DeleteSubscriptionSensorType(int subscriptionId, int sensorTypeId) {
        await GetConnection().ExecuteAsync(
            @"DELETE FROM  SubscriptionSensorType
WHERE subscriptionTypeId = @subscriptionId and sensorTypeId = @sensorTypeId",
            new {subscriptionId, sensorTypeId});
    }
}