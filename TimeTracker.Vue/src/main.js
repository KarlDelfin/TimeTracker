import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-icons/font/bootstrap-icons.css'

import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(router)
app.use(ElementPlus)
// app.config.globalProperties.api = 'https://localhost:7095/api'
app.config.globalProperties.api =
  'https://timetracker-api-ecbyacarcngbgqes.eastasia-01.azurewebsites.net/api'
app.mount('#app')
