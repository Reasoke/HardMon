<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ isEdit ? $t('alertEdit.titleEdit') : $t('alertEdit.titleCreate') }}</h2>

          <form @submit.prevent="saveAlert" class="edit-form">
            <label for="alert-message">
              {{ $t('alertEdit.message') }}:
              <input
                id="alert-message"
                type="text"
                v-model="alert.alertMessage"
                readonly
              />
            </label>

            <label for="alert-checkbox">
              <input
                id="alert-checkbox"
                type="checkbox"
                v-model="alert.isChecked"/>
              {{ $t('alertEdit.checked') }}
            </label>

            <div class="buttons">
              <button type="submit" class="save-button">
                {{ isEdit ? $t('alertEdit.save') : $t('alertEdit.create')}}
              </button>
            </div>

          </form>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message">
        <p>{{ $t('alertEdit.loading') }}</p>
      </div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('alertEdit.authMessage') }}</router-link>
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
  props: ['accountId', 'deviceId', 'sensorId', 'alertId'],
  data() {
    return {
      loading: false,
      isEdit: false,
      alert: {
        alertMessage: '',
        isChecked: false,
      },
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
      'getAlertById',
      'updateAlert',
    ]),

    async init() {
      this.loading = true;
      if (!this.userInfo) {
        await this.fetchUserInfo();
      }

      if (this.alertId) {
        this.isEdit = true;
        try {
          const alert = await this.getAlertById({
            accountId: this.accountId,
            sensorId: this.sensorId,
            alertId: this.alertId,
          });
          this.alert = alert;
        } catch (err) {
          alert('Sensor download error');
          this.$router.push(`/accounts/${this.accountId}/devices/${this.deviceId}/sensors/${this.sensorId}/alerts`);
        }
      }

      this.loading = false;
    },

    async saveAlert() {
      try {
        if (this.isEdit) {
          await this.updateAlert({
            accountId: this.accountId,
            sensorId: this.sensorId,
            alertId: this.alert.id,
            updatedAlert: {
              isChecked: this.alert.isChecked,
            },
          });
        }
        this.$router.push(`/accounts/${this.accountId}/devices/${this.deviceId}/sensors/${this.sensorId}/alerts`);
      } catch (err) {
        alert(err.message);
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

#alert-message {
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
