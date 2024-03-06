<template>
    <v-container>
        <!-- Header with Title -->
        <div class="header">
            <h1>Users</h1>
        </div>

        <!-- Users List -->
        <div class="users-list">
            <!-- Role Selector -->
            <v-select v-model="selectedRole" :items="roles" label="Select Role" @change="fetchUsers"></v-select>

            <v-text-field v-model="search" label="Search" prepend-inner-icon="mdi-magnify" single-line variant="outlined"
                hide-details></v-text-field>

            <v-data-table :headers="headers" :items="users" :search="search"></v-data-table>
        </div>
    </v-container>
</template>
  
<script>
import axios from 'axios';

export default {
    name: 'UsersPage',
    data() {
        return {
            search: '',
            headers: [
                {
                    align: 'start',
                    key: 'userId',
                    sortable: false,
                    title: 'User Id',
                },
                { key: 'email', title: 'Email' },
                { key: 'role', title: 'Role' },
            ],
            users: [],
            selectedRole: 'User',
            roles: ['User', 'Provider', 'Admin'],
            apiUrl: 'http://localhost:5009/api/users',
        };
    },
    methods: {
        async fetchUsers() {
            try {
                const response = await axios.get(`${this.apiUrl}?Role=${this.selectedRole}`);
                this.users = response.data;
            } catch (error) {
                console.error('Error fetching users:', error);
            }
        },
    },
    mounted() {
        this.fetchUsers(); // Fetch users when the component is mounted
    },
    watch: {
        selectedRole() {
            this.fetchUsers();
        }
    },
};
</script>
  
<style>
.header {
    display: flex;
    align-items: center;
    justify-content: space-between;
}
</style>




  