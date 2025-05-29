using Microsoft.AspNetCore.Mvc;
using server.Classes;
using server.Services;

namespace server.Controllers;

[CheckSysAdminRights]
public class SystemAdminController : BaseApiController {
    private readonly ISystemAdminService systemAdminService;

    public SystemAdminController(ISystemAdminService systemAdminService) {
        this.systemAdminService = systemAdminService;
    }

    [HttpGet, Route("api/sysAdmin/sensorType")]
    public async Task<IEnumerable<SensorTypeDto>> GetSensorTypes() {
        return await systemAdminService.GetSensorTypes();
    }
    
    [HttpGet, Route("api/sysAdmin/sensorType/{sensorTypeId:int}")]
    public async Task<SensorTypeDto> GetSensorTypeDetails(int sensorTypeId) {
        return await systemAdminService.GetSensorTypeDetails(sensorTypeId);
    }
    
    [HttpPost, Route("api/sysAdmin/sensorType")]
    public async Task AddSensorType(SensorTypeRequest request) {
        await systemAdminService.AddSensorType(request);
    }
    
    [HttpPost, Route("api/sysAdmin/sensorType/{sensorTypeId:int}")]
    public async Task ModifySensorTypeDetails(int sensorTypeId, SensorTypeRequest request) {
        await systemAdminService.ModifySensorTypeDetails(sensorTypeId, request);
    }
    
    [HttpDelete, Route("api/sysAdmin/sensorType/{sensorTypeId:int}")]
    public async Task DeleteSensorType(int sensorTypeId) {
        await systemAdminService.DeleteSensorType(sensorTypeId);
    }
    
    
    
    [HttpGet, Route("api/sysAdmin/subscription")]
    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptions() {
        return await systemAdminService.GetSubscriptions();
    }
    
    [HttpGet, Route("api/sysAdmin/subscription/{subscriptionId:int}")]
    public async Task<SubscriptionDto> GetSubscriptionDetails(int subscriptionId) {
        return await systemAdminService.GetSubscriptionDetails(subscriptionId);
    }
    
    [HttpPost, Route("api/sysAdmin/subscription")]
    public async Task AddSubscription(SubscriptionRequest request) {
        await systemAdminService.AddSubscription(request);
    }
    
    [HttpPost, Route("api/sysAdmin/subscription/{subscriptionId:int}")]
    public async Task ModifySubscriptionDetails(int subscriptionId, SubscriptionRequest request) {
        await systemAdminService.ModifySubscriptionDetails(subscriptionId, request);
    }
    
    [HttpDelete, Route("api/sysAdmin/subscription/{subscriptionId:int}")]
    public async Task DeleteSubscription(int subscriptionId) {
        await systemAdminService.DeleteSubscription(subscriptionId);
    }



    [HttpGet, Route("api/sysAdmin/userType")]
    public async Task<IEnumerable<UserTypeDto>> GetUserTypes() {
        return await systemAdminService.GetUserTypes();
    }
    [HttpPost, Route("api/sysAdmin/userType")]
    public async Task AddUserType(UserTypeRequest request) {
        await systemAdminService.AddUserTypes(request);
    }
    [HttpDelete, Route("api/sysAdmin/userType/{userTypeId:int}")]
    public async Task DeleteUserType(int userTypeId) {
        await systemAdminService.DeleteUserType(userTypeId);
    }


    [HttpGet, Route("api/sysAdmin/account")]
    public async Task<IEnumerable<AccountAdminDto>> GetAccounts() {
        return await systemAdminService.GetAccounts();
    }
    [HttpDelete, Route("api/sysAdmin/account/{accountId:int}")]
    public async Task DeleteAccount(int accountId) {
        await systemAdminService.DeleteAccount(accountId);
    }



    [HttpGet, Route("api/sysAdmin/user")]
    public async Task<IEnumerable<UserDto>> GetUsers() {
        return await systemAdminService.GetUsers();
    }
    [HttpDelete, Route("api/sysAdmin/user/{userId:int}")]
    public async Task DeleteUser(int userId) {
        await systemAdminService.DeleteUser(userId);
    }



    [HttpGet, Route("api/sysAdmin/subscriptionSensorType")]
    public async Task<IEnumerable<SubscriptionSensorTypeDto>> GetSubscriptionSensorTypes() {
        return await systemAdminService.GetSubscriptionSensorTypes();
    }
    
    [HttpPost, Route("api/sysAdmin/subscriptionSensorType")]
    public async Task AddSubscription(SubscriptionSensorTypeRequest request) {
        await systemAdminService.AddSubscriptionSensorType(request);
    }
    
    [HttpGet, Route("api/sysAdmin/subscriptionSensorType/{subscriptionId:int}/{sensorTypeId:int}")]
    public async Task<SubscriptionSensorTypeDto> GetSubscriptionSensorTypeDetails(int subscriptionId, int sensorTypeId) {
        return await systemAdminService.GetSubscriptionSensorTypeDetails(subscriptionId, sensorTypeId);
    }
    
    [HttpDelete, Route("api/sysAdmin/subscriptionSensorType/{subscriptionId:int}/{sensorTypeId:int}")]
    public async Task DeleteSubscriptionSensorType(int subscriptionId, int sensorTypeId) {
        await systemAdminService.DeleteSubscriptionSensorType(subscriptionId, sensorTypeId);
    }
}