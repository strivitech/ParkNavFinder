<template>
    <v-container>
        <l-map :zoom="zoom" :center="center" style="height: 85vh;">
            <l-tile-layer :url="url" />
            <l-marker v-for="user in validUsers" :key="user.id" :lat-lng="getLatLng(user)" :icon="carIcon">
            </l-marker>
            <l-marker v-for="parking in parkings" :key="parking.id"
                :lat-lng="[parking.latitude, parking.longitude]"></l-marker>
        </l-map>
    </v-container>
</template>
  
<script>
import 'leaflet/dist/leaflet.css';
import { LMap, LTileLayer, LMarker } from '@vue-leaflet/vue-leaflet';
import L from 'leaflet'; // Import core Leaflet
import { HubConnectionBuilder } from '@microsoft/signalr';
import TWEEN from '@tweenjs/tween.js';
import axios from 'axios';

export default {
    name: 'MapPage',
    components: {
        LMap,
        LTileLayer,
        LMarker
    },
    data() {
        return {
            center: [50.45466, 30.5238], // Example center coordinates
            zoom: 12,
            url: 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
            users: [], // Store user locations here
            parkings: [], // Store parking locations here
            connection: null,
            carIcon: new L.Icon({
                iconUrl: require('@/assets/car.png'), // Replace with the path to your car icon
                iconSize: [20, 20], // Adjust the size as needed
            }),
        };
    },
    computed: {
        validUsers() {
            return this.users.filter(user =>
                typeof user.latitude === 'number' && typeof user.longitude === 'number'
            );
        }
    },
    methods: {
        getLatLng(user) {
            return [user.latitude, user.longitude];
        },
        updateMarkerPosition(userId, newCoordinate) {
            const userIndex = this.users.findIndex(u => u.id === userId);
            if (userIndex !== -1) {
                const user = this.users[userIndex];
                const startPos = { latitude: user.latitude, longitude: user.longitude };
                const endPos = { latitude: newCoordinate.latitude, longitude: newCoordinate.longitude };

                new TWEEN.Tween(startPos)
                    .to(endPos, 3000) // Duration of 3000 ms (3 seconds)
                    .onUpdate(() => {
                        // Update user's coordinates
                        this.users[userIndex].latitude = startPos.latitude;
                        this.users[userIndex].longitude = startPos.longitude;
                    })
                    .start();
            } else {
                // Add a new user if not already in the array
                this.users.push({
                    id: userId,
                    latitude: newCoordinate.latitude,
                    longitude: newCoordinate.longitude
                });
            }
            // Trigger reactivity in Vue
            this.users = [...this.users];
        },
        async fetchParkingData() {
            try {
                const response = await axios.get('http://localhost:5009/api/parking/getAll');
                this.parkings = response.data;
            } catch (error) {
                console.error('Error fetching parking data:', error);
            }
        },
        initializeSignalR() {
            this.connection = new HubConnectionBuilder()
                .withUrl('http://localhost:5009/api/drivershub')
                .build();

            this.connection.on('ReceiveDriverLocation', (userId, coordinate) => {
                console.log('Received location update:', userId, coordinate);
                this.updateMarkerPosition(userId, coordinate);
            });

            this.connection.start().catch(err => console.error('SignalR Connection Error:', err));
        },
        animate(time) {
            requestAnimationFrame(this.animate);
            TWEEN.update(time);
        }
    },
    mounted() {
        this.initializeSignalR();
        this.animate();
        this.fetchParkingData();
    }
};
</script>
  
<style>
/* Add any specific styles here */
</style>
