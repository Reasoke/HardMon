<template>
  <div class="sidebar">
    <ul>
      <li>
        <router-link to="/profile" exact-active-class="active">
          {{ $t('sidebar.profile') }}</router-link>
      </li>
      <li>
        <router-link to="/" exact-active-class="active">{{ $t('sidebar.accounts') }}</router-link>
      </li>
      <li v-if="isSysAdmin">
        <router-link to="/admin" exact-active-class="active">{{ $t('sidebar.admin') }}</router-link>
      </li>
    </ul>
  </div>
</template>

<script>
import { mapState } from 'vuex';

export default {
  computed: {
    ...mapState(['userInfo']),
    isSysAdmin() {
      return this.userInfo?.isSysAdmin === true;
    },
  },
};
</script>

<style scopeld>
.sidebar {
  width: 200px;
  background-color: #B8C7D4;
  padding: 20px;
}

.sidebar ul {
  list-style: none;
  padding: 0;
}

.sidebar li {
  margin-bottom: 20px;
}

.sidebar a {
  text-decoration: underline;
  color: black;
  font-weight: bold;
}

.sidebar a.active {
  color: darkblue;
}
</style>
