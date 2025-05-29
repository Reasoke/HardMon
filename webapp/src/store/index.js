import { createStore } from 'vuex';
import axios from 'axios';

export default createStore({
  state: {
    userInfo: null,
    accounts: [],
    devices: [],
    sensors: [],
    alerts: [],
    activeSensors: [],
    sensorTypes: [],
    sensorData: [],
  },
  mutations: {
    updateUserInfo(state, userInfo) {
      state.userInfo = userInfo;
    },
    updateAccountsInfo(state, accounts) {
      state.accounts = accounts;
    },
    removeAccount(state, accountId) {
      state.accounts = state.accounts.filter((acc) => acc.id !== accountId);
    },
    updateDevicesInfo(state, devices) {
      state.devices = devices;
    },
    removeDevice(state, deviceId) {
      state.devices = state.devices.filter((device) => device.id !== deviceId);
    },
    updateAlertsInfo(state, alerts) {
      state.alerts = alerts;
    },
    updateSensorsInfo(state, sensors) {
      state.sensors = sensors;
    },
    removeSensor(state, sensorId) {
      state.sensors = state.sensors.filter((sensor) => sensor.id !== sensorId);
    },
    updateSensorTypesInfo(state, sensorTypes) {
      state.sensorTypes = sensorTypes;
    },
    updateSensorDataInfo(state, sensorData) {
      state.sensorData = sensorData;
    },
    updateActiveSensorsInfo(state, activeSensors) {
      state.activeSensors = activeSensors;
    },
  },
  actions: {
    async postUserInfo({ commit }, { email, password }) {
      try {
        const response = await axios.post('api/profile/login', {
          email,
          password,
        });
        commit('updateUserInfo', response.data);
      } catch (error) {
        console.error('Login error:', error);
        if (error.response?.status === 401) {
          alert('Невірна пошта або пароль');
        }
      }
    },

    async postRegisterUser({ commit }, { name, email, password }) {
      try {
        const response = await axios.post('/api/profile/register', {
          name,
          email,
          password,
        });
        commit('updateUserInfo', response.data);
      } catch (error) {
        console.error('Register error:', error);
      }
    },

    async logout({ commit }) {
      try {
        await axios.get('/api/profile/logout');
        commit('updateUserInfo', null);
      } catch (error) {
        console.error('Logout error:', error);
      }
    },

    async fetchUserInfo({ commit }) {
      try {
        const response = await axios.get('/api/profile');
        commit('updateUserInfo', response.data);
      } catch (error) {
        console.warn('Пользователь не авторизован');
        // commit('updateUserInfo', null);
      }
    },

    async updateProfile({ commit }, userInfo) {
      try {
        const response = await axios.post('/api/profile', userInfo);
        commit('updateUserInfo', response.data);
        return true;
      } catch (error) {
        console.error('Error updating profile:', error);
        return false;
      }
    },

    // ACCOUNTS
    async getAccounts({ commit }) {
      try {
        const response = await axios.get('/api/accounts');
        commit('updateAccountsInfo', response.data);
      } catch (error) {
        console.warn('Error getting accounts:', error);
      }
    },

    async getAccountById({ commit }, id) {
      try {
        const response = await axios.get(`/api/accounts/${id}`);
        commit('updateAccountsInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Помилка отримання акаунта за ID:', error);
        throw error;
      }
    },

    async addAccount({ commit }, accountData) {
      try {
        await axios.post('/api/accounts', accountData);
        commit('updateAccountsInfo', accountData);
      } catch (error) {
        console.error('Error adding account:', error);
      }
    },

    async updateAccount({ commit }, { id, updatedData }) {
      try {
        console.log('Отправляемые данные:', updatedData);
        await axios.post(`/api/accounts/${id}`, updatedData);
        commit('updateAccountsInfo', updatedData);
      } catch (error) {
        console.error('Error updating account:', error);
      }
    },

    async deleteAccount({ commit }, id) {
      try {
        await axios.delete(`/api/accounts/${id}`);
        commit('removeAccount', id);
      } catch (error) {
        console.error('Error deleting account:', error);
      }
    },

    // DEVICES
    async getDevices({ commit }, accountId) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/devices`);
        commit('updateDevicesInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting devices:', error);
        throw error;
      }
    },

    async getDeviceById({ commit }, { accountId, deviceId }) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/devices/${deviceId}`);
        commit('updateDevicesInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting device by id:', error);
        throw error;
      }
    },

    async addDevice({ commit }, { accountId, device }) {
      try {
        console.log(device);
        await axios.post(`/api/accounts/${accountId}/devices`, device);
        commit('updateAccountsInfo', device);
      } catch (error) {
        console.error('Error adding account:', error);
      }
    },

    async updateDevice({ commit }, { accountId, deviceId, updatedDevice }) {
      try {
        console.log('Отправляемые данные:', updatedDevice);
        await axios.post(`/api/accounts/${accountId}/devices/${deviceId}`, updatedDevice);
        commit('updateAccountsInfo', updatedDevice);
      } catch (error) {
        console.error('Error updating account:', error);
      }
    },

    async deleteDevice({ commit }, { accountId, deviceId }) {
      try {
        await axios.delete(`/api/accounts/${accountId}/devices/${deviceId}`);
        commit('removeDevice', deviceId);
      } catch (error) {
        console.error('Error deleting account:', error);
      }
    },

    // SENSORS
    async getSensorTypes({ commit }, accountId) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/sensorTypes`);
        commit('updateSensorTypesInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting devices:', error);
        throw error;
      }
    },

    async getSensorData({ commit }, { accountId, sensorId }) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/sensors/${sensorId}/data`);
        commit('updateSensorDataInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting devices:', error);
        throw error;
      }
    },

    async getActiveSensors({ commit }, accountId) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/sensors/active`);
        commit('updateActiveSensorsInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting devices:', error);
        throw error;
      }
    },

    async getSensors({ commit }, { accountId, deviceId }) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/devices/${deviceId}/sensors`);
        commit('updateSensorsInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting devices:', error);
        throw error;
      }
    },

    async getSensorById({ commit }, { accountId, deviceId, sensorId }) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/devices/${deviceId}/sensors/${sensorId}`);
        commit('updateSensorsInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting device by id:', error);
        throw error;
      }
    },

    async addSensor({ commit }, { accountId, deviceId, sensor }) {
      try {
        await axios.post(`/api/accounts/${accountId}/devices/${deviceId}/sensors`, sensor);
        commit('updateSensorsInfo', sensor);
      } catch (error) {
        if (error.response && error.response.status === 403) {
          throw new Error('У вас немає прав для додавання сенсора');
        } else {
          console.error('Error adding sensor:', error);
          throw new Error('Помилка при додаванні сенсора');
        }
      }
    },

    async updateSensor({ commit }, {
      accountId, deviceId, sensorId, updatedSensor,
    }) {
      try {
        // console.log('Отправляемые данные:', updatedSensor);
        await axios.post(`/api/accounts/${accountId}/devices/${deviceId}/sensors/${sensorId}`, updatedSensor);
        commit('updateSensorsInfo', updatedSensor);
      } catch (error) {
        if (error.response && error.response.status === 403) {
          throw new Error('У вас немає прав для редагування сенсора');
        }
        console.error('Error updating account:', error);
      }
    },

    async deleteSensor({ commit }, { accountId, deviceId, sensorId }) {
      try {
        await axios.delete(`/api/accounts/${accountId}/devices/${deviceId}/sensors/${sensorId}`);
        commit('removeSensor', sensorId);
      } catch (error) {
        if (error.response && error.response.status === 403) {
          throw new Error('У вас немає прав для редагування сенсора');
        }
        console.error('Error deleting account:', error);
      }
    },

    // ALERTS
    async getAlerts({ commit }, { accountId, sensorId }) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/sensors/${sensorId}/alert`);
        commit('updateAlertsInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting devices:', error);
        throw error;
      }
    },

    async getAlertById({ commit }, { accountId, sensorId, alertId }) {
      try {
        const response = await axios.get(`/api/accounts/${accountId}/sensors/${sensorId}/alert/${alertId}`);
        commit('updateAlertsInfo', response.data);
        return response.data;
      } catch (error) {
        console.error('Error getting device by id:', error);
        throw error;
      }
    },

    async updateAlert({ commit }, {
      accountId, sensorId, alertId, updatedAlert,
    }) {
      try {
        await axios.post(`/api/accounts/${accountId}/sensors/${sensorId}/alert/${alertId}`, updatedAlert);
        commit('updateAlertsInfo', updatedAlert);
      } catch (error) {
        if (error.response && error.response.status === 403) {
          throw new Error('У вас немає прав для редагування сенсора');
        }
        console.error('Error updating account:', error);
      }
    },
  },
  getters: {
    isAuthenticated: (state) => !!state.userInfo,
  },
});
