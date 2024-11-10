<template>
    <v-container>
        <l-map :zoom="zoom" :center="center" style="height: 85vh;">
            <l-tile-layer :url="url" />
            <l-marker v-for="parking in parkings" :key="parking.id" :lat-lng="[parking.latitude, parking.longitude]"
                      @click="openParkingDialog(parking)">
            </l-marker>
            <l-marker :lat-lng="currentPosition" :icon="carIcon" />
        </l-map>

        <v-dialog v-model="dialogVisible" persistent max-width="30vw">
            <v-card>
                <v-card-title>Parking Details</v-card-title>
                <v-card-text>
                    <div>ID: {{ selectedParking.id }}</div>
                    <div>Name: {{ selectedParking.name }}</div>
                    <div>Latitude: {{ selectedParking.latitude }}</div>
                    <div>Longitude: {{ selectedParking.longitude }}</div>
                    <div v-if="selectedParking.state">
                        <div>Total Observers: {{ selectedParking.state.TotalObservers }}</div>
                        <div>Probability: {{ selectedParking.state.Probability }}</div>
                        <div>Last Calculated: {{ selectedParking.state.LastCalculatedUtc }}</div>
                    </div>
                    <div v-else>State: Not available yet</div>
                </v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary" text @click="dialogVisible = false">Close</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-container>
</template>

<script>
import 'leaflet/dist/leaflet.css';
import { LMap, LTileLayer, LMarker } from '@vue-leaflet/vue-leaflet';
import L from 'leaflet';
import { HubConnectionBuilder } from '@microsoft/signalr';
import TWEEN from '@tweenjs/tween.js';
import { useAuth0 } from '@auth0/auth0-vue';
import axios from 'axios';

