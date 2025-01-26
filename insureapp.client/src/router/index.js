import { createRouter, createWebHistory } from 'vue-router'
import NotFound from '../views/NotFound.vue'

const routes = [
  {
    path: '/',
    redirect: '/reports'
  },
  {
    path: '/reports',
    name: 'Reports',
    component: () => import('../views/reports/ReportsList.vue')
  },
  {
    path: '/reports/new',
    name: 'NewReport',
    component: () => import('../views/reports/CreateReport.vue')
  },
  {
    path: '/reports/:id',
    name: 'ReportDetails',
    component: () => import('../views/reports/ReportDetails.vue')
  },
  {
    path: '/users',
    name: 'Users',
    component: () => import('../views/users/UsersList.vue')
  },
  {
    path: '/users/new',
    name: 'NewUser',
    component: () => import('../views/users/CreateUser.vue')
  },
  {
    path: '/users/:id',
    name: 'UserDetails',
    component: () => import('../views/users/UserDetails.vue')
  },
  {
    path: '/agents',
    name: 'Agents',
    component: () => import('../views/agents/AgentsList.vue')
  },
  {
    path: '/agents/new',
    name: 'NewAgent',
    component: () => import('../views/agents/CreateAgent.vue')
  },
  {
    path: '/agents/:id',
    name: 'AgentDetails',
    component: () => import('../views/agents/AgentDetails.vue')
  },
  {
    path: '/insurance-types',
    name: 'InsuranceTypes',
    component: () => import('../views/insurance/InsuranceTypesList.vue')
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFound
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
