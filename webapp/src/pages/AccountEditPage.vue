<template>
  <main>
    <div v-if="isAuthenticated">
      <div class="container">
        <SidebarComponent />

        <div class="main">
          <h2>{{ isEdit ? $t('account.editTitle') : $t('account.addTitle') }}</h2>

          <form @submit.prevent="saveAccount" class="edit-form">
            <label for="account-name">{{ $t('account.nameLabel') }}
            <input
              id="account-name"
              v-model="account.name"
              type="text"
              required
              :placeholder="$t('account.namePlaceholder')"
            /></label>

            <div class="buttons">
              <button type="submit" class="save-button">
                {{ isEdit ? $t('account.save') : $t('account.create') }}
              </button>

              <button
                v-if="isEdit"
                type="button"
                class="delete-button"
                @click="handleDeleteAccount">
              {{ $t('account.delete') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div v-else-if="loading">
      <div class="loading_message">
        <p>{{ $t('account.loading') }}</p>
      </div>
    </div>

    <div v-else>
      <div class="not_authorised_message">
        <router-link to="/login">{{ $t('account.loginPrompt') }}</router-link>
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
  props: ['id'],
  data() {
    return {
      loading: false,
      account: {
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
    ...mapActions(['fetchUserInfo', 'getAccountById', 'addAccount', 'updateAccount', 'deleteAccount']),
    async init() {
      this.loading = true;
      if (!this.userInfo) {
        await this.fetchUserInfo();
      }

      if (this.id) {
        this.isEdit = true;
        try {
          const account = await this.getAccountById(this.id);
          this.account = {
            id: account.id,
            name: account.name,
            subscription: account.subscription,
          };
        } catch (err) {
          alert('Account loading error');
          this.$router.push('/');
        }
      }

      this.loading = false;
    },

    async saveAccount() {
      try {
        if (this.isEdit) {
          await this.updateAccount({
            id: this.account.id,
            updatedData: {
              id: this.account.id,
              name: this.account.name,
              subscription: this.account.subscription,
            },
          });
        } else {
          await this.addAccount(this.account);
        }
        this.$router.push('/');
      } catch (err) {
        alert('Account save error');
      }
    },

    async handleDeleteAccount() {
      try {
        await this.deleteAccount(this.account.id);
        this.$router.push('/');
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
