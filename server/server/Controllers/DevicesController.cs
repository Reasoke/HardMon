using Microsoft.AspNetCore.Mvc;
using server.Classes;
using server.Services;

namespace server.Controllers;

public class DevicesController : BaseApiController {
    private readonly IDevicesService devicesService;
    
    public DevicesController(IDevicesService devicesService) {
        this.devicesService = devicesService;
    }

    [CheckAccount]
    [HttpGet, Route("api/accounts/{accountId:int}/devices")]
    public async Task<IEnumerable<DeviceDto>> GetDevices(int accountId) {
        return await devicesService.GetDevices(accountId);
    }
    
    [CheckAccount(AccountUserType.Admin)]
    [HttpPost, Route("api/accounts/{accountId:int}/devices")]
    public async Task AddDevice(int accountId, DeviceRequestDto request) {
        await devicesService.AddDevice(accountId, request);
    }
        
    [CheckAccount(AccountUserType.Admin)]
    [HttpDelete, Route("api/accounts/{accountId:int}/devices/{deviceId:int}")]
    public async Task DeleteDevices(int accountId, int deviceId) {
        await devicesService.DeleteDevice(accountId, deviceId);
    }
        
    [CheckAccount]
    [HttpGet, Route("api/accounts/{accountId:int}/devices/{deviceId:int}")]
    public async Task<DeviceDto> GetDeviceDetails(int accountId, int deviceId) {
        return await devicesService.GetDeviceDetails(accountId, deviceId);
    }
    
    [CheckAccount(AccountUserType.Admin)]
    [HttpPost, Route("api/accounts/{accountId:int}/devices/{deviceId:int}")]
    public async Task EditDevice(int accountId, int deviceId, DeviceRequestDto request) {
        await devicesService.EditDeviceDetails(accountId, deviceId, request);
    }
}