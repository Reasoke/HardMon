<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ $t('devices.accountDevicesTitle') }} №{{ accountId }}</h2>

          <div class="device-list">
            <div v-for="device in devices" :key="device.id" class="device-card"
            @click="goToSensonrs(this.accountId, device.id)"
            @keydown.enter="goToSensonrs(this.accountId, device.id)"
            @keydown.space.prevent="goToSensonrs(this.accountId, device.id)"
            tabindex="0" role="button">
              <div class="device-name">{{ device.name }}</div>
              <div class="buttons">
                <button class="edit-button" @click.stop="goToEditDevice(this.accountId, device.id)">
                  {{ $t('devices.edit') }}</button>
              </div>
            </div>
          </div>

          <button class="add-button" @click="goToAddDevice(this.accountId)">
            ➕ {{ $t('devices.add') }}</button>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message"><p>{{ $t('devices.loading') }}</p></div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('devices.loginPrompt') }}</router-link>
      </div>
    </div>
  </main>
</template>

<script>
import SidebarComponent from '@/components/SidebarComponent.vue';
import { mapState, mapGetters, mapActions } from 'vuex';

export default {
  props: ['accountId'],
  components: {
    SidebarComponent,
  },
  data() {
    return {
      loading: false,
    };
  },
  computed: {
    ...mapState(['userInfo', 'devices']),
    ...mapGetters(['isAuthenticated']),
  },
  created() {
    this.init();
  },
  methods: {
    ...mapActions(['fetchUserInfo', 'getDevices']),
    async init() {
      this.loading = true;
      if (!this.userInfo) await this.fetchUserInfo();
      await this.getDevices(this.accountId);
      this.loading = false;
    },
    goToSensonrs(accountId, deviceId) {
      this.$router.push(`/accounts/${accountId}/devices/${deviceId}/sensors`);
    },
    goToAddDevice(accountId) {
      this.$router.push(`/accounts/${accountId}/devices/create`);
    },
    goToEditDevice(accountId, deviceId) {
      this.$router.push(`/accounts/${accountId}/devices/${deviceId}`);
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

.device-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-bottom: 30px;
  margin-top: 30px;
}

.device-card {
  all: unset;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background-color: #f0f4f8;
  border: 1px solid #ccc;
  cursor: pointer;
}

.device-card:hover {
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

.account-name, .add-button, .edit-button {
  font-size: 1.2rem;
}

.add-button:hover {
  background-color: #1f7855;
}
</style>
