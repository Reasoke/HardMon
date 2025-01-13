using System.Net;
using Dapper;
using server.Classes;

namespace server.Repository;

public interface IDevicesRepository {
    Task<IEnumerable<DeviceDto>> GetDevices(int accountId);
    Task AddDevice(int accountId, DeviceRequestDto request);
    Task DeleteDevice(int accountId, int deviceId);
    Task<DeviceDto> GetDeviceDetails(int accountId, int deviceId);
    Task ModifyDeviceDetails(int accountId, int deviceId, DeviceRequestDto request);
    Task<DeviceAccountDto> GetDeviceAccountDetails(Guid authToken);
}

public class DevicesRepository : BaseRepository, IDevicesRepository {
        
    public DevicesRepository(ISettingsProvider settingsProvider) : base(settingsProvider) {
    }
    
    public async Task<IEnumerable<DeviceDto>> GetDevices(int accountId) {
        return await GetConnection().QueryAsync<DeviceDto>(
            @"SELECT deviceId as Id, deviceName as Name
FROM Device
WHERE accountId = @accountId
ORDER BY deviceName", 
            new { accountId });
    }

    public async Task AddDevice(int accountId, DeviceRequestDto request) {
        await GetConnection().ExecuteAsync(
            @"INSERT INTO Device (accountId, deviceName) values (@accountId, @name)", 
            new { accountId, name = request.Name });
    }

    public async Task DeleteDevice(int accountId, int deviceId) {
        await GetConnection().ExecuteAsync(
            @"DELETE FROM Device WHERE accountId = @accountId AND deviceId = @deviceId", 
            new { accountId, deviceId });
    }

    public async Task<DeviceDto> GetDeviceDetails(int accountId, int deviceId) {
        var device = await GetConnection().QueryFirstOrDefaultAsync<DeviceDto>(
            @"SELECT deviceId as Id, deviceName as Name
FROM Device
WHERE accountId = @accountId and deviceId = @deviceId", 
            new { accountId, deviceId });
        if (device == null)
            throw new DomainException(HttpStatusCode.NotFound, "Device is not found");
        return device;
    }

    public async Task ModifyDeviceDetails(int accountId, int deviceId, DeviceRequestDto request) {
        var rowsAffected = await GetConnection().ExecuteAsync(
            @"UPDATE Device
SET  deviceName = @deviceName
WHERE accountId = @accountId and deviceId = @deviceId", 
            new { accountId, deviceId ,deviceName = request.Name});
        
        if (rowsAffected == 0) {
            throw new DomainException(HttpStatusCode.NotFound, "Device is not found");
        }
    }

    public async Task<DeviceAccountDto> GetDeviceAccountDetails(Guid authToken) {
        var device = await GetConnection().QueryFirstOrDefaultAsync<DeviceAccountDto>(
            @"SELECT D.deviceId, D.deviceName, D.accountId
FROM Sensor as S
    JOIN Device D on S.deviceId = D.deviceId
WHERE S.authToken = @authToken", 
            new { authToken });
        if (device == null)
            throw new DomainException(HttpStatusCode.NotFound, "Device is not found");
        return device;
    }
}