import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import ForgotPasswordView from '../views/ForgotPasswordView.vue'
import RegisterView from '../views/RegisterView.vue'
import TimeTracker from '../views/TimeTracker.vue'
import ActivityView from '../views/ActivityView.vue'
import UserTaskView from '../views/UserTaskView.vue'
import UserView from '../views/UserView.vue'
import NotFound from '../views/NotFound.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: LoginView,
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
    },
    {
      path: '/forgot-password',
      name: 'forgotPassword',
      component: ForgotPasswordView,
    },
    {
      path: '/timetracker',
      name: 'timeTracker',
      component: TimeTracker,
    },
    {
      path: '/user-task',
      name: 'user-task',
      component: UserTaskView,
    },
    {
      path: '/activity',
      name: 'activity',
      component: ActivityView,
    },
    {
      path: '/user',
      name: 'user',
      component: UserView,
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: NotFound,
      // meta: {
      //   isPublic: true,
      // },
    },
  ],
})

export default router
