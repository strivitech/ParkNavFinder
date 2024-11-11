<template>
    <div class="home-content">
        <h1>Welcome to User App</h1>
        <p>Please login to use the service</p>
        <!-- You can add more content here as needed -->
    </div>
</template>
  
<script>
export default {
    name: 'HomePage',
    // Script for the Home component
};
</script>
  
<style>
.home-content {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    min-height: calc(100vh - 64px);
    /* Adjusted for the header height */
    text-align: center;
    padding: 20px;
    box-sizing: border-box;
}

h1 {
    font-size: 2.5em;
    margin: 0;
    padding-bottom: 0.5em;
}

p {
    font-size: 1.2em;
    margin: 0;
}
</style>
  