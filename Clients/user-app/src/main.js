import { createApp } from 'vue';
import App from './App.vue';
import vuetify from './plugins/vuetify';
import router from './router';
import { createAuth0 } from '@auth0/auth0-vue';

const app = createApp(App);

app.use(router);
app.use(vuetify);
app.use(
  createAuth0({
    domain: process.env.VUE_APP_AUTH0_DOMAIN,
    clientId: process.env.VUE_APP_AUTH0_CLIENT_ID,
    authorizationParams: {
      redirect_uri: process.env.VUE_APP_AUTH0_REDIRECT_URI,
      audience: process.env.VUE_APP_AUTH0_AUDIENCE
    },
    cacheLocation: 'localstorage',  // Options: 'memory', 'localstorage' for persistence
    useRefreshTokens: true,         // Enables refresh tokens for longer session durations
  })
);

app.mount('#app');
