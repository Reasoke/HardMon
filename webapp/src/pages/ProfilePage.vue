<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
         <SidebarComponent/>

        <div class="main">
          <img src="https://cdn-icons-png.flaticon.com/512/847/847969.png" alt="Avatar" class="avatar" />

          <label for="name" class="visually-hidden">{{ $t('profile.name') }}
            <input type="text" id="name" v-model="editedProfile.name" class="input" /></label>

          <label for="email" class="visually-hidden">{{ $t('profile.email') }}
            <input type="email" id="email" v-model="editedProfile.email" class="input" /></label>

          <button class="save-button" @click="saveProfile">{{ $t('profile.save') }}</button>
          <button class="logout-button" @click="logoutProfile">{{ $t('profile.logout') }}</button>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message">
        <p>{{ $t('profile.loading') }}</p>
      </div>
    </div>

      <div v-else>
        <div class="not_authorised_message">
          <router-link to="/login">{{ $t('profile.loginPrompt') }}</router-link>
        </div>
      </div>
  </main>
</template>

<script>
import { mapState, mapGetters, mapActions } from 'vuex';
import SidebarComponent from '@/components/SidebarComponent.vue';

export default {
  components: {
    SidebarComponent,
  },
  data() {
    return {
      editedProfile: {
        name: '',
        email: '',
      },
      loading: false,
    };
  },
  computed: {
    ...mapState(['userInfo']),
    ...mapGetters(['isAuthenticated']),
  },
  watch: {
    userInfo: {
      immediate: true,
      handler(newVal) {
        if (newVal) {
          this.editedProfile = { ...newVal };
        }
      },
    },
  },
  created() {
    if (!this.userInfo) {
      this.fetchUserInfo();
    }
  },
  methods: {
    ...mapActions(['fetchUserInfo', 'updateProfile', 'logout']),
    async saveProfile() {
      this.loading = true;
      const success = await this.updateProfile(this.editedProfile);
      await this.fetchUserInfo();
      this.loading = false;
      if (!success) {
        alert('Failed to save profile');
      }
    },
    async logoutProfile() {
      await this.logout();
    },
  },
};
</script>

<style scoped>
.not_authorised_message{
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 75vh;
}

.not_authorised_message p {
  font-size: 1.5rem;
  font-weight: bold;
}

.loading_message{
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 75vh;
}

.loading_message p {
  font-size: 1.5rem;
  font-weight: bold;
}

.container {
  display: flex;
}

/* Main section */
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 40px;
  font-size: 1.2rem;
}

.avatar {
  width: 80px;
  height: 80px;
  margin-bottom: 30px;
}

.input {
  display: block;
  margin: 1rem 0;
  padding: 0.5rem;
  width: 100%;
}

.save-button,
.logout-button {
  margin-right: 1rem;
  padding: 0.5rem 1rem;
}

.not_authorised_message,
.loading_message {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 75vh;
  font-size: 1.3rem;
  font-weight: bold;
}

.input {
  width: 300px;
  padding: 10px;
  margin-bottom: 20px;
  text-align: center;
  border: 1px solid #000000;
  background-color: #B8C7D4;
  font-family: monospace;
}

.save-button {
  padding: 8px 24px;
  background-color: #28a776;
  border: 1px solid #aaa;
  cursor: pointer;
  color: #ffffff;
}

.save-button:hover {
  background-color: #1f7855;
}

.logout-button {
  margin-top: 20px;
  padding: 8px 24px;
  background-color: #2859a7;
  border: 1px solid #aaa;
  cursor: pointer;
  color: #ffffff;
}

.logout-button:hover {
  background-color: #163972;
}
</style>
