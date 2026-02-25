<template>
  <div class="d-flex justify-content-center align-items-center background">
    <!-- START FORGOT PASSWORD -->
    <div
      v-if="isVerifyCode == false && isResetPassword == false"
      class="shadow col-sm-4 p-4 rounded bg-white"
    >
      <h2 class="text-center">Forgot Password</h2>
      <el-form class="mt-4" label-position="top" @submit.prevent="sendCode">
        <el-form-item>
          <el-input v-model="email" placeholder="Email" />
        </el-form-item>
      </el-form>
      <div class="d-flex justify-content-end">
        <el-button type="primary" class="w-100" @click="sendCode">Send</el-button>
      </div>
      <div class="d-flex justify-content-center mt-3 mb-2">
        <el-button type="primary" link @click="$router.push('/')">Back to Login</el-button>
      </div>
    </div>
    <!-- END FORGOT PASSWORD -->

    <!-- START VERIFY CODE -->
    <div
      v-if="isVerifyCode == true && isResetPassword == false"
      class="shadow col-sm-4 p-4 rounded bg-white"
    >
      <h2 class="text-center">Verify Code</h2>
      <el-form class="mt-4" label-position="top" @submit.prevent="verifyCode">
        <el-form-item>
          <el-input v-model="code" placeholder="Code" />
        </el-form-item>
      </el-form>
      <div class="d-flex justify-content-end">
        <el-button type="primary" class="w-100" @click="verifyCode">Verify</el-button>
      </div>
      <div class="d-flex justify-content-center mt-3 mb-2">
        <el-button type="primary" link @click="$router.push('/')">Back to Login</el-button>
      </div>
    </div>
    <!-- END VERIFY CODE -->

    <!-- START RESET PASSWORD -->
    <div
      v-if="isVerifyCode == false && isResetPassword == true"
      class="shadow col-sm-4 p-4 rounded bg-white"
    >
      <h2 class="text-center">Reset Password</h2>
      <el-form class="mt-4" label-position="top" @submit.prevent="resetPassword">
        <el-form-item>
          <el-input v-model="password" type="password" show-password placeholder="Password" />
        </el-form-item>
        <el-form-item>
          <el-input
            v-model="confirmPassword"
            type="password"
            show-password
            placeholder="Confirm Password"
          />
        </el-form-item>
      </el-form>
      <div class="d-flex justify-content-end">
        <el-button type="primary" class="w-100" @click="resetPassword">Login</el-button>
      </div>
      <div class="d-flex justify-content-center mt-3 mb-2">
        <el-button type="primary" link @click="$router.push('/')">Back to Login</el-button>
      </div>
    </div>
    <!-- END RESET PASSWORD -->
  </div>
</template>

<script>
import axios from 'axios'
import { ElMessage, ElLoading } from 'element-plus'
// const api = import.meta.env.VITE_APP_API_URL
// const api = 'https://calendar-api-eufwfccudhaebee4.eastasia-01.azurewebsites.net/api'
export default {
  data() {
    return {
      isVerifyCode: false,
      isResetPassword: false,

      email: '',
      code: '',
      password: '',
      confirmPassword: '',
    }
  },
  methods: {
    // SEND CODE
    sendCode() {
      const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })

      // CHECK INPUT FIELD
      if (this.email == '') {
        setTimeout(() => {
          loading.close()
        }, 500),
          ElMessage.warning('Please input email address')
        return
      }

      // CHECK EMAIL
      if (!pattern.test(this.email)) {
        setTimeout(() => {
          loading.close()
        }, 500),
          ElMessage.warning('Please input valid email address')
        return
      }
      axios
        .get(`${this.api}/User/Email/${this.email}`)
        .then((response) => {
          if (response.data == 'Failed to send verification code') {
            loading.close()
            ElMessage.warning('Email address not registered')
            return
          }
          loading.close()
          this.email = response.data.email
          this.isVerifyCode = true
          this.isResetPassword = false
          ElMessage.success('Verification code sent successfully')
        })
        .catch((e) => {
          loading.close()
          ElMessage.error(e)
          this.email = ''
        })
    },

    // VERIFY CODE
    verifyCode() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })

      axios
        .get(`${this.api}/User/Email/${this.email}/VerificationCode/${this.code}`)
        .then((response) => {
          if (this.code != response.data.verificationCode) {
            loading.close()
            ElMessage.warning('Invalid verification code')
            return
          }
          ElMessage.success('Code verified')
          this.isVerifyCode = false
          this.isResetPassword = true
          loading.close()
        })
        .catch(() => {
          loading.close()
          ElMessage.warning('Invalid verification code')
          this.code = ''
        })
    },

    // RESET PASSWORD
    resetPassword() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })

      // CHECK INPUT FIELDS
      if (this.password == '' || this.confirmPassword == '') {
        setTimeout(() => {
          loading.close()
        }, 500),
          ElMessage.warning('Please input fields!')
        return
      }

      // CHECK PASSWORD
      if (this.password != this.confirmPassword) {
        setTimeout(() => {
          loading.close()
        }, 500),
          ElMessage.warning('Password not match!')
        return
      }

      axios
        .put(`${this.api}/User/Email/${this.email}/Password/${this.password}`)
        .then((response) => {
          if (response.data == 'success') {
            loading.close()
            ElMessage.success('Password updated successfully')
            setTimeout(() => {
              window.location.replace('/')
            }, 500)
          }
        })
        .catch((e) => {
          loading.close()
          ElMessage.error(e)
        })
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
</style>
