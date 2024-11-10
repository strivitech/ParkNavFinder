import { createRouter, createWebHistory } from 'vue-router';
import UserProfile from './components/UserProfilePage.vue';
import Map from './components/MapPage.vue';

const routes = [
    { path: '/', name: 'UserProfile', component: UserProfile },
    { path: '/map', name: 'Map', component: Map },
];

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes,
});

export default router;
