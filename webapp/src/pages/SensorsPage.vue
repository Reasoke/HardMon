<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ $t('sensorPage.title') }} №{{ deviceId }}</h2>

          <div class="sensor-list">
            <div v-for="sensor in sensors" :key="sensor.id" class="sensor-card"
            @click="goToAlerts(this.accountId, this.deviceId, sensor.id)"
            @keydown.enter="goToAlerts(this.accountId, this.deviceId, sensor.id)"
            @keydown.space.prevent="goToAlerts(this.accountId, this.deviceId, sensor.id)"
            tabindex="0" role="button">
              <div class="device-type">{{ sensor.sensorType }}</div>
              <div class="device-config">{{ sensor.config }}</div>
              <div class="buttons">
                <button class="edit-button"
                @click.stop="goToEditSensor(this.accountId, this.deviceId, sensor.id)">
                {{ $t('sensorPage.editButton') }}</button>
              </div>
            </div>
          </div>

          <button class="add-button" @click="goToAddSensor(this.accountId, this.deviceId)">
            ➕ {{ $t('sensorPage.addButton') }}</button>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message"><p>{{ $t('sensorPage.loading') }}</p></div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('sensorPage.authMessage') }}</router-link>
      </div>
    </div>
  </main>
</template>

<script>
import SidebarComponent from '@/components/SidebarComponent.vue';
import { mapState, mapGetters, mapActions } from 'vuex';

export default {
  props: ['accountId', 'deviceId'],
  components: {
    SidebarComponent,
  },
  data() {
    return {
      loading: false,
    };
  },
  computed: {
    ...mapState(['userInfo', 'sensors']),
    ...mapGetters(['isAuthenticated']),
  },
  created() {
    this.init();
  },
  methods: {
    ...mapActions(['fetchUserInfo', 'getSensors']),
    async init() {
      this.loading = true;
      if (!this.userInfo) await this.fetchUserInfo();
      await this.getSensors({ accountId: this.accountId, deviceId: this.deviceId });
      this.loading = false;
    },
    goToAlerts(accountId, deviceId, sensorId) {
      this.$router.push(`/accounts/${accountId}/devices/${deviceId}/sensors/${sensorId}/alerts`);
    },
    goToAddSensor(accountId, deviceId) {
      this.$router.push(`/accounts/${accountId}/devices/${deviceId}/sensors/create`);
    },
    goToEditSensor(accountId, deviceId, sensorId) {
      this.$router.push(`/accounts/${accountId}/devices/${deviceId}/sensors/${sensorId}`);
    },
  },
};
</script>

<style scoped>
.not_authorised_message {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 75vh;
}

.not_authorised_message p, .loading_message p {
  font-size: 1.5rem;
  font-weight: bold;
}

.loading_message {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 75vh;
}

.container {
  display: flex;
}

/* Main section */
.main {
  flex: 1;
  padding: 40px;
  font-size: 1.2rem;
}

.sensor-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-bottom: 30px;
  margin-top: 30px;
}

.sensor-card {
  all: unset;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background-color: #f0f4f8;
  border: 1px solid #ccc;
  cursor: pointer;
}

.sensor-card:hover {
  background-color: #e1ecf4;
}

.edit-button {
  padding: 5px 10px;
  background-color: #007bff;
  color: white;
  border: none;
  cursor: pointer;
  font-size: 0.9rem;
}

.edit-button:hover {
  background-color: #0056b3;
}

.device-type, .add-button, .edit-button {
  font-size: 1.2rem;
}

.add-button:hover {
  background-color: #1f7855;
}
</style>