export default {
    name: 'MapPage',
    components: { LMap, LTileLayer, LMarker },
    data() {
        return {
            center: [50.4321, 30.3887],
            zoom: 12,
            url: 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
            currentPosition: [50.42368, 30.381128],
            carIcon: new L.Icon({
                iconUrl: require('@/assets/car.png'),
                iconSize: [20, 20],
            }),
            connection: null,
            parkings: [],
            dialogVisible: false,
            selectedParking: {},
            routeCoordinates: [
                // Your list of coordinates here
                [
                        30.396747,
                        50.430504
                    ],
                    [
                        30.396938,
                        50.430841
                    ],
                    [
                        30.396848,
                        50.430958
                    ],
                    [
                        30.396936,
                        50.431114
                    ],
                    [
                        30.396677,
                        50.431184
                    ],
                    [
                        30.397017,
                        50.431796
                    ],
                    [
                        30.397014,
                        50.431917
                    ],
                    [
                        30.396991,
                        50.432066
                    ],
                    [
                        30.397005,
                        50.432151
                    ],
                    [
                        30.397063,
                        50.432239
                    ],
                    [
                        30.397198,
                        50.432331
                    ],
                    [
                        30.397832,
                        50.432476
                    ],
                    [
                        30.398742,
                        50.432693
                    ],
                    [
                        30.398944,
                        50.432746
                    ],
                    [
                        30.399906,
                        50.432998
                    ],
                    [
                        30.400332,
                        50.43311
                    ],
                    [
                        30.401117,
                        50.433363
                    ],
                    [
                        30.40349,
                        50.433984
                    ],
                    [
                        30.406304,
                        50.434695
                    ],
                    [
                        30.407112,
                        50.434897
                    ],
                    [
                        30.408349,
                        50.435207
                    ],
                    [
                        30.409188,
                        50.435413
                    ],
                    [
                        30.409891,
                        50.435573
                    ],
                    [
                        30.410079,
                        50.435614
                    ],
                    [
                        30.410478,
                        50.43562
                    ],
                    [
                        30.410625,
                        50.435603
                    ],
                    [
                        30.410832,
                        50.435541
                    ],
                    [
                        30.410916,
                        50.435492
                    ],
                    [
                        30.411064,
                        50.435366
                    ],
                    [
                        30.411251,
                        50.4351
                    ],
                    [
                        30.411367,
                        50.43492
                    ],
                    [
                        30.411554,
                        50.434631
                    ],
                    [
                        30.411628,
                        50.434509
                    ],
                    [
                        30.411914,
                        50.434201
                    ],
                    [
                        30.412185,
                        50.433873
                    ],
                    [
                        30.412315,
                        50.433854
                    ],
                    [
                        30.412419,
                        50.433875
                    ],
                    [
                        30.412479,
                        50.433925
                    ],
                    [
                        30.412503,
                        50.434008
                    ],
                    [
                        30.412269,
                        50.434406
                    ],
                    [
                        30.411948,
                        50.434789
                    ],
                    [
                        30.410544,
                        50.437042
                    ],
                    [
                        30.410212,
                        50.437672
                    ],
                    [
                        30.410059,
                        50.437906
                    ],
                    [
                        30.40994,
                        50.438041
                    ],
                    [
                        30.4099,
                        50.438084
                    ],
                    [
                        30.409552,
                        50.438553
                    ],
                    [
                        30.408637,
                        50.439988
                    ],
                    [
                        30.408461,
                        50.440252
                    ],
                    [
                        30.408122,
                        50.440898
                    ],
                    [
                        30.408037,
                        50.441213
                    ],
                    [
                        30.408022,
                        50.441449
                    ],
                    [
                        30.408051,
                        50.441766
                    ],
                    [
                        30.408098,
                        50.442063
                    ],
                    [
                        30.40817,
                        50.442259
                    ],
                    [
                        30.408382,
                        50.442655
                    ],
                    [
                        30.408515,
                        50.442833
                    ],
                    [
                        30.408642,
                        50.442979
                    ],
                    [
                        30.408936,
                        50.443236
                    ],
                    [
                        30.409161,
                        50.443418
                    ],
                    [
                        30.409505,
                        50.443659
                    ],
                    [
                        30.409903,
                        50.443873
                    ],
                    [
                        30.410387,
                        50.44408
                    ],
                    [
                        30.414404,
                        50.445122
                    ],
                    [
                        30.416652,
                        50.445718
                    ],
                    [
                        30.420467,
                        50.446694
                    ],
                    [
                        30.420413,
                        50.446858
                    ],
                    [
                        30.419946,
                        50.447662
                    ],
                    [
                        30.419716,
                        50.448045
                    ],
                    [
                        30.419502,
                        50.4484
                    ],
                    [
                        30.419376,
                        50.448609
                    ],
                    [
                        30.418434,
                        50.450175
                    ],
                    [
                        30.418091,
                        50.450726
                    ],
                    [
                        30.41767,
                        50.451419
                    ],
                    [
                        30.417638,
                        50.451496
                    ],
                    [
                        30.417703,
                        50.451636
                    ],
                    [
                        30.417766,
                        50.451692
                    ],
                    [
                        30.417984,
                        50.451793
                    ],
                    [
                        30.419022,
                        50.451789
                    ],
                    [
                        30.42025,
                        50.451819
                    ],
                    [
                        30.42049,
                        50.451818
                    ],
                    [
                        30.421045,
                        50.451819
                    ],
                    [
                        30.421205,
                        50.451818
                    ],
                    [
                        30.421393,
                        50.451816
                    ],
                    [
                        30.422123,
                        50.451823
                    ],
                    [
                        30.422276,
                        50.451813
                    ],
                    [
                        30.423842,
                        50.451549
                    ],
                    [
                        30.424254,
                        50.45147
                    ],
                    [
                        30.427537,
                        50.45097
                    ],
                    [
                        30.427239,
                        50.450226
                    ]
            ],
            interval: 5000, // 5 seconds
            movementQueue: []
        };
    },
    methods: {
        openParkingDialog(parking) {
            this.selectedParking = parking;
            this.dialogVisible = true;
        },
        async fetchParkingData() {
            try {
                const response = await axios.get('http://localhost:5007/api/Parking/all');
                this.parkings = response.data;
                console.log('Parking data:', this.parkings);
            } catch (error) {
                console.error('Error fetching parking data:', error);
            }
        },
        async sendLocation(position) {
            if (this.connection && this.connection.state === 'Connected') {
                await this.connection.invoke('SendLocation', { 
                    Longitude: position[0], 
                    Latitude: position[1] 
                });
            }
        },
        async initializeSignalR() {
            const { getAccessTokenSilently } = useAuth0();
            const token = await getAccessTokenSilently();
            this.connection = new HubConnectionBuilder()
                .withUrl('http://localhost:5002/api/usershub', {
                    accessTokenFactory: () => token,
                })
                .withAutomaticReconnect()
                .build();

            this.connection.on('ReceiveParkingState', (parkingStateInfos) => {
                console.log('Received parking states:', parkingStateInfos);
                parkingStateInfos.forEach(stateInfo => {
                    const parking = this.parkings.find(p => p.id === stateInfo.parkingId);
                    if (parking) {
                        parking.state = {
                            TotalObservers: stateInfo.totalObservers,
                            Probability: stateInfo.probability,
                            LastCalculatedUtc: stateInfo.lastCalculatedUtc
                        };
                        console.log('Updated parking with id', parking.id);
                    }
                });
            });

            await this.connection.start();
            this.prepareRouteQueue();
            this.processMovementQueue();
        },
        prepareRouteQueue() {
            for (let i = 0; i < this.routeCoordinates.length - 1; i++) {
                const start = this.routeCoordinates[i];
                const end = this.routeCoordinates[i + 1];
                const distance = this.calculateDistance(start, end);
                const steps = distance > 0.07 ? Math.ceil(distance / 0.07) : 1;

                for (let step = 1; step <= steps; step++) {
                    const interpolatedPoint = this.interpolatePoint(start, end, step / steps);
                    this.movementQueue.push(interpolatedPoint);
                }
            }
        },
        async processMovementQueue() {
            while (this.movementQueue.length > 0) {
                const nextPosition = this.movementQueue.shift();
                this.animatePositionTransition(this.currentPosition, nextPosition, 3000);
                await this.sendLocation(nextPosition);
                await new Promise(resolve => setTimeout(resolve, this.interval));
            }
        },
        animatePositionTransition(startPosition, endPosition, duration) {
            const start = { lat: startPosition[0], lng: startPosition[1] };
            const end = { lat: endPosition[1], lng: endPosition[0] };
            
            new TWEEN.Tween(start)
                .to(end, duration)
                .onUpdate(() => {
                    this.currentPosition = [start.lat, start.lng];
                })
                .start();
        },
        calculateDistance([lng1, lat1], [lng2, lat2]) {
            const R = 6371; // Radius of Earth in kilometers
            const dLat = (lat2 - lat1) * (Math.PI / 180);
            const dLng = (lng2 - lng1) * (Math.PI / 180);
            const a =
                Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(lat1 * (Math.PI / 180)) *
                Math.cos(lat2 * (Math.PI / 180)) *
                Math.sin(dLng / 2) * Math.sin(dLng / 2);
            return R * 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        },
        interpolatePoint([lng1, lat1], [lng2, lat2], t) {
            return [
                lng1 + (lng2 - lng1) * t,
                lat1 + (lat2 - lat1) * t
            ];
        },
        animate(time) {
            requestAnimationFrame(this.animate);
            TWEEN.update(time);
        }
    },
    mounted() {
        this.fetchParkingData();
        this.initializeSignalR();
        this.animate();
    }
};
</script>

<style>
/* Add any specific styles here */
</style>
