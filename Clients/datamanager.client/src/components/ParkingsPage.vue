<template>
    <v-container>
        <!-- Parking List -->
        <div class="parkings-list">
            <v-data-table v-model:expanded="expanded" :headers="parkingHeaders" :search="search" :items="parkings"
                item-value="id" show-expand>
                <template v-slot:top>
                    <v-toolbar flat>
                        <v-text-field v-model="search" label="Search" prepend-inner-icon="mdi-magnify" single-line
                            variant="outlined" hide-details></v-text-field>
                    </v-toolbar>
                </template>
                <template v-slot:expanded-row="{ columns, item }">
                    <tr>
                        <td :colspan="columns.length">
                            <div><strong>Description:</strong> {{ item.description }}</div>
                            <div><strong>Address:</strong> {{ item.address.street }}, {{ item.address.streetNumber }}, {{
                                item.address.city }}, {{ item.address.country }}</div>
                            <div><strong>Coordinates:</strong> Latitude: {{ item.latitude }}, Longitude: {{ item.longitude
                            }}</div>
                            <div><strong>Total Spaces:</strong> {{ item.totalSpaces }}</div>
                        </td>
                    </tr>
                </template>
            </v-data-table>
        </div>
    </v-container>
</template>
  
  
<script>
import axios from 'axios';

export default {
    name: 'ParkingsPage',
    data() {
        return {
            search: '',
            expanded: [],
            parkingHeaders: [
                { title: 'ID', key: 'id' },
                { title: 'Provider ID', key: 'providerId' },
                { title: 'Name', key: 'name' },
                { title: 'Total Spaces', key: 'totalSpaces' },
                { title: '', key: 'data-table-expand' }
            ],
            parkings: []
        };
    },
    methods: {
        async fetchParkings() {
            try {
                const response = await axios.get('http://localhost:5009/api/parking/getall');
                this.parkings = response.data;
            } catch (error) {
                console.error('Error fetching parkings:', error);
            }
        }
    },
    mounted() {
        this.fetchParkings(); // Fetch parkings when the component is mounted
    }
};
</script>
  
  
<style></style>
  