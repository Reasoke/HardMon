import { createApp } from 'vue';
import axios from 'axios';
import App from './App.vue';
import router from './router';
import store from './store';
import i18n from './i18n';

axios.defaults.withCredentials = true;
const app = createApp(App);

app.use(router);
app.use(store);
app.use(i18n);
store.dispatch('fetchUserInfo').finally(() => {
  app.mount('#app');
});
