<template>
  <header>
    <nav>
      <router-link to="/" class="logo">
        <img class="logo_img" alt="logo" src="../assets/logo.svg" />
      </router-link>

      <div class="menu">
        <select v-model="$i18n.locale" class="lang-selector" aria-label="Language selector">
          <option value="uk">Українська</option>
          <option value="en">English</option>
        </select>

        <button class="profile_button" @click="goToProfile" aria-label="Profile">
          <img class="profile_img" alt="Profile" src="../assets/profile.svg">
        </button>
      </div>
    </nav>
  </header>
</template>

<script>
export default {
  methods: {
    goToProfile() {
      this.$router.push('/profile');
    },
  },
};
</script>

<style scoped>
nav {
  top: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0rem 2rem;
  background-color: #B8C7D4;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.logo_img {
  height: 6.5rem;
  font-size: 1.5rem;
  font-weight: bold;
}

.menu {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.menu a {
  text-decoration: none;
  color: #000;
  font-weight: 700;
  font-size: 1.47rem;
}

.profile_button {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 2rem;
}

.profile_img {
  height: 2.5rem;
  font-size: 1.5rem;
  font-weight: bold;
}

.lang-selector {
  font-size: 1rem;
  padding: 0.3rem 0.6rem;
  border-radius: 4px;
  border: 1px solid #999;
  background-color: #fff;
  cursor: pointer;
}
</style>
