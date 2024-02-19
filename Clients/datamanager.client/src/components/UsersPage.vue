<template>
    <v-container>
        <!-- Header with Title and Button -->
        <div class="header">
            <h1>Users</h1>
            <v-btn color="primary" @click="dialog = true">
                Create a New User
            </v-btn>
        </div>

        <!-- Create User Dialog -->
        <v-dialog v-model="dialog" persistent max-width="600px">
            <v-card>
                <v-card-title>
                    <span class="headline">Create User</span>
                </v-card-title>
                <v-card-text>
                    <v-container>
                        <v-row>
                            <v-col cols="12">
                                <v-text-field v-model="newUser.email" label="Email" type="email" required></v-text-field>
                            </v-col>
                            <v-col cols="12">
                                <v-text-field v-model="newUser.password" label="Password" type="password"
                                    required></v-text-field>
                            </v-col>
                            <v-col cols="12">
                                <v-select v-model="newUser.role" :items="roles" label="Role" required></v-select>
                            </v-col>
                        </v-row>
                    </v-container>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" text @click="dialog = false">Cancel</v-btn>
                    <v-btn color="blue darken-1" text @click="createUser">Create</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog v-model="deleteConfirmDialog" persistent max-width="300px">
            <v-card>
                <v-card-title class="text-h5">Confirm Deletion</v-card-title>
                <v-card-text>Are you sure you want to delete this user?</v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="green darken-1" text @click="deleteConfirmDialog = false">Cancel</v-btn>
                    <v-btn color="red darken-1" text @click="confirmDelete">Delete</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <!-- Users List -->
        <div class="users-list">
            <!-- Role Selector -->
            <v-select v-model="selectedRole" :items="roles" label="Select Role" @change="fetchUsers"></v-select>

            <v-text-field v-model="search" label="Search" prepend-inner-icon="mdi-magnify" single-line variant="outlined"
                hide-details></v-text-field>

            <v-data-table :headers="headers" :items="users" :search="search">
                <template v-slot:[`item.actions`]="{ item }">
                    <v-btn small color="red" @click="promptDeleteUser(item.userId)">Delete</v-btn>
                </template>
            </v-data-table>
        </div>
    </v-container>
</template>
  
<script>
import axios from 'axios';

export default {
    name: 'UsersPage',
    data() {
        return {
            dialog: false,
            newUser: {
                email: '',
                password: '',
                role: ''
            },
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
                { key: 'actions', title: 'Actions' }
            ],
            users: [],
            selectedRole: 'User',
            roles: ['User', 'Provider', 'Admin'],
            apiUrl: 'http://localhost:5009/api/users',
            deleteConfirmDialog: false,
            userToDelete: null,
        };
    },
    methods: {
        async createUser() {
            try {
                const response = await axios.post('http://localhost:5009/api/users', this.newUser);
                console.log('User created:', response.data);
                this.dialog = false;
                this.fetchUsers();
            } catch (error) {
                console.error('Error creating user:', error);
            }
        },
        async deleteUser(userId) {
            try {
                await axios.delete(`http://localhost:5009/api/users/${userId}`);
                console.log(`User with ID: ${userId} deleted`);
                this.fetchUsers();
            } catch (error) {
                console.error('Error deleting user:', error);
            }
        },
        async fetchUsers() {
            try {
                const response = await axios.get(`${this.apiUrl}?Role=${this.selectedRole}`);
                this.users = response.data;
            } catch (error) {
                console.error('Error fetching users:', error);
            }
        },
        // Method to open the confirmation dialog
        promptDeleteUser(userId) {
            this.userToDelete = userId;
            this.deleteConfirmDialog = true;
        },

        // Method to confirm deletion
        async confirmDelete() {
            if (this.userToDelete) {
                await this.deleteUser(this.userToDelete);
            }
            this.deleteConfirmDialog = false;
            this.userToDelete = null;
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




  