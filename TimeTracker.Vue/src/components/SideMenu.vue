<template>
  <el-menu class="sideMenu" :default-active="activeLink">
    <router-link to="/timetracker">
      <el-menu-item index="/timetracker"
        ><i class="bi bi-hourglass-split me-2"></i>
        <span>Time Tracker</span>
      </el-menu-item>
    </router-link>
    <router-link to="/user-task" v-if="user.roleLevel == 1">
      <el-menu-item index="/user-task"
        ><i class="bi bi-card-list me-2"></i>
        <span>User Task</span>
      </el-menu-item>
    </router-link>
    <router-link to="/activity" v-if="user.roleLevel == 1">
      <el-menu-item index="/activity"
        ><i class="bi bi-card-list me-2"></i>
        <span>Activity</span>
      </el-menu-item>
    </router-link>
    <router-link to="/user" v-if="user.roleLevel == 1">
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
      user: {},
      activeLink: '',
    }
  },
  watch: {
    $route(to) {
      this.activeLink = to.path
    },
  },
  mounted() {
    this.activeLink = this.$route.path
    this.user = JSON.parse(localStorage.getItem('user'))
    if (this.user == null) {
      this.$router.push('/')
    }
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
.el-menu-item {
  color: #6c6e72;
}
</style>
