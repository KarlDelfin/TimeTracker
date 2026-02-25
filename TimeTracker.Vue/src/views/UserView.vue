<template>
  <el-main>
    <el-card class="menuCard">
      <div class="d-flex justify-content-between">
        <h4>User</h4>
        <!-- <el-button type="primary" @click="openForm('Assign User', [])">Assign User</el-button> -->
      </div>

      <el-scrollbar height="500px">
        <el-collapse accordion>
          <el-collapse-item title="Filter Results" name="1">
            <el-form @submit.prevent="getUserAssignments">
              <el-input placeholder="Search User" v-model="searchUser" />
              <div class="d-flex justify-content-end mt-2">
                <el-button @click="clearSearch"> Reset </el-button>
                <el-button type="primary" @click="getUserAssignments"> Apply </el-button>
              </div>
            </el-form>
          </el-collapse-item>
        </el-collapse>
        <div v-if="userAssignments.length == 0">
          <el-empty description="No Data" />
        </div>
        <div v-else>
          <el-table :data="userAssignments" :row-class-name="tableRowClassName">
            <el-table-column label="Name">
              <template #default="scope">
                <img
                  @click="viewImage(scope.row.image)"
                  :src="scope.row.image"
                  :alt="`${scope.row.firstName} ${scope.row.lastName}`"
                />
                {{ `${scope.row.firstName} ${scope.row.lastName}` }}
              </template>
            </el-table-column>
            <el-table-column label="Email" prop="email" />
            <el-table-column label="Role" prop="roleName" />
            <el-table-column label="Operation" align="center">
              <template #default="scope">
                <el-button size="small" @click="openForm('Edit User', scope.row)">Edit</el-button>
                <el-button
                  size="small"
                  v-if="scope.row.isDisabled == false"
                  type="danger"
                  @click="disableUser(scope.row)"
                  >Disable</el-button
                >
                <el-button v-else size="small" type="primary" @click="disableUser(scope.row)"
                  >Enable</el-button
                >
              </template>
            </el-table-column>
          </el-table>
        </div>
      </el-scrollbar>
    </el-card>
    <el-card class="mt-2">
      <div id="paginationCard">
        <el-pagination
          v-model:current-page="userPagination.currentPage"
          v-model:page-size="userPagination.elementsPerPage"
          :page-sizes="[5, 10, 25, 50]"
          :total="userPagination.totalElements"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="getUserAssignments"
          @current-change="getUserAssignments"
        />
      </div>
    </el-card>
  </el-main>

  <el-dialog :title="title" center v-model="dialog.previewImage" :before-close="clear" width="400">
    <div class="content-center">
      <div class="d-flex justify-content-center">
        <img :src="base64Image" style="width: 200px" />
      </div>
      <el-form @submit.prevent="" class="mt-3" v-if="form.userId != '' || form.roleId != ''">
        <el-form-item label="User">
          <el-select disabled v-model="form.userId" :filter-method="getUsers" filterable>
            <el-option
              v-for="user in users"
              :key="user.userId"
              :label="`${user.firstName} ${user.lastName}`"
              :value="user.userId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="Role">
          <el-select v-model="form.roleId" :filter-method="getRoles" filterable>
            <el-option
              v-for="role in roles"
              :key="role.roleId"
              :label="`${role.roleName}`"
              :value="role.roleId"
            />
          </el-select>
        </el-form-item>
        <div class="d-flex justify-content-end">
          <el-button type="primary" @click="updateUser">Confirm</el-button>
        </div>
      </el-form>
    </div>
  </el-dialog>
</template>

