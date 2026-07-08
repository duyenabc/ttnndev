import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth'; // Import store của bạn
import MainLayout from '../layouts/MainLayout.vue';

const routes = [
  { path: '/login', component: () => import('../views/Login.vue') },
  {
    path: '/',
    component: MainLayout,
    redirect: '/dashboard',
    meta: { requiresAuth: true }, // Đánh dấu trang này cần đăng nhập
    children: [
      { path: 'dashboard', component: () => import('../views/Dashboard.vue') },
      { path: 'diaries', component: () => import('../views/Diaries.vue') },
      { path: 'scores', component: () => import('../views/Scores.vue') }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// THIẾT LẬP NAVIGATION GUARD
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();

  // Kiểm tra xem trang đích có yêu cầu đăng nhập không
  if (to.meta.requiresAuth && !authStore.token) {
    // Nếu chưa đăng nhập, đá về trang login
    next('/login');
  } else if (to.path === '/login' && authStore.token) {
    // Nếu đã đăng nhập rồi mà cố vào trang login thì đá về dashboard
    next('/dashboard');
  } else {
    // Cho phép đi tiếp
    next();
  }
});

export default router;
