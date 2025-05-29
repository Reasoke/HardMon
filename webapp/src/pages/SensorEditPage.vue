<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ isEdit ? $t('sensorEdit.titleEdit') : $t('sensorEdit.titleAdd') }}</h2>

          <form @submit.prevent="saveSensor" class="edit-form">
            <label for="sensor-type">
              {{ $t('sensorEdit.sensorType') }}:
              <select v-model="sensor.sensorTypeId" required>
                <option disabled value="">{{ $t('sensorEdit.selectType') }}</option>
                <option v-for="type in sensorTypes" :key="type.id" :value="type.id">
                  {{ type.name }}
                </option>
              </select>
            </label>

            <label for="sensor-config">
              {{$t('sensorEdit.configPlaceholder')}}:
              <input id="sensor-config" v-model="sensor.config"
              type="text" :placeholder="$t('sensorEdit.configPlaceholder')" />
            </label>

            <label for="sensor-config">
              {{$t('sensorEdit.authTokenPlaceholder')}}:
              <input id="sensor-config" v-model="sensor.authToken"
              type="text" :placeholder="$t('sensorEdit.authTokenPlaceholder')" />
            </label>

            <div v-if="isEdit && activeSensors.length">
              <h3>{{ $t('sensorEdit.activeSensorsTitle') }}</h3>
              <table>
                <thead>
                  <tr>
                    <th>{{ $t('sensorEdit.authToken') }}</th>
                    <th>{{ $t('sensorEdit.registered') }}</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="sensor in activeSensors" :key="sensor.authToken">
                    <td>{{ sensor.authToken }}</td>
                    <td>{{ sensor.registered }}</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="buttons">
              <button type="submit" class="save-button">
                {{ isEdit ? $t('sensorEdit.saveButton') : $t('sensorEdit.createButton') }}
              </button>

              <button
                v-if="isEdit"
                type="button"
                class="delete-button"
                @click="handleDeleteSensor">
                {{ $t('sensorEdit.deleteButton') }}
              </button>
            </div>
          </form>

          <div v-if="isEdit && sensorData.length">
            <h3>{{ $t('sensorEdit.sensorDataTitle') }}</h3>
            <table>
              <thead>
                <tr>
                  <th>{{ $t('sensorEdit.time') }}</th>
                  <th>{{ $t('sensorEdit.value') }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="data in sensorData" :key="data.id">
                  <td>{{ data.created }}</td>
                  <td>{{ data.value }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message">
        <p>{{ $t('sensorEdit.loading') }}</p>
      </div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('sensorEdit.authMessage') }}</router-link>
      </div>
    </div>
  </main>
</template>

<script>
import SidebarComponent from '@/components/SidebarComponent.vue';
import { mapState, mapGetters, mapActions } from 'vuex';

export default {
  components: {
    SidebarComponent,
  },
  props: ['accountId', 'deviceId', 'sensorId'],
  data() {
    return {
      loading: false,
      isEdit: false,
      sensor: {
        config: '',
        sensorTypeId: '',
      },
      sensorTypes: [],
      sensorData: [],
      activeSensors: [],
    };
  },
  computed: {
    ...mapState(['userInfo']),
    ...mapGetters(['isAuthenticated']),
  },
  created() {
    this.init();
  },
  methods: {
    ...mapActions([
      'fetchUserInfo',
      'getSensorById',
      'addSensor',
      'updateSensor',
      'getSensorTypes',
      'getSensorData',
      'getActiveSensors',
      'deleteSensor',
    ]),

    async init() {
      this.loading = true;
      if (!this.userInfo) {
        await this.fetchUserInfo();
      }

      try {
        this.sensorTypes = await this.getSensorTypes(this.accountId);
      } catch (err) {
        alert('Error loading sensor types');
      }

      if (this.sensorId) {
        this.isEdit = true;
        try {
          const sensor = await this.getSensorById({
            accountId: this.accountId,
            deviceId: this.deviceId,
            sensorId: this.sensorId,
          });
          this.sensor = sensor;

          this.sensorData = await this.getSensorData({
            accountId: this.accountId,
            sensorId: this.sensorId,
          });

          this.activeSensors = await this.getActiveSensors(this.accountId);
        } catch (err) {
          alert('Sensor download error');
          this.$router.push(`/accounts/${this.accountId}/devices/${this.deviceId}/sensors`);
        }
      }

      this.loading = false;
    },

    async saveSensor() {
      try {
        if (this.isEdit) {
          await this.updateSensor({
            accountId: this.accountId,
            deviceId: this.deviceId,
            sensorId: this.sensor.id,
            updatedSensor: {
              config: this.sensor.config,
              authToken: this.sensor.authToken,
            },
          });
        } else {
          await this.addSensor({
            accountId: this.accountId,
            deviceId: this.deviceId,
            sensor: this.sensor,
          });
        }
        this.$router.push(`/accounts/${this.accountId}/devices/${this.deviceId}/sensors`);
      } catch (err) {
        alert(err.message);
      }
    },

    async handleDeleteSensor() {
      try {
        await this.deleteSensor({
          accountId: this.accountId,
          deviceId: this.deviceId,
          sensorId: this.sensor.id,
        });
        this.$router.push(`/accounts/${this.accountId}/devices/${this.deviceId}/sensors`);
      } catch (err) {
        alert('Account deletion error');
      }
    },
  },
};
</script>

<style scoped>
.container {
  display: flex;
}

.main {
  flex: 1;
  padding: 40px;
}

.edit-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
  width: 100%;
  padding-top: 20px;
}

.edit-form input,
.edit-form select {
  padding: 10px;
  font-size: 1rem;
  border: 1px solid #ccc;
}

#sensor-config {
  margin-top: 30px;
  width: 100%;
  padding: 10px;
  font-size: 1rem;
  border: 1px solid #ccc;
  box-sizing: border-box;
}

.buttons {
  display: flex;
  gap: 15px;
}

.save-button {
  background-color: #28a745;
  color: white;
  border: none;
  padding: 10px 15px;
  cursor: pointer;
  margin-bottom: 30px;
}

.save-button:hover {
  background-color: #218838;
}

.delete-button {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 10px 15px;
  cursor: pointer;
  margin-bottom: 30px;
}

.delete-button:hover {
  background-color: #c82333;
}

.delete-button, .save-button, .edit-form {
  font-size: 1.2rem;
}

.loading_message,
.not_authorised_message {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 75vh;
}

.loading_message p,
.not_authorised_message p {
  font-size: 1.5rem;
  font-weight: bold;
}

table {
  margin-top: 30px;
  border-collapse: collapse;
  width: 100%;
}

th, td {
  padding: 10px;
  border: 1px solid #ccc;
}
</style>
