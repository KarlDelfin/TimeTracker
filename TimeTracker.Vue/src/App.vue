<template>
  <el-container>
    <el-header v-if="this.$route.path != '/'" class="border">
      <el-menu class="el-menu-demo d-flex justify-content-end" mode="horizontal">
        <el-sub-menu index="2">
          <template #title>Profile</template>
          <el-menu-item @click="logout"><i class="bi bi-power me-2"></i> Logout</el-menu-item>
        </el-sub-menu>
      </el-menu></el-header
    >
    <el-container>
      <el-aside v-if="this.$route.path != '/'" class="border" width="200px">
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
  methods: {
    logout() {
      localStorage.clear()
      this.$router.push('/')
      ElMessage.success('You have been logout')
    },
  },
}
</script>

<style>
.main {
  min-height: 90vh;
}
</style>
