import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import TimeTracker from '../views/TimeTracker.vue'
import ActivityView from '../views/ActivityView.vue'
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
      path: '/time-tracker',
      name: 'timeTracker',
      component: TimeTracker,
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
