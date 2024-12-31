<template>
  <el-main>
    <el-card class="menuCard">
      <h4>Time Tracker</h4>

      <div class="d-flex justify-content-end mb-2">
        <el-button type="primary" @click="openForm('Add Record')">Add Record</el-button>
      </div>
      <el-collapse accordion>
        <el-collapse-item title="Filter Results" name="1">
          <el-form>
            <el-input placeholder="Search Activity" />
            <div class="d-flex justify-content-end mt-2">
              <el-button> Reset </el-button>
              <el-button type="primary"> Apply </el-button>
            </div>
          </el-form>
        </el-collapse-item>
      </el-collapse>
      <el-scrollbar height="500px">
        <el-table :data="records">
          <el-table-column label="Elapsed Time" prop="currentRunningTime">
            <template #default="scope">
              <span class="border p-2 rounded">
                {{ formatElapsedTime(scope.row.currentRunningTime, scope.$index) }}
              </span>
            </template>
          </el-table-column>
          <el-table-column label="Activity Name" prop="activityName" />
          <el-table-column label="Details" align="center">
            <template #default="scope">
              <el-button size="small" @click="openForm('Record Details', scope.row)"
                >View</el-button
              >
            </template>
          </el-table-column>
          <el-table-column label="Status" prop="status">
            <template #default="scope">
              <div v-if="scope.row.recordStatusName == 'Start'">
                <el-tag type="primary">Started</el-tag>
              </div>
              <div v-else-if="scope.row.recordStatusName == 'Resume'">
                <el-tag type="primary">Resumed</el-tag>
              </div>
              <div v-else-if="scope.row.recordStatusName == 'Pause'">
                <el-tag type="warning">Paused</el-tag>
              </div>
              <div v-else-if="scope.row.recordStatusName == 'Complete'">
                <el-tag type="success">Completed</el-tag>
              </div>
              <div v-else-if="scope.row.recordStatusName == 'Reassign'">
                <el-tag type="info">Reassigned</el-tag>
              </div>
              <div v-else>
                <el-tag type="info">Not yet started</el-tag>
              </div>
            </template>
          </el-table-column>
          <el-table-column label="Operation" align="center">
            <template #default="scope">
              <!-- START [START] -->
              <el-popconfirm
                hide-icon
                width="250px"
                title="Do you want to start this Activity?"
                confirmButtonText="Yes"
                cancelButtonText="No"
                @confirm="updateStatus(scope.row.recordId, 'Start', scope.row.currentRunningTime)"
                @cancel="getRecordByUserId()"
              >
                <template #reference>
                  <el-button
                    :style="
                      scope.row.recordStatusName == 'Start' ? 'border: 1px solid #409EFF' : ''
                    "
                    :disabled="
                      scope.row.recordStatusName == 'Start' ||
                      scope.row.recordStatusName == 'Complete' ||
                      activeRowId != null
                    "
                    size="small"
                    ><i class="bi bi-play-fill"></i></el-button
                ></template>
              </el-popconfirm>
              <!-- END [START] -->

              <!-- START [PAUSE] -->
              <el-button
                :style="
                  scope.row.recordStatusName == 'Pause' || scope.row.recordStatusName == null
                    ? 'border: 1px solid #E6A23C'
                    : ''
                "
                :disabled="
                  scope.row.recordStatusName == 'Pause' ||
                  scope.row.recordStatusName == null ||
                  scope.row.recordStatusName == 'Complete'
                "
                size="small"
                @click="updateStatus(scope.row.recordId, 'Pause', scope.row.currentRunningTime)"
                ><i class="bi bi-pause-fill"></i
              ></el-button>
              <!-- END [PAUSE] -->

              <!-- START [COMPLETE] -->
              <el-popconfirm
                hide-icon
                width="250px"
                title="Do you want to complete this Activity?"
                confirmButtonText="Yes"
                cancelButtonText="No"
                @confirm="
                  updateStatus(scope.row.recordId, 'Complete', scope.row.currentRunningTime)
                "
                @cancel="getRecordByUserId()"
              >
                <template #reference>
                  <el-button
                    :style="
                      scope.row.recordStatusName == 'Complete' ? 'border: 1px solid #67C23A' : ''
                    "
                    :disabled="
                      scope.row.recordStatusName == 'Complete' ||
                      scope.row.recordStatusName == null ||
                      scope.row.recordStatusName == 'Reassign'
                    "
                    size="small"
                    ><span>Complete</span></el-button
                  >
                </template>
              </el-popconfirm>
              <!-- END [COMPLETE] -->

              <!-- START [REMOVE] -->
              <el-button
                v-if="scope.row.recordStatusName == null"
                type="danger"
                size="small"
                @click="deleteRecord(scope.row.recordId)"
                >Remove</el-button
              >
              <!-- END [REMOVE] -->
            </template>
          </el-table-column>
        </el-table>
      </el-scrollbar>
    </el-card>
    <!-- START RECORD PAGINATION -->
    <el-card class="mt-2">
      <div id="paginationCard">
        <el-pagination
          v-model:current-page="recordPagination.currentPage"
          v-model:page-size="recordPagination.elementsPerPage"
          :page-sizes="[5, 10, 25, 50]"
          :total="recordPagination.totalElements"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="getRecordByUserId"
          @current-change="getRecordByUserId"
        />
      </div>
    </el-card>
    <!-- END RECORD PAGINATION -->
  </el-main>

  <!-- START RECORD DIALOG -->
  <el-dialog :title="title" v-model="dialog.recordForm" center width="500" :before-close="clear">
    <el-form label-position="top" @submit.prevent="submitForm">
      <el-form-item
        label="Activity Name"
        v-for="(activity, index) in recordForm"
        :key="index"
        :rules="[{ required: true }]"
      >
        <div class="row w-100">
          <div class="col-11">
            <el-select
              v-model="activity.activityId"
              placeholder="Enter Activity Name"
              :filter-method="getActivityByUserId"
              filterable
            >
              <el-option
                v-for="activity in activities"
                :key="activity.activityId"
                :label="`${activity.activityName} - ${
                  activity.activityEstimatedTime > 1
                    ? `${activity.activityEstimatedTime} hours`
                    : `${activity.activityEstimatedTime} hour`
                }
                `"
                :value="activity.activityId"
              />
            </el-select>
          </div>
          <div v-if="recordForm.length > 1" class="col-1">
            <el-button type="danger" @click="removePushedRecord(index)"
              ><i class="bi bi-x"></i
            ></el-button>
          </div>
        </div>
      </el-form-item>
      <el-button size="small" @click="addMore">Add More</el-button>
    </el-form>
    <template #footer>
      <div class="d-flex justify-content-end">
        <el-button type="primary" @click="submitForm">Confirm</el-button>
      </div>
    </template>
  </el-dialog>
  <!-- END RECORD DIALOG -->

  <!-- START VIEW RECORD -->
  <el-dialog :title="title" v-model="dialog.viewRecord" center width="600" :before-close="clear">
    <div class="pt-2">
      <span class="fw-bold">Activity Name: </span>
      <span class="pt-2">{{ form.activityName }}</span>
    </div>
    <div class="pt-2">
      <span class="fw-bold">Activity Description: </span>
      <span class="pt-2">{{ form.activityDescription }}</span>
    </div>
    <div class="pt-2">
      <span class="fw-bold">Estimated Time: </span>
      <span class="pt-2">{{
        form.activityEstimatedTime > 1
          ? `${form.activityEstimatedTime} hours`
          : `${form.activityEstimatedTime} hour`
      }}</span>
    </div>
    <el-divider />
    <!-- START STATUS LOG TABLE -->
    <el-table :data="statusLogs">
      <el-table-column label="Status Name" prop="statusLogName">
        <template #default="scope">
          <div v-if="scope.row.statusLogName == 'Start'">
            <el-tag type="primary">Started</el-tag>
          </div>
          <div v-else-if="scope.row.statusLogName == 'Resume'">
            <el-tag type="primary">Resumed</el-tag>
          </div>
          <div v-else-if="scope.row.statusLogName == 'Pause'">
            <el-tag type="warning">Paused</el-tag>
          </div>
          <div v-else-if="scope.row.statusLogName == 'Complete'">
            <el-tag type="success">Completed</el-tag>
          </div>
          <div v-else-if="scope.row.statusLogName == 'Reassign'">
            <el-tag type="info">Reassigned</el-tag>
          </div>
          <div v-else>
            <el-tag type="info">Not yet started</el-tag>
          </div>
        </template>
      </el-table-column>
      <el-table-column label="Date / Time Log" prop="dateTimeCreated">
        <template #default="scope"> {{ formatDateTime(scope.row.dateTimeCreated) }}</template>
      </el-table-column>
    </el-table>
    <!-- END STATUS LOG TABLE -->

    <!-- START STATUS LOG PAGINATION -->
    <el-card class="mt-2">
      <div id="paginationCard">
        <el-pagination
          v-model:current-page="statusLogPagination.currentPage"
          v-model:page-size="statusLogPagination.elementsPerPage"
          :page-sizes="[5, 10, 25, 50]"
          :total="statusLogPagination.totalElements"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="getStatusLogByRecordId(recordId)"
          @current-change="getStatusLogByRecordId(recordId)"
        />
      </div>
    </el-card>
    <!-- END STATUS LOG PAGINATION -->
  </el-dialog>
  <!-- END VIEW RECORD -->
</template>

<script>
import axios from 'axios'
import { ElMessage, ElMessageBox, ElLoading } from 'element-plus'
const api = import.meta.env.VITE_APP_API_URL

export default {
  data() {
    return {
      activeRowId: null,
      recordId: '',
      form: {
        activityName: '',
        activityDescription: '',
        activityEstimatedTime: '',
      },
      title: '',
      recordForm: [{ activityId: '', userId: '' }],
      dialog: {
        recordForm: false,
        viewRecord: false,
      },
      user: {},
      statusLogPagination: {
        currentPage: 1,
        elementsPerPage: 5,
        totalElements: 0,
      },
      recordPagination: {
        currentPage: 1,
        elementsPerPage: 10,
        totalElements: 0,
      },
      activityPagination: {
        currentPage: 1,
        elementsPerPage: 10,
        totalElements: 0,
      },
      records: [],
      activities: [],
      statusLogs: [],
    }
  },
  computed: {
    // filterActivity() {
    //   return this.activities.filter(
    //     (a) => !this.recordForm.some((r) => r.activityId === a.activityId),
    //   )
    // },
  },

  methods: {
    addMore() {
      this.recordForm.push({
        activityId: '',
        userId: '',
      })
    },
    removePushedRecord(index) {
      this.recordForm.splice(index, 1)
    },

    // CLEAR
    clear() {
      this.form.activityName = ''
      this.form.activityDescription = ''
      this.form.activityEstimatedTime = ''
      this.dialog.recordForm = false
      this.dialog.viewRecord = false
      this.recordForm = []
      this.recordForm.push({
        activityId: '',
        userId: '',
      })
      this.title = ''
      this.recordId = ''
    },

    // OPEN FORM
    openForm(title, data) {
      this.title = title

      if (title == 'Add Record') {
        this.dialog.recordForm = true
        this.getActivityByUserId(null)
      }
      if (title == 'Record Details') {
        this.recordId = data.recordId
        this.dialog.viewRecord = true
        this.form.activityName = data.activityName
        this.form.activityDescription = data.activityDescription
        this.form.activityEstimatedTime = data.activityEstimatedTime
        this.getStatusLogByRecordId(data.recordId)
      }
    },

    // DELETE RECORD
    deleteRecord(recordId) {
      ElMessageBox.confirm('Do you want to delete this Activity?', 'Warning', {
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel',
        type: 'warning',
      })
        .then(() => {
          axios
            .delete(`${api}/Record/${recordId}`)
            .then(() => {
              this.getRecordByUserId()
              ElMessage.success('Record deleted successfully')
            })
            .catch((error) => {
              ElMessage.error(error)
            })
        })
        .catch(() => {})
    },

    // GET STATUS BY RECORD ID
    getStatusLogByRecordId(recordId) {
      axios
        .get(
          `${api}/StatusLog/Record/${recordId}?currentPage=${this.statusLogPagination.currentPage}&elementsPerPage=${this.statusLogPagination.elementsPerPage}`,
        )
        .then((response) => {
          this.statusLogs = response.data.results
          this.statusLogPagination.totalElements = response.data.totalElements
        })
    },
    // START, PAUSE, COMPLETE
    updateStatus(recordId, statusName, currentRunningTime) {
      var payload = {
        recordId: recordId,
        statusName: statusName,
        currentRunningTime: currentRunningTime,
      }
      if (statusName == 'Start') {
        this.activeRowId = recordId
        localStorage.setItem('elapsedTime', currentRunningTime)
      } else {
        this.activeRowId = null
        localStorage.setItem('elapsedTime', currentRunningTime)
      }
      axios
        .post(`${api}/StatusLog`, payload)
        .then(() => {
          if (statusName == 'Start') {
            ElMessage.success(`Activity has been Started`)
          } else if (statusName == 'Pause') {
            ElMessage.success(`Activity has been Paused`)
          }

          this.getRecordByUserId()
        })
        .catch((error) => {
          ElMessage.error(error)
        })
    },

    // SUBMIT FORM
    submitForm() {
      if (this.title == 'Add Record') {
        const payload = this.recordForm.map((data) => ({
          activityId: data.activityId,
          userId: this.user.userId,
        }))
        axios
          .post(`${api}/Record`, payload)
          .then(() => {
            ElMessage.success('Records added succesfully')
            this.getRecordByUserId()
            this.clear()
          })
          .catch((error) => {
            if (error.status) {
              ElMessage.info('Please select activity')
            } else {
              ElMessage.error(error)
            }
          })
      }
    },

    // GET RECORD BY USER ID
    getRecordByUserId(data) {
      if (typeof data == 'number') {
        this.records.forEach((record) => {
          if (record.recordStatusName === 'Start') {
            this.updateStatus(record.recordId, 'Pause', record.currentRunningTime)
          }
        })
      }
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      axios
        .get(
          `${api}/Record/User/${this.user.userId}?currentPage=${this.recordPagination.currentPage}&elementsPerPage=${this.recordPagination.elementsPerPage}`,
        )
        .then((response) => {
          this.records = response.data.results
          this.recordPagination.totalElements = response.data.totalElements
          loading.close()
        })
    },

    // GET ACTIVITY BY USER ID
    getActivityByUserId(data) {
      axios
        .get(
          `${api}/Activity/User/${this.user.userId}?currentPage=${this.activityPagination.currentPage}&elementsPerPage=${this.activityPagination.elementsPerPage}&search=${data}`,
        )
        .then((response) => {
          this.activities = response.data.results
          this.activityPagination.totalElements = response.data.totalElements
        })
    },

    // REACTIVE ELAPSE TIME
    updateElapsedTime() {
      this.records.forEach((record) => {
        if (record.recordStatusName === 'Start') {
          record.currentRunningTime += 1000
          this.activeRowId = record.recordId
        }
      })
    },

    formatElapsedTime(elapsedTime) {
      let hours = Math.floor(elapsedTime / (1000 * 60 * 60))
      let minutes = Math.floor((elapsedTime / (1000 * 60)) % 60)
      let seconds = Math.floor((elapsedTime / 1000) % 60)

      hours = hours.toString().padStart(2, '0')
      minutes = minutes.toString().padStart(2, '0')
      seconds = seconds.toString().padStart(2, '0')

      return `${hours}:${minutes}:${seconds}`
    },

    formatDateTime(dateTime) {
      if (!dateTime) {
        return ''
      }

      const months = [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July',
        'August',
        'September',
        'October',
        'November',
        'December',
      ]

      const date = new Date(dateTime)
      const year = date.getFullYear()
      const monthIndex = date.getMonth()
      const month = months[monthIndex]
      const day = date.getDate().toString().padStart(2, '0')
      const hours = date.getHours().toString().padStart(2, '0') // 24-hour format
      const minutes = date.getMinutes().toString().padStart(2, '0')
      const seconds = date.getSeconds().toString().padStart(2, '0')

      return `${month} ${day}, ${year} / ${hours}:${minutes}:${seconds}`
    },

    // IF PAGE REFRESHED PAUSE THE STARTED ACTIVITY
    refreshPage() {
      this.records.forEach((record) => {
        if (record.recordStatusName === 'Start') {
          this.updateStatus(record.recordId, 'Pause', record.currentRunningTime)
        }
      })
    },
  },
  mounted() {
    this.user = JSON.parse(localStorage.getItem('user'))
    if (this.user == null) {
      window.location.replace('/')
    }
    this.getRecordByUserId()
    setInterval(this.updateElapsedTime, 1000)
    window.addEventListener('beforeunload', this.refreshPage)
  },
  beforeUnmount() {
    // IF PAGE IS CHANGED  PAUSE THE STARTED ACTIVITY
    this.records.forEach((record) => {
      if (record.recordStatusName === 'Start') {
        this.updateStatus(record.recordId, 'Pause', record.currentRunningTime)
      }
    })
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
