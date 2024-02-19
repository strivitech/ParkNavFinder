<template>
  <v-app>
    <!-- Main Container -->
    <v-app-bar app color="primary" dark>
      <!-- Toggle Button for Sidebar -->
      <v-app-bar-nav-icon @click="toggleDrawer"></v-app-bar-nav-icon>
      <v-toolbar-title>Data Manager</v-toolbar-title>
    </v-app-bar>

    <!-- Sidebar with Adjusted Properties -->
    <v-navigation-drawer v-model="drawer" app class="sidebar">
      <v-list dense>
        <v-list-item-group v-model="activeItem" active-class="active-item">
          <v-list-item v-for="(item, index) in menuItems" :key="index" :to="item.path" link
            @click="setActiveItem(item.path)" :class="{ 'router-link-active': activeItem === item.path }">
            <v-list-item-content>
              <v-list-item-title class="text-center">
                {{ item.title }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list-item-group>
      </v-list>
    </v-navigation-drawer>

    <!-- Main Content -->
    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script>
import { ref, watch } from 'vue';
import { useRoute } from 'vue-router';

export default {
  name: 'App',
  setup() {
    const route = useRoute();
    const drawer = ref(true);
    const activeItem = ref(route.path);

    const toggleDrawer = () => {
      drawer.value = !drawer.value;
    };

    const setActiveItem = (path) => {
      activeItem.value = path;
    };

    watch(route, (newRoute) => {
      activeItem.value = newRoute.path;
    }, { immediate: true });

    const menuItems = [
      { title: 'Home', path: '/' },
      { title: 'Users', path: '/users' },
    ];

    return {
      drawer,
      activeItem,
      menuItems,
      toggleDrawer,
      setActiveItem
    };
  }
};
</script>

<style>
.sidebar {
  width: 150px;
  color: white;
  background-color: #1E1E1E;
}

.router-link-active {
  background-color: #424242;
  /* Active item background color */
}

/* Hover effect */
.v-list-item:hover {
  background-color: #333333;
}

.text-center {
  text-align: center;
}
</style>
