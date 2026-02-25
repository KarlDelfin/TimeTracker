<template>
  <el-container>
    <el-header
      v-if="
        this.$route.path != '/' && $route.path != '/register' && $route.path != '/forgot-password'
      "
      class="border"
    >
      <el-menu class="el-menu-demo d-flex justify-content-end" mode="horizontal">
        <el-sub-menu index="2">
          <template #title>Hi! {{ `${user.firstName} ${user.lastName}` }}</template>
          <el-menu-item class="text-danger" @click="logout"
            ><i class="bi bi-power me-2"></i> Logout</el-menu-item
          >
        </el-sub-menu>
      </el-menu></el-header
    >
    <el-container>
      <el-aside
        v-if="
          this.$route.path != '/' && $route.path != '/register' && $route.path != '/forgot-password'
        "
        class="border"
        width="200px"
      >
        <SideMenu />
      </el-aside>
      <!-- <el-main> -->
      <RouterView />
      <!-- </el-main> -->
    </el-container>
  </el-container>
</template>
<script>
import { ElMessage } from 'element-plus'

import SideMenu from './components/SideMenu.vue'
export default {
  components: { SideMenu },
  data() {
    return {
      user: {},
    }
  },
  methods: {
    logout() {
      localStorage.clear()
      this.$router.push('/')
      ElMessage.success('You have been logout')
    },
  },
  mounted() {
    setTimeout(() => {
      this.user = JSON.parse(localStorage.getItem('user'))
    }, 500)
  },
}
</script>

<style>
.main {
  min-height: 90vh;
}
</style>
