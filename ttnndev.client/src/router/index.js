import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import MainLayout from '../layouts/MainLayout.vue';

const routes = [
  { path: '/login', component: () => import('../views/Login.vue') },
  { path: '/register', name: 'Register', component: () => import('../views/Register.vue') }, // ĐÃ THÊM DẤU PHẨY Ở ĐÂY
  {
    path: '/',
    component: MainLayout,
    redirect: '/dashboard',
    meta: { requiresAuth: true },
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

  // Cho phép vào trang đăng nhập HOẶC đăng ký mà không cần check auth
  if (to.path === '/login' || to.path === '/register') {
    if (authStore.token) {
      next('/dashboard'); // Nếu đã đăng nhập thì về thẳng dashboard
    } else {
      next(); // Cho phép vào trang login/register
    }
  } else {
    // Các trang khác bắt buộc phải đăng nhập
    if (to.meta.requiresAuth && !authStore.token) {
      next('/login');
    } else {
      next();
    }
  }
});

export default router;
