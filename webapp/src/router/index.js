import {
  createRouter,
  createWebHistory,
} from 'vue-router';
import LoginPage from '@/pages/LoginPage.vue';
import RegisterPage from '@/pages/RegisterPage.vue';
import ProfilePage from '@/pages/ProfilePage.vue';
import AccountsPage from '@/pages/AccountsPage.vue';
import AccountEditPage from '@/pages/AccountEditPage.vue';
import DevicesPage from '@/pages/DevicesPage.vue';
import DeviceEditPage from '@/pages/DeviceEditPage.vue';
import SensorsPage from '@/pages/SensorsPage.vue';
import SensorEditPage from '@/pages/SensorEditPage.vue';
import AlertsPage from '@/pages/AlertsPage.vue';
import AlertEditPage from '@/pages/AlertEditPage.vue';
import AdminPage from '@/pages/AdminPage.vue';
import SensorTypeCreatePage from '@/pages/SensorTypeCreatePage.vue';
import UserTypeCreatePage from '@/pages/UserTypeCreatePage.vue';

export default createRouter({
  history: createWebHistory(),
  routes: [{
    path: '/',
    name: 'AccountsPage',
    components: {
      default: AccountsPage,
    },
  },
  {
    path: '/admin',
    props: true,
    component: AdminPage,
  },
  {
    path: '/admin/sensorTypes/create',
    props: true,
    component: SensorTypeCreatePage,
  },
  {
    path: '/admin/userTypes/create',
    props: true,
    component: UserTypeCreatePage,
  },
  {
    path: '/login',
    name: 'LoginPage',
    props: true,
    component: LoginPage,
  },
  {
    path: '/register',
    name: 'RegisterPage',
    props: true,
    component: RegisterPage,
  },
  {
    path: '/accounts/create',
    component: AccountEditPage,
    props: true,
  },
  {
    path: '/accounts/:id',
    component: AccountEditPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices',
    component: DevicesPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices/:deviceId',
    component: DeviceEditPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices/create',
    component: DeviceEditPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices/:deviceId/sensors',
    component: SensorsPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices/:deviceId/sensors/:sensorId',
    component: SensorEditPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices/:deviceId/sensors/create',
    component: SensorEditPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices/:deviceId/sensors/:sensorId/alerts',
    component: AlertsPage,
    props: true,
  },
  {
    path: '/accounts/:accountId/devices/:deviceId/sensors/:sensorId/alerts/:alertId',
    component: AlertEditPage,
    props: true,
  },
  {
    path: '/profile',
    name: 'ProfilePage',
    props: true,
    component: ProfilePage,
  }],
});
