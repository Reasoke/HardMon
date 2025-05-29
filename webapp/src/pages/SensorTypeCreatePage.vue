<template>
  <main>
    <div v-if="isSysAdmin">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ $t('adminSensorType.addSensorTypeTitle') }}</h2>

          <form @submit.prevent="saveSensorType" class="edit-form">
            <label for="name">
              {{ $t('adminSensorType.sensorTypeName') }}:
              <input
                id="name"
                v-model="sensorType.name"
                type="text"
                required
                :placeholder="$t('adminSensorType.sensorTypePlaceholder')"
              />
            </label>

            <div class="buttons">
              <button type="submit" class="save-button">{{ $t('adminSensorType.save') }}</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message"><p>{{ $t('adminSensorType.loading') }}</p></div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('adminSensorType.authMessage') }}</router-link>
      </div>
    </div>
  </main>
</template>

<script>
import SidebarComponent from '@/components/SidebarComponent.vue';
import { mapState, mapActions } from 'vuex';
import axios from 'axios';

export default {
  components: {
    SidebarComponent,
  },
  data() {
    return {
      loading: false,
      sensorType: {
        name: '',
      },
    };
  },
  computed: {
    ...mapState(['userInfo']),
    isSysAdmin() {
      return this.userInfo?.isSysAdmin === true;
    },
  },
  created() {
    this.init();
  },
  methods: {
    ...mapActions(['fetchUserInfo']),
    async init() {
      this.loading = true;
      if (!this.userInfo) await this.fetchUserInfo();
      this.loading = false;
    },

    async saveSensorType() {
      try {
        await axios.post('/api/sysAdmin/sensorType', this.sensorType);
        this.$router.push('/admin');
      } catch (error) {
        alert('Error creating SensorType');
        console.error(error);
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
}

.edit-form input {
  padding: 10px;
  font-size: 1rem;
  border: 1px solid #ccc;
}

.save-button {
  background-color: #28a745;
  color: white;
  border: none;
  padding: 10px 15px;
  cursor: pointer;
  font-size: 1rem;
}

.save-button:hover {
  background-color: #218838;
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
