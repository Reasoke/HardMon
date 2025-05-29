<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ $t('accounts.title') }}</h2>
          <div class="account-list">
            <div v-for="account in accounts" :key="account.id" class="account-card"
            @click="goToDevices(account.id)" @keydown.enter="goToDevices(account.id)"
            @keydown.space.prevent="goToDevices(account.id)" tabindex="0" role="button">
              <div class="account-name">{{ account.name }}</div>

              <button class="edit-button" @click.stop="goToEdit(account.id)">
                {{ $t('accounts.edit') }}
              </button>
            </div>
          </div>

          <button class="add-button" @click="goToAdd">
            ➕ {{ $t('accounts.add') }}
          </button>
        </div>

      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message">
        <p>{{ $t('accounts.loading') }}</p>
      </div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('accounts.loginPrompt') }}</router-link>
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
      loading: false,
    };
  },
  computed: {
    ...mapState(['userInfo', 'accounts']),
    ...mapGetters(['isAuthenticated']),
  },
  created() {
    this.init();
  },
  methods: {
    ...mapActions(['fetchUserInfo', 'getAccounts']),
    async init() {
      this.loading = true;
      if (!this.userInfo) {
        await this.fetchUserInfo();
      }
      await this.getAccounts();
      this.loading = false;
    },
    goToDevices(id) {
      this.$router.push(`/accounts/${id}/devices`);
    },

    goToEdit(id) {
      this.$router.push(`/accounts/${id}`);
    },

    goToAdd() {
      this.$router.push('/accounts/create');
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

.account-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-bottom: 30px;
  margin-top: 30px;
}

.account-card {
  all: unset;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background-color: #f0f4f8;
  border: 1px solid #ccc;
  cursor: pointer;
}

.account-card:hover {
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
