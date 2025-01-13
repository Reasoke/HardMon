using server.Repository;

namespace server.Services {
    public interface IDevicesService {
        Task<IEnumerable<DeviceDto>> GetDevices(int accountId);
        Task AddDevice(int accountId, DeviceRequestDto request);
        Task DeleteDevice(int accountId, int deviceId);
        Task<DeviceDto> GetDeviceDetails(int accountId, int deviceId);
        Task EditDeviceDetails(int accountId, int deviceId, DeviceRequestDto request);
    }

    public class DevicesService : IDevicesService {
        private readonly IDevicesRepository devicesRepository;

        public DevicesService(IDevicesRepository devicesRepository) {
            this.devicesRepository = devicesRepository;
        }

        public async Task<IEnumerable<DeviceDto>> GetDevices(int accountId) {
            return await devicesRepository.GetDevices(accountId);
        }

        public async Task AddDevice(int accountId, DeviceRequestDto request) {
            await devicesRepository.AddDevice(accountId, request);
        }

        public async Task DeleteDevice(int accountId, int deviceId) {
            await devicesRepository.DeleteDevice(accountId, deviceId);
        }

        public async Task<DeviceDto> GetDeviceDetails(int accountId, int deviceId) {
            return await devicesRepository.GetDeviceDetails(accountId, deviceId);
        }

        public async Task EditDeviceDetails(int accountId, int deviceId, DeviceRequestDto request) {
            await devicesRepository.ModifyDeviceDetails(accountId, deviceId, request);
        }
    }
}