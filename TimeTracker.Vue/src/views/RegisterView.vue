<template>
  <div class="d-flex justify-content-center align-items-center background">
    <div class="shadow col-sm-4 p-4 rounded bg-white">
      <h2 class="text-center">Register</h2>
      <el-form class="mt-4" label-position="top" @submit.prevent="register">
        <div class="d-flex justify-content-center">
          <el-upload
            drag
            ref="uploader"
            v-model:file-list="fileList"
            action="getUploadTrigger"
            list-type="picture-card"
            :on-success="onSuccess"
            :on-preview="onPreview"
            :on-remove="onRemove"
            :before-upload="beforeUpload"
            method="get"
            accept="image/jpeg, image/png, image/jpg"
            :limit="1"
            :on-exceed="onExceed"
            class="mb-4"
          >
            <i class="bi bi-cloud-arrow-up-fill fs-3"></i>
            <div class="el-upload__text">Drop file here or <em>click to upload</em></div>
          </el-upload>
        </div>
        <el-form-item>
          <el-input v-model="form.firstName" placeholder=" First Name" />
        </el-form-item>
        <el-form-item>
          <el-input v-model="form.lastName" placeholder=" Last Name" />
        </el-form-item>
        <el-form-item>
          <el-input v-model="form.email" placeholder=" Email" />
        </el-form-item>
        <el-form-item>
          <el-input v-model="form.password" type="password" show-password placeholder=" Password" />
        </el-form-item>
        <el-form-item>
          <el-input
            v-model="form.confirmPassword"
            type="password"
            show-password
            placeholder=" Confirm Password"
          />
        </el-form-item>
      </el-form>
      <div class="d-flex justify-content-end">
        <el-button type="primary" class="w-100" @click="register">Register</el-button>
      </div>
      <div class="d-flex justify-content-center mt-3 mb-2">
        <el-button type="primary" link @click="$router.push('/')">Back to Login</el-button>
      </div>
      <div class="d-flex justify-content-center mt-3">
        <el-button link @click="$router.push('/forgot-password')">Forgot Password?</el-button>
      </div>
    </div>
  </div>

  <!-- Image Preview Dialog -->
  <el-dialog v-model="dialog.previewImage" :before-close="clear">
    <div class="d-flex justify-content-center">
      <div>
        <img :src="previewImg" style="width: 300px" />
      </div>
    </div>
  </el-dialog>
</template>

<script>
import axios from 'axios'
import { ElMessage, ElLoading } from 'element-plus'
// const api = import.meta.env.VITE_APP_API_URL
// const api = 'https://calendar-api-eufwfccudhaebee4.eastasia-01.azurewebsites.net/api'
export default {
  data() {
    return {
      form: {
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
      },
      fileList: [],
      file: [],
      dialog: {
        previewImage: false,
      },
    }
  },
  methods: {
    // CLEAR
    clear() {
      this.dialog.previewImage = false
      this.previewImage = null
      this.form.firstName = ''
      this.form.lastName = ''
      this.form.email = ''
      this.form.password = ''
      this.form.confirmPassword = ''
      this.fileList = []
      this.file = []
    },

    // REGISTER
    register() {
      const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })

      // CHECK IMAGE
      if (this.file.length == 0) {
        setTimeout(() => {
          loading.close()
        }, 500)
        ElMessage.warning('Please select image')
        return
      }

      // CHECK INPUT FIELDS
      if (
        this.form.firstName == '' ||
        this.form.lastName == '' ||
        this.form.email == '' ||
        this.form.password == '' ||
        this.form.confirmPassword == ''
      ) {
        setTimeout(() => {
          loading.close()
        }, 500)
        ElMessage.warning('Please input fields')
        return
      }

      // CHECK EMAIL
      if (!pattern.test(this.form.email)) {
        setTimeout(() => {
          loading.close()
        }, 500)
        ElMessage.warning('Please input valid email address')
        return
      }

      // CHECK PASSWORD MATCH
      if (this.form.password != this.form.confirmPassword) {
        setTimeout(() => {
          loading.close()
        }, 500)
        ElMessage.warning('Password not match')
        return
      }

      const formData = new FormData()
      this.file.forEach((file) => {
        formData.append('firstname', this.form.firstName)
        formData.append('lastname', this.form.lastName)
        formData.append('email', this.form.email)
        formData.append('password', this.form.password)
        formData.append('image', file)
      })
      axios
        .post(`${this.api}/User`, formData)
        .then((response) => {
          if (response.data == 'success') {
            ElMessage.success('Account created successfully')
            this.clear()
            setTimeout(() => {
              loading.close()
              window.location.replace('/')
            }, 1000)
          } else {
            setTimeout(() => {
              loading.close()
            }, 500)

            loading.close()
            ElMessage.error(response.data)
          }
        })
        .catch((e) => {
          setTimeout(() => {
            loading.close()
          }, 500)
          this.clear()
          ElMessage.error(e)
        })
    },

    // ON EXCEED FILE SIZE
    onExceed(files) {
      this.$refs.uploader.clearFiles()
      this.$refs.uploader.handleStart(files[0])
    },

    // PREVIEW IMAGE
    onPreview(file) {
      this.previewImg = file.url || file.thumbUrl
      this.dialog.previewImage = true
    },

    // SELECTED IMAGES
    onSuccess(res, file) {
      this.file.push(file.raw)
    },

    // REMOVE SELECTED IMAGE
    onRemove(file) {
      this.file = this.file.filter((f) => f.uid !== file.uid)
    },

    // CHECK FILE FORMAT / FILE SIZE
    beforeUpload(file) {
      if (file.type !== 'image/jpeg' && file.type !== 'image/png') {
        ElMessage.warning('File type should be JPG, PNG')
        return false
      }
      if (file.size > 10000000) {
        ElMessage.warning('File size should be less than 10MB')
        return false
      }
    },
  },

  mounted() {
    if (localStorage.getItem('user') != null) {
      window.location.replace('/timetracker')
    }
  },
}
</script>

<style>
.background {
  background-image: url('../assets/login.jpg');
  width: 100%;
  height: 100%;
  background-repeat: no-repeat;
  background-size: cover;
  min-height: 100vh;
}

.el-upload el-upload--picture-card {
  display: none;
}

.el-upload-dragger {
  height: 100%;
}

.bi-cloud-arrow-up-fill::before {
  color: #606266;
}
</style>
