import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import MainLayout from '../layouts/MainLayout.vue';

// CHỈ KHAI BÁO MẢNG ROUTES 1 LẦN
const routes = [
  { path: '/login', component: () => import('../views/Login.vue') },
  { path: '/register', name: 'Register', component: () => import('../views/Register.vue') },
  {
    path: '/',
    component: MainLayout,
    redirect: '/dashboard',
    meta: { requiresAuth: true },
    children: [
      { path: 'dashboard', component: () => import('../views/Dashboard.vue') },

      // Dành cho Sinh Viên
      {
        path: 'diaries',
        component: () => import('../views/Diaries.vue'),
        meta: { roles: ['SinhVien'] }
      },

      // Dành cho Giảng Viên
      {
        path: 'scores',
        component: () => import('../views/Scores.vue'),
        meta: { roles: ['GiangVien'] }
      },
      {
        path: 'teacher-students',
        name: 'TeacherStudents',
        component: () => import('../views/TeacherStudents.vue'),
        meta: { roles: ['GiangVien'] }
      }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// THIẾT LẬP NAVIGATION GUARD BẢO MẬT
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();

  // 1. Cho phép vào tự do trang login / register
  if (to.path === '/login' || to.path === '/register') {
    if (authStore.token) {
      next('/dashboard'); // Đã đăng nhập thì không vào lại login nữa
    } else {
      next();
    }
  }
  else {
    // 2. Kiểm tra nếu chưa đăng nhập mà đòi vào trang có RequiresAuth
    if (to.meta.requiresAuth && !authStore.token) {
      next('/login');
    }
    else {
      // 3. KIỂM TRA PHÂN QUYỀN (ROLES) - MỚI ĐƯỢC THÊM
      // Nếu route có quy định roles, và vai trò của user hiện tại không nằm trong danh sách đó
      if (to.meta.roles && !to.meta.roles.includes(authStore.user?.vaiTro)) {
        // Không có quyền truy cập, đẩy về màn hình tổng quan
        next('/dashboard');
      } else {
        // Hợp lệ, cho phép đi tiếp
        next();
      }
    }
  }
});

export default router;
