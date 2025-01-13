using server.Repository;

namespace server.Services {
    public interface ISystemAdminService {
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

    public class SystemAdminService : ISystemAdminService {
        private readonly ISystemAdminRepository systemAdminRepository;

        public SystemAdminService(ISystemAdminRepository systemAdminRepository) {
            this.systemAdminRepository = systemAdminRepository;
        }

        public async Task<IEnumerable<SensorTypeDto>> GetSensorTypes() {
            return await systemAdminRepository.GetSensorTypes();
        }

        public async Task<SensorTypeDto> GetSensorTypeDetails(int sensorTypeId) {
            return await systemAdminRepository.GetSensorTypeDetails(sensorTypeId);
        }
        
        public async Task AddSensorType(SensorTypeRequest request) {
            await systemAdminRepository.AddSensorType(request);
        }

        public async Task DeleteSensorType(int sensorId) {
            await systemAdminRepository.DeleteSensorType(sensorId);
        }

        public async Task ModifySensorTypeDetails(int sensorTypeId, SensorTypeRequest request) {
            await systemAdminRepository.ModifySensorTypeDetails(sensorTypeId, request);
        }
        

        
        public async Task<IEnumerable<SubscriptionDto>> GetSubscriptions() {
            return await systemAdminRepository.GetSubscriptions();
        }

        public async Task<SubscriptionDto> GetSubscriptionDetails(int subscriptionId) {
            return await systemAdminRepository.GetSubscriptionDetails(subscriptionId);
        }

        public async Task AddSubscription(SubscriptionRequest request) {
            await systemAdminRepository.AddSubscription(request);
        }

        public async Task ModifySubscriptionDetails(int subscriptionId, SubscriptionRequest request) {
            await systemAdminRepository.ModifySubscriptionDetails(subscriptionId, request);
        }

        public async Task DeleteSubscription(int subscriptionId) {
            await systemAdminRepository.DeleteSubscription(subscriptionId);
        }

        
        
        public async Task<IEnumerable<SubscriptionSensorTypeDto>> GetSubscriptionSensorTypes() {
            return await systemAdminRepository.GetSubscriptionSensorTypes();
        }

        public async Task AddSubscriptionSensorType(SubscriptionSensorTypeRequest request) {
            await systemAdminRepository.AddSubscriptionSensorType(request);
        }

        public async Task<SubscriptionSensorTypeDto> GetSubscriptionSensorTypeDetails(int subscriptionId, int sensorTypeId) {
            return await systemAdminRepository.GetSubscriptionSensorTypeDetails(subscriptionId, sensorTypeId);
        }

        public async Task DeleteSubscriptionSensorType(int subscriptionId, int sensorTypeId) {
            await systemAdminRepository.DeleteSubscriptionSensorType(subscriptionId, sensorTypeId);
        }
    }
}