<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ isEdit ? $t('device.editTitle') : $t('device.createTitle') }}</h2>

          <form @submit.prevent="saveDevice" class="edit-form">
            <label for="device-name">{{ $t('device.nameLabel') }}
            <input
              id="device-name"
              v-model="device.name"
              type="text"
              required
              :placeholder="$t('device.namePlaceholder')"
            /></label>

            <div class="buttons">
              <button type="submit" class="save-button">
                {{ isEdit ? $t('device.save') : $t('device.create') }}
              </button>

              <button
                v-if="isEdit"
                type="button"
                class="delete-button"
                @click="handleDeleteDevice">
              {{ $t('device.delete') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message">
        <p>{{ $t('device.loading') }}</p>
      </div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('device.loginPrompt') }}</router-link>
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
  props: ['accountId', 'deviceId'],
  data() {
    return {
      loading: false,
      device: {
        name: '',
      },
      isEdit: false,
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
    ...mapActions(['fetchUserInfo', 'getDeviceById', 'addDevice', 'updateDevice', 'deleteDevice']),
    async init() {
      this.loading = true;
      if (!this.userInfo) {
        await this.fetchUserInfo();
      }

      console.log(this.accountId);
      console.log(this.deviceId);

      if (this.deviceId) {
        this.isEdit = true;
        try {
          const device = await this.getDeviceById({
            accountId: this.accountId,
            deviceId: this.deviceId,
          });
          this.device = {
            id: device.id,
            name: device.name,
          };
        } catch (err) {
          alert('Error loading device');
          this.$router.push(`/accounts/${this.accountId}/devices`);
        }
      }

      this.loading = false;
    },

    async saveDevice() {
      try {
        if (this.isEdit) {
          await this.updateDevice({
            accountId: this.accountId,
            deviceId: this.device.id,
            updatedDevice: {
              id: this.device.id,
              name: this.device.name,
            },
          });
        } else {
          await this.addDevice({ accountId: this.accountId, device: this.device });
        }
        this.$router.push(`/accounts/${this.accountId}/devices`);
      } catch (err) {
        alert('Account save error');
      }
    },

    async handleDeleteDevice() {
      try {
        await this.deleteDevice({ accountId: this.accountId, deviceId: this.device.id });
        this.$router.push(`/accounts/${this.accountId}/devices`);
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
  max-width: 500px;
  padding-top:20px;
}

.edit-form input {
  padding: 10px;
  font-size: 1rem;
  border: 1px solid #ccc;
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
</style>
