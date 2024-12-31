<template>
  <el-menu class="sideMenu" :default-active="activeLink">
    <router-link to="/time-tracker">
      <el-menu-item index="/time-tracker"
        ><i class="bi bi-hourglass-split me-2"></i>
        <span>Time Tracker</span>
      </el-menu-item>
    </router-link>
    <router-link to="/activity">
      <el-menu-item index="/activity"
        ><i class="bi bi-card-list me-2"></i>
        <span>Activity</span>
      </el-menu-item>
    </router-link>
    <router-link to="/user">
      <el-menu-item index="/user"
        ><i class="bi bi-people-fill me-2"></i>
        <span>User</span>
      </el-menu-item>
    </router-link></el-menu
  >
</template>
<script>
export default {
  data() {
    return {
      activeLink: '',
    }
  },
  mounted() {
    this.activeLink = this.$route.path
  },
}
</script>

<style>
.sideMenu {
  min-height: 90vh;
}
a {
  color: inherit;
  text-decoration: none !important;
}
</style>
