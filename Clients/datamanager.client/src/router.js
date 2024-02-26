import { createRouter, createWebHistory } from 'vue-router';
import Home from './components/HomePage.vue';
import Users from './components/UsersPage.vue';
import Parkings from './components/ParkingsPage.vue';
import Map from './components/MapPage.vue';

const routes = [
    {
        path: '/',
        name: 'Home',
        component: Home
    },
    {
        path: '/users',
        name: 'Users',
        component: Users
    },
    {
        path: '/parkings',
        name: 'Parkings',
        component: Parkings
    },
    {
        path: '/map',
        name: 'Map',
        component: Map
    }
];

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
});

export default router;
