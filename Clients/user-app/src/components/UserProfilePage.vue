<template>
  <div v-if="isAuthenticated" class="user-profile">
    <div class="profile-header">
      <img :src="user.picture" alt="User's profile picture" class="profile-picture" />
    </div>

    <div class="profile-details">
      <h2>Profile Details</h2>
      <ul>
        <li><strong>Nickname:</strong> {{ user.nickname }}</li>
        <li><strong>Email:</strong> {{ user.email }}</li>
        <li><strong>Role:</strong> {{ user['http://parknavfinder.com/roles']
      ? user['http://parknavfinder.com/roles'][0]
      : 'No role assigned' }}</li>
        <li><strong>Last Updated:</strong> {{ new Date(user.updated_at).toLocaleDateString() }}</li>
      </ul>
    </div>
  </div>
</template>

<script>
import { useAuth0 } from '@auth0/auth0-vue';

export default {
  name: 'UserProfile',
  setup() {
    const { user, isAuthenticated } = useAuth0();

    return { user, isAuthenticated};
  },
};
</script>

<style scoped>
.user-profile {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  text-align: center;
  color: #333;
}

.profile-header {
  margin-bottom: 20px;
}

.profile-picture {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  margin-bottom: 10px;
}

.profile-details {
  background-color: #f7f7f7;
  padding: 15px;
  border-radius: 8px;
}

.profile-details h2 {
  margin-bottom: 10px;
}

.profile-details ul {
  list-style: none;
  padding: 0;
}

.profile-details li {
  margin: 8px 0;
  font-size: 16px;
}
</style>
