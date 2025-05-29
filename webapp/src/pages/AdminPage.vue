<template>
  <main>
    <div v-if="isSysAdmin">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <div class="admin-panel">
            <h2>{{ $t('admin.sensorTypeTitle') }}</h2>
            <div class="sensortype-list">
              <div v-for="sensortype in sensorTypes" :key="sensortype.id" class="card">
                <div class="device-type">{{ sensortype.name }}</div>
                <div class="buttons">
                  <button class="delete-button" type="button"
                    @click="handleDeleteSensor(sensortype.id)">
                    {{ $t('admin.delete') }}
                  </button>
                </div>
              </div>
            </div>

            <button class="add-button" @click="goToAddSensor">
              ➕ {{ $t('admin.addSensorType') }}
            </button>
          </div>

          <div class="admin-panel">
            <h2>{{ $t('admin.userTypeTitle') }}</h2>
            <div class="usertype-list">
              <div v-for="userType in userTypes" :key="userType.id" class="card">
                <div class="usertype-name">{{ userType.name }}</div>
                <div class="buttons">
                  <button class="delete-button" type="button"
                    @click="handleDeleteUserType(userType.id)">
                    {{ $t('admin.delete') }}
                  </button>
                </div>
              </div>
            </div>

            <button class="add-button" @click="goToAddUserType">
              ➕ {{ $t('admin.addUserType') }}
            </button>
          </div>

          <div class="admin-panel">
            <h2>{{ $t('admin.subscriptionTitle') }}</h2>
            <div class="subscription-list">
              <div v-for="subscription in subscriptions" :key="subscription.id" class="card">
                <div class="subscription-name">{{ subscription.name }}</div>
                <div class="buttons">
                  <button class="delete-button" type="button"
                    @click="handleDeleteSubscription(subscription.id)">
                    {{ $t('admin.delete') }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="admin-panel">
            <h2>{{ $t('admin.accountTitle') }}</h2>
            <div class="account-list">
              <div v-for="account in accounts" :key="account.id" class="card">
                <div class="account-name"> {{ account.name }} </div>
                <div class="buttons">
                  <button class="delete-button" type="button"
                    @click="handleDeleteAccount(account.id)">
                    {{ $t('admin.delete') }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="admin-panel">
            <h2>{{ $t('admin.userTitle') }}</h2>
            <div class="user-list">
              <div v-for="user in users" :key="user.id" class="card">
                <div class="user-info">
                  <div><strong>{{ $t('admin.name') }}:</strong> {{ user.name }}</div>
                  <div><strong>{{ $t('admin.email') }}:</strong> {{ user.email }}</div>
                  <div><strong>{{ $t('admin.role') }}:</strong>
                    {{ user.isSysAdmin ? 'SysAdmin' : 'User' }}</div>
                </div>
                <div class="buttons">
                  <button class="delete-button" type="button"
                    @click="handleDeleteUser(user.id)">
                    {{ $t('admin.delete') }}
                  </button>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message"><p>{{ $t('admin.loading') }}</p></div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('admin.authMessage') }}</router-link>
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
      sensorTypes: [],
      subscriptions: [],
      userTypes: [],
      accounts: [],
      users: [],
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
      await this.getSensorTypes();
      await this.getSubscriptions();
      await this.getUserTypes();
      await this.getAccounts();
      await this.getUsers();
      this.loading = false;
    },

    async getSensorTypes() {
      try {
        const response = await axios.get('/api/sysAdmin/sensorType');
        this.sensorTypes = response.data;
      } catch (error) {
        console.error('Error while retrieving SensorTypes:', error);
      }
    },
    async handleDeleteSensor(id) {
      try {
        await axios.delete(`/api/sysAdmin/sensorType/${id}`);
        this.sensorTypes = this.sensorTypes.filter((s) => s.id !== id);
      } catch (error) {
        alert('Error while deleting');
        console.error(error);
      }
    },
    goToAddSensor() {
      this.$router.push('/admin/sensorTypes/create');
    },

    async getSubscriptions() {
      try {
        const response = await axios.get('/api/sysAdmin/subscription');
        this.subscriptions = response.data;
      } catch (error) {
        console.error('Error while retrieving Subscriptions:', error);
      }
    },
    async handleDeleteSubscription(id) {
      try {
        await axios.delete(`/api/sysAdmin/subscription/${id}`);
        this.subscriptions = this.subscriptions.filter((s) => s.id !== id);
      } catch (error) {
        alert('Error while deleting Subscription');
        console.error(error);
      }
    },

    async getUserTypes() {
      try {
        const response = await axios.get('/api/sysAdmin/userType');
        this.userTypes = response.data;
      } catch (error) {
        console.error('Error while retrieving UserTypes:', error);
      }
    },
    async handleDeleteUserType(id) {
      try {
        await axios.delete(`/api/sysAdmin/userType/${id}`);
        this.userTypes = this.userTypes.filter((t) => t.id !== id);
      } catch (error) {
        alert('Error while deleting UserType');
        console.error(error);
      }
    },
    goToAddUserType() {
      this.$router.push('/admin/userTypes/create');
    },

    async getAccounts() {
      try {
        const response = await axios.get('/api/sysAdmin/account');
        this.accounts = response.data;
      } catch (error) {
        console.error('Error while retrieving Accounts:', error);
      }
    },
    async handleDeleteAccount(id) {
      try {
        await axios.delete(`/api/sysAdmin/account/${id}`);
        this.accounts = this.accounts.filter((acc) => acc.id !== id);
      } catch (error) {
        alert('Error while deleting Account');
        console.error(error);
      }
    },

    async getUsers() {
      try {
        const response = await axios.get('/api/sysAdmin/user');
        this.users = response.data;
      } catch (error) {
        console.error('Error while retrieving Users:', error);
      }
    },
    async handleDeleteUser(id) {
      try {
        await axios.delete(`/api/sysAdmin/user/${id}`);
        this.users = this.users.filter((user) => user.id !== id);
      } catch (error) {
        alert('Error while deleting User');
        console.error(error);
      }
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
h2{
  margin-bottom:30px;
  margin-top:30px;
}

/* Main section */
.main {
  flex: 1;
  padding: 40px;
  font-size: 1.2rem;
}

.delete-button {
  padding: 5px 10px;
  background-color: #d13d3d;
  color: white;
  border: none;
  cursor: pointer;
  font-size: 0.9rem;
}

.delete-button:hover {
  background-color: #9c2727;
}

.sensortype-list {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}

.card {
  background-color: #f2f2f2;
  border: 1px solid #ccc;
  border-radius: 8px;
  padding: 15px 20px;
  width: 220px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.device-type {
  font-weight: bold;
  font-size: 1.1rem;
  margin-bottom: 10px;
}

.buttons {
  display: flex;
  justify-content: flex-end;
}

.add-button {
  margin-top: 30px;
  padding: 10px 15px;
  font-size: 1rem;
  background-color: #28a745;
  color: white;
  border: none;
  cursor: pointer;
}

.add-button:hover {
  background-color: #218838;
}

.subscription-list {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  margin-top: 30px;
}

.subscription-name {
  font-weight: bold;
  font-size: 1.1rem;
  margin-bottom: 10px;
}

.usertype-list {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  margin-top: 30px;
}

.usertype-name {
  font-weight: bold;
  font-size: 1.1rem;
  margin-bottom: 10px;
}

.account-list {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  margin-top: 30px;
}

.account-name {
  font-weight: bold;
  font-size: 1.1rem;
  margin-bottom: 10px;
}

.user-list {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  margin-top: 30px;
}

.user-info {
  margin-bottom: 10px;
  font-size: 1rem;
}

.user-info div {
  margin-bottom: 5px;
}
</style>
