<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ $t('alertList.title') }} №{{ sensorId }}</h2>

          <div class="alert-list">
            <div v-for="alert in alerts" :key="alert.id" class="alert-card">
              <div class="alert-type">{{ alert.alertMessage }}</div>
              <div class="alert-config">{{ alert.created }}</div>
              <div class="buttons">
                <button class="edit-button"
                @click.stop="goToEditAlert(this.accountId, this.deviceId, this.sensorId, alert.id)">
                {{ $t('alertList.editButton') }}</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message"><p>{{ $t('alertList.loading') }}</p></div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('alertList.authMessage') }}</router-link>
      </div>
    </div>
  </main>
</template>

<script>
import SidebarComponent from '@/components/SidebarComponent.vue';
import { mapState, mapGetters, mapActions } from 'vuex';

export default {
  props: ['accountId', 'deviceId', 'sensorId'],
  components: {
    SidebarComponent,
  },
  data() {
    return {
      loading: false,
    };
  },
  computed: {
    ...mapState(['userInfo', 'alerts']),
    ...mapGetters(['isAuthenticated']),
  },
  created() {
    this.init();
  },
  methods: {
    ...mapActions(['fetchUserInfo', 'getAlerts']),
    async init() {
      this.loading = true;
      if (!this.userInfo) await this.fetchUserInfo();
      console.log(this.sensotId);
      await this.getAlerts({ accountId: this.accountId, sensorId: this.sensorId });
      this.loading = false;
    },
    goToEditAlert(accountId, deviceId, sensorId, alertId) {
      this.$router.push(`/accounts/${accountId}/devices/${deviceId}/sensors/${sensorId}/alerts/${alertId}`);
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

.alert-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-bottom: 30px;
  margin-top: 30px;
}

.alert-card {
  all: unset;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background-color: #f0f4f8;
  border: 1px solid #ccc;
  cursor: pointer;
}

.alert-card:hover {
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

.alert-type, .add-button, .edit-button {
  font-size: 1.2rem;
}

.add-button:hover {
  background-color: #1f7855;
}
</style>
