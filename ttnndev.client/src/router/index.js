import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import MainLayout from '../layouts/MainLayout.vue';

const routes = [
  { path: '/login', component: () => import('../views/Login.vue') },
  { path: '/register', name: 'Register', component: () => import('../views/Register.vue') },
  { path: '/forgot-password', component: () => import('../views/ForgotPassword.vue') },
  { path: '/reset-password', component: () => import('../views/ResetPassword.vue') },
  { path: '/activate', component: () => import('../views/Activate.vue') },
  {
    path: '/',
    component: MainLayout,
    redirect: '/dashboard',
    meta: { requiresAuth: true },
    children: [
      { path: 'dashboard', component: () => import('../views/Dashboard.vue') },
      { path: 'profile', component: () => import('../views/Profile.vue') },
      { path: 'change-password', component: () => import('../views/ChangePassword.vue') },

      // Sinh viên
      { path: 'diaries', component: () => import('../views/Diaries.vue'), meta: { roles: ['SinhVien'] } },

      // Giảng viên
      { path: 'scores', component: () => import('../views/Scores.vue'), meta: { roles: ['GiangVien'] } },
      {
        path: 'teacher-students',
        name: 'TeacherStudents',
        component: () => import('../views/TeacherStudents.vue'),
        meta: { roles: ['GiangVien'] }
      },

      // Admin (E00/E01)
      { path: 'admin/accounts', component: () => import('../views/AdminAccounts.vue'), meta: { roles: ['Admin'] } },
      { path: 'admin/requests', component: () => import('../views/PendingRequests.vue'), meta: { roles: ['Admin'] } },

      // Giáo vụ khoa (E02)
      { path: 'giaovu/accounts', component: () => import('../views/AdminAccounts.vue'), meta: { roles: ['GiaoVu'] } },
      { path: 'giaovu/requests', component: () => import('../views/GiaoVuRequests.vue'), meta: { roles: ['GiaoVu'] } }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

const publicPaths = ['/login', '/register', '/forgot-password', '/reset-password', '/activate'];

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const isPublic = publicPaths.includes(to.path);

  if (isPublic) {
    // Đã đăng nhập thì không vào lại trang công khai (trừ khi buộc đổi mật khẩu)
    if (authStore.accessToken && to.path === '/login') {
      return next('/dashboard');
    }
    return next();
  }

  // Cần đăng nhập
  if (!authStore.accessToken) {
    return next('/login');
  }

  // E15.3.6: buộc đổi mật khẩu tạm trước khi vào các trang khác
  if (authStore.user?.buocDoiMatKhau && to.path !== '/change-password') {
    return next('/change-password?forced=1');
  }

  // Phân quyền theo vai trò
  if (to.meta.roles && !to.meta.roles.includes(authStore.user?.vaiTro)) {
    return next('/dashboard');
  }

  return next();
});

export default router;
