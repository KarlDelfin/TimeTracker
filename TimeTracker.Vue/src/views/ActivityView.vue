<template>
  <el-main>
    <el-card class="menuCard">
      <div class="d-flex justify-content-between">
        <div><h4>Activity</h4></div>
        <div class="d-flex justify-content-end mb-2">
          <el-button type="primary" @click="openForm('Create Activity')">Create Activity</el-button>
        </div>
      </div>

      <el-scrollbar height="500px">
        <el-collapse accordion>
          <el-collapse-item title="Filter Results" name="1">
            <el-form @submit.prevent="getActivityByUserId">
              <el-input placeholder="Search Activity" v-model="search" />
              <div class="d-flex justify-content-end mt-2">
                <el-button @click="(clear(), getActivityByUserId())"> Reset </el-button>
                <el-button type="primary" @click="getActivityByUserId"> Apply </el-button>
              </div>
            </el-form>
          </el-collapse-item>
        </el-collapse>

        <el-table :data="activities" :row-class-name="tableRowClassName">
          <el-table-column label="Name" prop="activityName" />
          <el-table-column label="Description" prop="activityDescription" />
          <el-table-column label="Estimated Time (hour)" prop="activityEstimatedTime">
            <template #default="scope">{{
              scope.row.activityEstimatedTime > 1
                ? `${scope.row.activityEstimatedTime} hours`
                : `${scope.row.activityEstimatedTime} hour`
            }}</template>
          </el-table-column>
          <el-table-column label="Assigned to" prop="activityEstimatedTime">
            <template #default="scope">
              <el-tag
                v-if="scope.row.firstName == null || scope.row.lastName == null"
                type="warning"
                >Not yet assigned</el-tag
              >
              <el-tag v-else>{{ `${scope.row.firstName} ${scope.row.lastName}` }}</el-tag></template
            >
          </el-table-column>
          <el-table-column label="Operation" align="center">
            <template #default="scope">
              <el-button size="small" @click="openForm('Edit Activity', scope.row)">Edit</el-button>
              <el-button type="danger" size="small" @click="deleteActivity(scope.row.activityId)"
                >Delete</el-button
              >
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
          :total="activityPagination.totalElements"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="getActivityByUserId"
          @current-change="getActivityByUserId"
        />
      </div>
    </el-card>
  </el-main>

  <!-- START DIALOG -->
  <el-dialog :title="title" v-model="dialog.activityForm" center width="600" :before-close="clear">
    <el-form label-position="top" @submit.prevent="submitForm">
      <el-form-item label="Activity Name" :rules="[{ required: true }]">
        <el-input v-model="form.activityName" placeholder="Enter Activity Name" />
      </el-form-item>
      <el-form-item label="Activity Description" :rules="[{ required: true }]">
        <el-input v-model="form.activityDescription" placeholder="Enter Activity Description" />
      </el-form-item>
      <el-form-item label="Estimated Time (hour)" :rules="[{ required: true }]">
        <el-input-number :min="1" v-model="form.activityEstimatedTime" />
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="d-flex justify-content-end">
        <el-button type="primary" @click="submitForm">Confirm</el-button>
      </div>
    </template>
  </el-dialog>
  <!-- END DIALOG -->
</template>

<script>
import axios from 'axios'
import { ElMessage, ElMessageBox, ElLoading } from 'element-plus'
// const api = import.meta.env.VITE_APP_API_URL
export default {
  data() {
    return {
      search: '',
      activityId: '',
      title: '',
      user: {},
      dialog: {
        activityForm: false,
      },
      form: {
        activityName: '',
        activityDescription: '',
        activityEstimatedTime: 1,
      },
      activityPagination: {
        currentPage: 1,
        elementsPerPage: 10,
        totalElements: 0,
      },
      activities: [],
    }
  },
  methods: {
    tableRowClassName({ row }) {
      if (row.firstName == null || row.lastName == null) {
        return 'warning-row'
      }
      return ''
    },

    // GET ACTIVITY BY USER ID
    getActivityByUserId() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      axios
        .get(
          `${this.api}/Activity/User/${this.user.userId}?currentPage=${this.activityPagination.currentPage}&elementsPerPage=${this.activityPagination.elementsPerPage}&search=${this.search}`,
        )
        .then((response) => {
          this.activities = response.data.results
          this.activityPagination.totalElements = response.data.totalElements
          loading.close()
        })
    },

    // CLEAR
    clear() {
      this.dialog.activityForm = false
      this.form.activityName = ''
      this.form.activityDescription = ''
      this.form.activityEstimatedTime = 1
      this.search = ''
    },

    // OPEN FORM
    openForm(title, data) {
      this.dialog.activityForm = true
      this.title = title
      if (title == 'Edit Activity') {
        this.activityId = data.activityId
        this.form.activityName = data.activityName
        this.form.activityDescription = data.activityDescription
        this.form.activityEstimatedTime = data.activityEstimatedTime
      }
    },

    // SUBMIT FORM
    submitForm() {
      if (
        this.form.activityName == '' ||
        this.form.activityDescription == '' ||
        this.form.activityEstimatedTime == ''
      ) {
        ElMessage.warning('Please input required fields')
        return
      }
      if (this.title == 'Create Activity') {
        const payload = {
          userId: this.user.userId,
          activityName: this.form.activityName,
          activityDescription: this.form.activityDescription,
          activityEstimatedTime: this.form.activityEstimatedTime,
        }
        axios
          .post(`${this.api}/Activity`, payload)
          .then(() => {
            ElMessage.success('Activity added successfully')
            this.getActivityByUserId()
            this.clear()
          })
          .catch((error) => {
            ElMessage.error(error)
          })
      }

      if (this.title == 'Edit Activity') {
        const payload = {
          activityName: this.form.activityName,
          activityDescription: this.form.activityDescription,
          activityEstimatedTime: this.form.activityEstimatedTime,
        }
        axios
          .put(`${this.api}/Activity/${this.activityId}`, payload)
          .then(() => {
            ElMessage.success('Activity added successfully')
            this.getActivityByUserId()
            this.clear()
          })
          .catch((error) => {
            ElMessage.error(error)
          })
      }
    },

    // DELETE ACTIVITY
    deleteActivity(activityId) {
      ElMessageBox.confirm('Do you want to delete this Activity?', 'Warning', {
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel',
        type: 'warning',
      })
        .then(() => {
          axios
            .delete(`${this.api}/Activity/${activityId}`)
            .then(() => {
              ElMessage.success('Activity deleted successfully')
              this.getActivityByUserId()
            })
            .catch((error) => {
              ElMessage.error(error)
            })
        })
        .catch(() => {})
    },
  },
  mounted() {
    this.user = JSON.parse(localStorage.getItem('user'))
    if (this.user == null) {
      window.location.replace('/')
    }
    this.getActivityByUserId()
  },
}
</script>

<style>
.menuCard {
  min-height: 70vh;
  max-height: calc(92vh - 32px);
  overflow: hidden;
}
.el-table .primary-row {
  --el-table-tr-bg-color: var(--el-color-primary-light-9);
}
.el-table .success-row {
  --el-table-tr-bg-color: var(--el-color-success-light-9);
}
.el-table .warning-row {
  --el-table-tr-bg-color: var(--el-color-warning-light-9);
}
/* .el-table .danger-row {
  --el-table-tr-bg-color: var(--el-color-danger-light-9);
} */
</style>