<script>
import axios from 'axios'
import { ElMessage, ElLoading, ElMessageBox } from 'element-plus'
// const api = import.meta.env.VITE_APP_API_URL
// const api = 'https://calendar-api-eufwfccudhaebee4.eastasia-01.azurewebsites.net/api'
export default {
  data() {
    return {
      file: [],
      fileList: [],
      userAssignmentId: '',
      title: '',
      form: {
        userId: '',
        roleId: '',
      },
      dialog: {
        previewImage: false,
      },
      user: {},
      userAssignments: [],
      users: [],
      roles: [],
      searchUser: '',
      userPagination: {
        currentPage: 1,
        elementsPerPage: 10,
        totalElements: 0,
      },
      activities: [{ firstName: 'Karl', lastName: 'Delfin', email: 'dkhrl2000@gmail.com' }],
      tableLoading: true,
    }
  },
  methods: {
    updateUser() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      const payload = {
        userId: this.form.userId,
        roleId: this.form.roleId,
      }
      axios
        .put(`${this.api}/UserAssignment/${this.userAssignmentId}`, payload)
        .then((response) => {
          if (response.data == 'success') {
            ElMessage.success('User Assignment updated successfully')
            this.clear()
            loading.close()
          } else {
            ElMessage.error(response.data)
            loading.close()
          }
        })
        .catch((e) => {
          ElMessage.error(e)
          loading.close()
        })
    },
    getRoles(e) {
      if (e == undefined) {
        e = ''
      }
      axios
        .get(`${this.api}/Role?search=${e}&currentPage=1&elementsPerPage=10`)
        .then((response) => {
          this.roles = response.data.results
        })
    },
    getUsers(e) {
      if (e == undefined) {
        e = ''
      }
      axios
        .get(`${this.api}/User?search=${e}&currentPage=1&elementsPerPage=10`)
        .then((response) => {
          this.users = response.data.results
        })
    },
    openForm(title, data) {
      this.title = title

      if (title == 'Edit User') {
        this.userAssignmentId = data.userAssignmentId
        this.base64Image = data.image
        this.form.userId = data.userId
        this.form.roleId = data.roleId
        this.dialog.previewImage = true
        this.getRoles()
        this.getUsers()
      }
    },
    tableRowClassName({ row }) {
      if (row.isDisabled == true) {
        return 'danger-row'
      }
      return ''
    },
    disableUser(data) {
      ElMessageBox.confirm(
        `Do you want to ${data.isDisabled == false ? 'disable' : 'enable'} this user?`,
        'Warning',
        {
          confirmButtonText: 'Confirm',
          cancelButtonText: 'Cancel',
          type: 'warning',
        },
      )
        // CONFIRM
        .then(() => {
          const loading = ElLoading.service({
            lock: true,
            text: 'Loading',
            background: 'rgba(0, 0, 0, 0.7)',
          })
          axios
            .delete(`${this.api}/UserAssignment/${data.userAssignmentId}`)
            .then(() => {
              loading.close()
              this.getUserAssignments()
              ElMessage.success('User disabled successfully')
            })
            .catch((e) => {
              ElMessage.error(e)
            })
        })
        // CANCEL
        .catch(() => {})
    },
    viewImage(data) {
      this.base64Image = data
      this.dialog.previewImage = true
    },
    clearSearch() {
      this.searchUser = ''
      this.getUserAssignments()
    },
    clear() {
      this.userAssignmentId = ''
      this.title = ''
      this.base64Image = ''
      this.dialog.previewImage = false
      this.dialog.addUser = false
      this.base64 = ''
      this.form.userId = ''
      this.form.roleId = ''
    },
    getUserAssignments() {
      this.tableLoading = true
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      axios
        .get(
          `${this.api}/UserAssignment?search=${this.searchUser}&currentPage=${this.userPagination.currentPage}&elementsPerPage=${this.userPagination.elementsPerPage}`,
        )
        .then((response) => {
          loading.close()
          this.userAssignments = response.data.results
          this.userPagination.totalElements = response.data.totalElements
          this.tableLoading = false
        })
    },
  },
  mounted() {
    this.user = JSON.parse(localStorage.getItem('user'))
    if (this.user == null) {
      window.location.replace('/')
    }
    this.getUserAssignments()
  },
}
</script>

<style>
.menuCard {
  min-height: 70vh;
  max-height: calc(92vh - 32px);
  overflow: hidden;
}

.cell img {
  width: 50px;
  border-radius: 10px;
  margin: 0 10px 0 0;
  cursor: pointer;
}

.cell img:hover {
  opacity: 0.8;
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
.el-table .danger-row {
  --el-table-tr-bg-color: var(--el-color-danger-light-9);
}
</style>
