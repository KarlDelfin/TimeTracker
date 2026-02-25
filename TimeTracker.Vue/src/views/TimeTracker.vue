<template>
  <el-main>
    <el-card class="menuCard">
      <div class="d-flex justify-content-between">
        <div><h4>Time Tracker</h4></div>
        <div class="d-flex justify-content-end mb-2">
          <el-button type="primary" @click="openForm('Assign Activity')" v-if="user.roleLevel == 1"
            >Assign Activity</el-button
          >
        </div>
      </div>

      <el-scrollbar height="500px">
        <el-collapse accordion>
          <el-collapse-item title="Filter Results" name="1">
            <el-form @submit.prevent="getRecordByUserId()">
              <el-input placeholder="Search Record" v-model="search" />
              <div class="d-flex justify-content-end mt-2">
                <el-button @click="(clear(), getRecordByUserId())"> Reset </el-button>
                <el-button type="primary" @click="getRecordByUserId()"> Apply </el-button>
              </div>
            </el-form>
          </el-collapse-item>
        </el-collapse>

        <el-table :data="records" :row-class-name="tableRowClassName">
          <el-table-column label="Elapsed Time" prop="currentRunningTime">
            <template #default="scope">
              <span class="border p-2 rounded">
                {{ formatElapsedTime(scope.row.currentRunningTime, scope.$index) }}
              </span>
            </template>
          </el-table-column>
          <el-table-column label="Assigned to">
            <template #default="scope">
              {{ `${scope.row.firstName} ${scope.row.lastName}` }}
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
              <div v-if="scope.row.currentStatus == 'Start'">
                <el-tag type="primary">Started</el-tag>
              </div>
              <div v-else-if="scope.row.currentStatus == 'Resume'">
                <el-tag type="primary">Resumed</el-tag>
              </div>
              <div v-else-if="scope.row.currentStatus == 'Pause'">
                <el-tag type="warning">Paused</el-tag>
              </div>
              <div v-else-if="scope.row.currentStatus == 'Complete'">
                <el-tag type="success">Completed</el-tag>
              </div>
              <div v-else-if="scope.row.currentStatus == 'Reassign'">
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
              <span class="mx-1" v-if="scope.row.currentStatus == null">
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
                      :style="scope.row.currentStatus == 'Start' ? 'border: 1px solid #409EFF' : ''"
                      :disabled="
                        scope.row.currentStatus == 'Start' ||
                        scope.row.currentStatus == 'Complete' ||
                        activeRowId != null
                      "
                      size="small"
                      ><i class="bi bi-play-fill"></i></el-button
                  ></template>
                </el-popconfirm>
              </span>
              <!-- END [START] -->

              <!-- START [RESUME] -->
              <span class="mx-1" v-else>
                <el-popconfirm
                  hide-icon
                  width="250px"
                  title="Do you want to resume this Activity?"
                  confirmButtonText="Yes"
                  cancelButtonText="No"
                  @confirm="
                    updateStatus(scope.row.recordId, 'Resume', scope.row.currentRunningTime)
                  "
                  @cancel="getRecordByUserId()"
                >
                  <template #reference>
                    <el-button
                      :style="
                        scope.row.currentStatus == 'Resume' || scope.row.currentStatus == 'Start'
                          ? 'border: 1px solid #409EFF'
                          : ''
                      "
                      :disabled="
                        scope.row.currentStatus == 'Start' ||
                        scope.row.currentStatus == 'Complete' ||
                        activeRowId != null
                      "
                      size="small"
                      ><i class="bi bi-play-fill"></i></el-button
                  ></template>
                </el-popconfirm>
              </span>

              <!-- END [RESUME] -->

              <!-- START [PAUSE] -->
              <span class="mx-1">
                <el-button
                  :style="
                    scope.row.currentStatus == 'Pause' || scope.row.currentStatus == null
                      ? 'border: 1px solid #E6A23C'
                      : ''
                  "
                  :disabled="
                    scope.row.currentStatus == 'Pause' ||
                    scope.row.currentStatus == null ||
                    scope.row.currentStatus == 'Complete'
                  "
                  size="small"
                  @click="updateStatus(scope.row.recordId, 'Pause', scope.row.currentRunningTime)"
                  ><i class="bi bi-pause-fill"></i
                ></el-button>
              </span>
              <!-- END [PAUSE] -->

              <!-- START [COMPLETE] -->
              <span class="mx-1">
                <el-popconfirm
                  hide-icon
                  width="250px"
                  title="Do you want to complete this Activity?"
                  confirmButtonText="Yes"
                  cancelButtonText="No"
                  @confirm="
                    updateStatus(scope.row.recordId, 'Complete', scope.row.currentRunningTime)
                  "
                >
                  <template #reference>
                    <el-button
                      :style="
                        scope.row.currentStatus == 'Complete' ? 'border: 1px solid #67C23A' : ''
                      "
                      :disabled="
                        scope.row.currentStatus == 'Complete' ||
                        scope.row.currentStatus == null ||
                        scope.row.currentStatus == 'Reassign'
                      "
                      size="small"
                      ><span><i class="bi bi-stop-fill"></i></span
                    ></el-button>
                  </template>
                </el-popconfirm>
              </span>
              <!-- END [COMPLETE] -->

              <!-- START [REMOVE] -->
              <el-button
                class="mx-1"
                v-if="user.roleLevel == 1"
                type="danger"
                size="small"
                @click="deleteRecord(scope.row.recordId)"
                ><i class="bi bi-x"></i
              ></el-button>
              <!-- END [REMOVE] -->

              <!-- START [REASSIGN] -->
              <el-button
                class="w-50 mt-1"
                v-if="user.roleLevel == 1"
                size="small"
                @click="openForm('Reassign Activity', scope.row)"
                >Reassign</el-button
              >
              <!-- END [REASSIGN] -->
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
  <el-dialog :title="title" v-model="dialog.recordForm" center width="600" :before-close="clear">
    <el-form label-position="top" @submit.prevent="submitForm">
      <div
        class="d-flex justify-content-between"
        label="Activity Name"
        v-for="(activity, index) in recordForm"
        :key="index"
        :rules="[{ required: true }]"
      >
        <el-form-item>
          <el-select
            :style="recordForm.length > 1 ? 'width: 250px' : 'width: 280px'"
            v-model="activity.userId"
            filterable
            :filter-method="getUser"
            placeholder="Select User"
          >
            <el-option
              v-for="(user, index) in users"
              :key="index"
              :label="`${user.firstName} ${user.lastName} - ${user.email}`"
              :value="user.userId"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-select
            :style="recordForm.length > 1 ? 'width: 250px' : 'width: 280px'"
            v-model="activity.activityId"
            placeholder="Select Activity"
            :filter-method="getActivityByUserId"
            filterable
          >
            <el-option
              v-show="true"
              v-for="activity in filterActivity"
              :key="activity.activityId"
              :label="`${activity.activityName} - ${
                activity.activityEstimatedTime > 1
                  ? `${activity.activityEstimatedTime} hours`
                  : `${activity.activityEstimatedTime} hour`
              }
                `"
              :value="activity.activityId"
            />
            <el-option
              v-show="false"
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
        </el-form-item>
        <div v-if="recordForm.length > 1" class="col-1">
          <el-button type="danger" @click="removePushedRecord(index)"
            ><i class="bi bi-x"></i
          ></el-button>
        </div>
      </div>
      <el-button size="small" @click="addMore">Add More</el-button>
    </el-form>
    <template #footer>
      <div class="d-flex justify-content-end">
        <el-button type="primary" @click="submitForm">Confirm</el-button>
      </div>
    </template>
  </el-dialog>
  <!-- END RECORD DIALOG -->

  <!-- START REASSIGN DIALOG -->
  <el-dialog
    :title="title"
    v-model="dialog.reassignRecordForm"
    center
    width="600"
    :before-close="clear"
  >
    <el-form label-position="top" @submit.prevent="submitForm">
      <el-form-item>
        <div class="pt-2">
          <span class="fw-bold">Activity Name: </span>
          <span class="pt-2">{{ `${form.activityName}` }}</span>
        </div>
      </el-form-item>
      <el-form-item>
        <el-select
          :style="recordForm.length > 1 ? 'width: 250px' : 'width: 280px'"
          v-model="form.userId"
          filterable
          :filter-method="getUser"
          placeholder="Select User"
        >
          <el-option
            v-for="(user, index) in users"
            :key="index"
            :label="`${user.firstName} ${user.lastName} - ${user.email}`"
            :value="user.userId"
          />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="d-flex justify-content-end">
        <el-button type="primary" @click="reassignRecord">Confirm</el-button>
      </div>
    </template>
  </el-dialog>
  <!-- END REASSIGN DIALOG -->

  <!-- START VIEW RECORD -->
  <el-dialog :title="title" v-model="dialog.viewRecord" center width="600" :before-close="clear">
    <div class="pt-2">
      <span class="fw-bold">Assigned To: </span>
      <span class="pt-2">{{ `${form.firstName} ${form.lastName}` }}</span>
    </div>
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

  <div class="elapse_time" v-if="activeRowId != null" position="bottom" :offset="20">
    <div>
      <p><b>Activity Name:</b> {{ recordDetails.activityName }}</p>
    </div>
    <div>
      <p><b>Elapsed Time:</b> {{ formatElapsedTime(recordDetails.elapsedTime) }}</p>
    </div>

    <div>
      <el-button disabled style="border: 1px solid #409eff"
        ><i class="bi bi-play-fill"></i
      ></el-button>
      <el-button @click="updateStatus(recordId, 'Pause', recordDetails.elapsedTime)"
        ><i class="bi bi-pause-fill"></i
      ></el-button>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import { ElMessage, ElMessageBox, ElLoading } from 'element-plus'
// const api = import.meta.env.VITE_APP_API_URL

export default {
  data() {
    return {
      userSearch: '',
      search: '',
      activeRowId: null,
      recordId: '',
      form: {
        activityId: '',
        userId: '',
        recordId: '',
        firstName: '',
        lastName: '',
        activityName: '',
        activityDescription: '',
        activityEstimatedTime: '',
      },
      title: '',
      users: [],
      recordForm: [{ activityId: '', userId: '' }],
      tempRecordForm: [{ activityId: '', userId: '' }],
      dialog: {
        recordForm: false,
        viewRecord: false,
        reassignRecordForm: false,
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
      recordDetails: {
        activityName: '',
        elapsedTime: '',
      },
    }
  },
  computed: {
    filterActivity() {
      return this.activities.filter(
        (a) => !this.recordForm.some((r) => r.activityId === a.activityId),
      )
    },
  },

  methods: {
    reassignRecord() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      const payload = {
        userId: this.form.userId,
        activityId: this.form.activityId,
      }
      axios
        .put(`${this.api}/Record/${this.form.recordId}`, payload)
        .then((response) => {
          if (response.data == 'success') {
            loading.close()
            ElMessage.success('Task reassigned successfully')
            this.getRecordByUserId()
            this.clear()
          } else {
            loading.close()
            ElMessage.error(response.data)
            this.clear()
          }
        })
        .catch((e) => {
          loading.close()
          ElMessage.error(e)
          this.clear()
        })
    },
    getRecordByRecordId(recordId) {
      axios.get(`${this.api}/Record/${recordId}`).then((response) => {
        this.recordDetails.activityName = response.data.activityName
        this.recordDetails.elapsedTime = response.data.currentRunningTime
      })
    },
    tableRowClassName({ row }) {
      if (row.currentStatus == 'Resume' || row.currentStatus == 'Start') {
        return 'primary-row'
      }
      if (row.currentStatus == 'Start' || row.currentStatus == 'Complete') {
        return 'success-row'
      }
      if (row.currentStatus == 'Pause') {
        return 'warning-row'
      }
      // if (row.currentStatus == null) {
      //   return 'danger-row'
      // }
      return ''
    },
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
      this.form.userId = ''
      this.form.recordId = ''
      this.form.firstName = ''
      this.form.lastName = ''
      this.form.activityName = ''
      this.form.activityDescription = ''
      this.form.activityEstimatedTime = ''
      this.dialog.recordForm = false
      this.dialog.viewRecord = false
      this.dialog.reassignRecordForm = false
      this.recordForm = []
      this.recordForm.push({
        activityId: '',
        userId: '',
      })
      this.title = ''
      this.recordId = ''
      this.search = ''
    },

    // OPEN FORM
    openForm(title, data) {
      this.title = title
      if (title == 'Reassign Activity') {
        this.dialog.reassignRecordForm = true
        this.form.activityId = data.activityId
        this.form.userId = data.userId
        this.form.recordId = data.recordId
        this.form.activityName = data.activityName
        this.form.userId
        this.form.recordId
        this.getUser('')
      }
      if (title == 'Assign Activity') {
        this.dialog.recordForm = true
        this.getActivityByUserId(null)
        this.getUser('')
      }
      if (title == 'Record Details') {
        this.recordId = data.recordId
        this.dialog.viewRecord = true
        this.form.firstName = data.firstName
        this.form.lastName = data.lastName
        this.form.activityName = data.activityName
        this.form.activityDescription = data.activityDescription
        this.form.activityEstimatedTime = data.activityEstimatedTime
        this.getStatusLogByRecordId(data.recordId)
      }
    },

    // GET USER
    getUser(e) {
      axios.get(`${this.api}/User?search=${e}`).then((response) => {
        this.users = response.data.results
      })
    },

    // DELETE RECORD
    deleteRecord(recordId) {
      ElMessageBox.confirm('Do you want to remove this Record?', 'Warning', {
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancel',
        type: 'warning',
      })
        .then(() => {
          const loading = ElLoading.service({
            lock: true,
            text: 'Loading',
            background: 'rgba(0, 0, 0, 0.7)',
          })
          axios
            .delete(`${this.api}/Record/${recordId}`)
            .then(() => {
              loading.close()
              this.getRecordByUserId()
              ElMessage.success('Record removed successfully')
            })
            .catch((error) => {
              loading.close()
              ElMessage.error(error)
            })
        })
        .catch(() => {})
    },

    // GET STATUS BY RECORD ID
    getStatusLogByRecordId(recordId) {
      axios
        .get(
          `${this.api}/StatusLog/Record/${recordId}?currentPage=${this.statusLogPagination.currentPage}&elementsPerPage=${this.statusLogPagination.elementsPerPage}`,
        )
        .then((response) => {
          this.statusLogs = response.data.results
          this.statusLogPagination.totalElements = response.data.totalElements
        })
    },
    // START, PAUSE, COMPLETE
    updateStatus(recordId, statusName, currentRunningTime) {
      this.recordId = recordId
      this.getRecordByRecordId(recordId)
      var payload = {
        recordId: recordId,
        statusName: statusName,
        currentRunningTime: currentRunningTime,
      }
      if (statusName == 'Start' || statusName == 'Resume') {
        this.activeRowId = recordId
        localStorage.setItem('elapsedTime', currentRunningTime)
      } else {
        this.activeRowId = null
        localStorage.setItem('elapsedTime', currentRunningTime)
      }
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      axios
        .post(`${this.api}/StatusLog`, payload)
        .then(() => {
          if (statusName == 'Start') {
            ElMessage.success(`Activity has been Started`)
          } else if (statusName == 'Resume') {
            ElMessage.success(`Activity has been Resumed`)
          } else if (statusName == 'Pause') {
            ElMessage.success(`Activity has been Paused`)
          }

          this.getRecordByUserId()
        })
        .catch((error) => {
          loading.close()
          ElMessage.error(error)
        })
    },

    // SUBMIT FORM
    submitForm() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      if (this.title == 'Assign Activity') {
        const payload = this.recordForm.map((data) => ({
          activityId: data.activityId,
          userId: data.userId,
        }))
        axios
          .post(`${this.api}/Record`, payload)
          .then(() => {
            loading.close()
            ElMessage.success('Records added succesfully')
            this.getRecordByUserId()
            this.clear()
          })
          .catch((e) => {
            loading.close()
            ElMessage.error(e)
          })
      }
    },

    // GET RECORD BY USER ID
    getRecordByUserId(data) {
      if (typeof data == 'number') {
        this.records.forEach((record) => {
          if (record.currentStatus === 'Start' || record.currentStatus === 'Resume') {
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
          `${this.api}/Record/User/${this.user.userId}?currentPage=${this.recordPagination.currentPage}&elementsPerPage=${this.recordPagination.elementsPerPage}&search=${this.search}`,
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
          `${this.api}/Activity/User/${this.user.userId}?currentPage=${this.activityPagination.currentPage}&elementsPerPage=${this.activityPagination.elementsPerPage}&search=${data}&isFiltered=true`,
        )
        .then((response) => {
          this.activities = response.data.results
          this.activityPagination.totalElements = response.data.totalElements
        })
    },

    // REACTIVE ELAPSE TIME
    updateElapsedTime() {
      this.records.forEach((record) => {
        if (record.currentStatus === 'Start' || record.currentStatus == 'Resume') {
          record.currentRunningTime += 1000
          this.activeRowId = record.recordId
        }
      })
      this.recordDetails.elapsedTime += 1000
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
        if (record.currentStatus === 'Start' || record.currentStatus == 'Resume') {
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
      if (record.currentStatus === 'Start' || record.currentStatus == 'Resume') {
        this.updateStatus(record.recordId, 'Pause', record.currentRunningTime)
        this.activeRowId = null
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
.elapse_time {
  position: absolute;
  bottom: 20px;
  right: 20px;
  background: #fff;
  border: 1px solid #ebeef5;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  padding: 16px 20px;
  border-radius: 12px;
  min-width: 280px;
  z-index: 1000;
  transition: all 0.3s ease;
}

.elapse_time div:last-child {
  display: flex;
  justify-content: center;
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
.el-select {
  width: 280px;
}
</style>
