<template>
  <el-main>
    <el-card class="menuCard">
      <h4>User</h4>

      <div class="d-flex justify-content-end">
        <el-button type="primary">Assign User</el-button>
      </div>

      <el-scrollbar height="400px">
        <el-table :data="activities">
          <el-table-column label="First Name" prop="firstName" />
          <el-table-column label="Last Name" prop="lastName" />
          <el-table-column label="Email" prop="email" />
          <el-table-column label="Operation">
            <template #default="scope">
              <el-button size="small">Edit</el-button>
              <el-button type="danger" size="small">Delete</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-scrollbar>
    </el-card>
    <el-card class="mt-2">
      <div id="paginationCard">
        <el-pagination
          v-model:current-page="activityPagination.currentPage"
          v-model:page-size="activityPagination.elementsPerPage"
          :page-sizes="[5, 10, 25, 50]"
          :total="activityPagination.totalPages"
          :disabled="tableLoading"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handlePageSizeAssignedUsers"
          @current-change="handlePageChangeAssignedUsers"
        />
      </div>
    </el-card>
  </el-main>
</template>

<script>
import axios from 'axios'
import { ElMessage } from 'element-plus'
const api = import.meta.env.VITE_APP_API_URL
export default {
  data() {
    return {
      activityPagination: {
        currentPage: 1,
        elementsPerPage: 10,
        totalPages: 0,
      },
      activities: [{ firstName: 'Karl', lastName: 'Delfin', email: 'dkhrl2000@gmail.com' }],
    }
  },
  methods: {},
  mounted() {
    this.user = JSON.parse(localStorage.getItem('user'))
    if (this.user == null) {
      window.location.replace('/')
    }
  },
}
</script>

<style>
.menuCard {
  min-height: 70vh;
  max-height: calc(92vh - 32px);
  overflow: hidden;
}
</style>
