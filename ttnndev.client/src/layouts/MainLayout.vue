<template>
  <div class="ims-scope min-h-screen bg-surface text-on-surface">
    <!-- Top App Bar -->
    <header class="fixed top-0 left-0 w-full z-[60] flex justify-between items-center px-container-padding h-16 bg-[#005EA3] border-b border-white/10 font-body-md">
      <div class="flex items-center gap-8">
        <span class="font-display-lg text-display-lg font-black text-white">IMS</span>
        <span class="hidden md:block text-white/80 text-title-sm">Hệ thống Quản lý Thực tập</span>
      </div>

      <div class="flex items-center gap-4">
        <button class="material-symbols-outlined text-white p-2 rounded-full hover:bg-white/10 transition-colors" aria-label="Thông báo">notifications</button>
        <div class="relative">
          <button
            class="flex items-center gap-2 cursor-pointer p-1 pr-3 rounded-full hover:bg-white/10 transition-colors"
            @click="menuOpen = !menuOpen"
          >
            <span class="material-symbols-outlined text-white text-[32px]" style="font-variation-settings:'FILL' 1;">account_circle</span>
            <span class="hidden sm:block text-white text-body-md">{{ authStore.user?.hoTen || 'Người dùng' }}</span>
            <span class="material-symbols-outlined text-white text-[20px]">expand_more</span>
          </button>
          <div
            v-if="menuOpen"
            class="absolute right-0 mt-2 w-56 bg-white rounded-xl shadow-lg border border-outline-variant py-2 z-50"
          >
            <div class="px-4 py-2 border-b border-outline-variant">
              <p class="font-semibold text-on-surface">{{ authStore.user?.hoTen || 'Người dùng' }}</p>
              <p class="text-body-sm text-on-surface-variant">{{ roleLabel }}</p>
            </div>
            <router-link
              to="/profile"
              class="w-full text-left px-4 py-2.5 flex items-center gap-3 text-on-surface-variant hover:bg-surface-container transition-colors"
              @click="menuOpen = false"
            >
              <span class="material-symbols-outlined text-[20px]">person</span>
              Hồ sơ cá nhân
            </router-link>
            <router-link
              to="/change-password"
              class="w-full text-left px-4 py-2.5 flex items-center gap-3 text-on-surface-variant hover:bg-surface-container transition-colors"
              @click="menuOpen = false"
            >
              <span class="material-symbols-outlined text-[20px]">password</span>
              Đổi mật khẩu
            </router-link>
            <button
              class="w-full text-left px-4 py-2.5 flex items-center gap-3 text-on-surface-variant hover:bg-surface-container transition-colors border-t border-outline-variant"
              @click="askLogout"
            >
              <span class="material-symbols-outlined text-[20px]">logout</span>
              Đăng xuất
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- E15.2.2 Hộp thoại xác nhận đăng xuất -->
    <div
      v-if="showLogoutConfirm"
      class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4"
      @click.self="showLogoutConfirm = false"
    >
      <div class="w-full max-w-[400px] bg-white rounded-2xl shadow-xl p-6">
        <div class="flex items-center gap-3 mb-4">
          <span class="material-symbols-outlined text-primary text-[28px]">logout</span>
          <h3 class="font-title-lg text-title-lg font-semibold text-on-surface">Đăng xuất</h3>
        </div>
        <p class="text-body-md text-on-surface-variant mb-6">Bạn có chắc muốn đăng xuất?</p>
        <div class="flex justify-end gap-3">
          <button
            class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container transition-colors"
            @click="showLogoutConfirm = false"
          >
            Hủy
          </button>
          <button
            class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md transition-colors"
            style="background-color:#005ea3;"
            @click="confirmLogout"
          >
            Đăng xuất
          </button>
        </div>
      </div>
    </div>

    <!-- E15.7 Cảnh báo sắp hết phiên do không hoạt động -->
    <div
      v-if="showTimeoutWarning"
      class="fixed inset-0 z-[85] flex items-center justify-center bg-black/40 px-4"
    >
      <div class="w-full max-w-[420px] bg-white rounded-2xl shadow-xl p-6">
        <div class="flex items-center gap-3 mb-4">
          <span class="material-symbols-outlined text-amber-500 text-[28px]">schedule</span>
          <h3 class="font-title-lg text-title-lg font-semibold text-on-surface">Sắp hết phiên</h3>
        </div>
        <p class="text-body-md text-on-surface-variant mb-6">Phiên của bạn sắp hết hạn. Bạn có muốn tiếp tục không?</p>
        <div class="flex justify-end gap-3">
          <button
            class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container transition-colors"
            @click="endSessionNow"
          >
            Đăng xuất
          </button>
          <button
            class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md transition-colors"
            style="background-color:#005ea3;"
            @click="continueSession"
          >
            Tiếp tục
          </button>
        </div>
      </div>
    </div>

    <!-- Sidebar -->
    <aside class="fixed left-0 top-16 h-[calc(100vh-64px)] w-sidebar-width z-40 flex flex-col py-4 overflow-y-auto border-r border-outline-variant bg-white">
      <nav class="flex flex-col flex-1">
        <router-link
          v-for="item in menuItems"
          :key="item.path"
          :to="item.path"
          class="px-6 py-3 flex items-center gap-3 transition-colors active:scale-95 duration-200"
          :class="isActive(item.path)
            ? 'bg-primary-container text-on-primary-container border-l-4 border-primary font-bold'
            : 'text-on-surface-variant hover:bg-surface-container border-l-4 border-transparent'"
        >
          <span class="material-symbols-outlined text-[22px]">{{ item.icon }}</span>
          <span>{{ item.label }}</span>
        </router-link>
      </nav>

      <div class="px-6 pt-4 border-t border-outline-variant text-body-sm text-on-surface-variant">
        © {{ new Date().getFullYear() }} IMS
      </div>
    </aside>

    <!-- Main content -->
    <main class="ml-sidebar-width pt-16 min-h-screen">
      <div class="p-container-padding max-w-[1440px] mx-auto">
        <router-view />
      </div>
    </main>
  </div>
</template>

<script setup>
  import { computed, ref, onMounted, onUnmounted } from 'vue';
  import { useAuthStore } from '@/stores/auth';
  import { useRoute, useRouter } from 'vue-router';

  const authStore = useAuthStore();
  const route = useRoute();
  const router = useRouter();
  const menuOpen = ref(false);
  const showLogoutConfirm = ref(false);
  const showTimeoutWarning = ref(false);

  // E15.7: 60 phút không hoạt động → cảnh báo; sau thêm 5 phút → tự đăng xuất
  const INACTIVITY_MS = 60 * 60 * 1000;
  const GRACE_MS = 5 * 60 * 1000;
  let warnTimer = null;
  let logoutTimer = null;

  function clearTimers() {
    clearTimeout(warnTimer);
    clearTimeout(logoutTimer);
  }
  function scheduleTimers() {
    clearTimers();
    warnTimer = setTimeout(() => {
      showTimeoutWarning.value = true;
      logoutTimer = setTimeout(() => {
        showTimeoutWarning.value = false;
        authStore.logout({ redirect: false });
        router.replace('/login?expired=1');
      }, GRACE_MS);
    }, INACTIVITY_MS);
  }
  function onActivity() {
    if (showTimeoutWarning.value) return; // đang chờ người dùng quyết định
    scheduleTimers();
  }
  function continueSession() {
    showTimeoutWarning.value = false;
    scheduleTimers();
  }
  function endSessionNow() {
    showTimeoutWarning.value = false;
    clearTimers();
    authStore.logout();
  }

  const userRole = computed(() => authStore.user?.vaiTro);

  const roleLabels = {
    SinhVien: 'Sinh viên',
    GiangVien: 'Giảng viên',
    GiaoVu: 'Giáo vụ khoa',
    Admin: 'Quản trị viên'
  };
  const roleLabel = computed(() => roleLabels[userRole.value] || 'Người dùng');

  const allItems = [
    { path: '/dashboard', label: 'Tổng quan', icon: 'dashboard', roles: null },
    { path: '/diaries', label: 'Nhật ký thực tập', icon: 'menu_book', roles: ['SinhVien'] },
    { path: '/scores', label: 'Quản lý điểm số', icon: 'grade', roles: ['GiangVien'] },
    { path: '/teacher-students', label: 'Danh sách sinh viên', icon: 'groups', roles: ['GiangVien'] },
    { path: '/admin/accounts', label: 'Quản lý tài khoản', icon: 'manage_accounts', roles: ['Admin'] },
    { path: '/admin/requests', label: 'Yêu cầu chờ xử lý', icon: 'inbox', roles: ['Admin'] },
    { path: '/giaovu/accounts', label: 'Quản lý tài khoản', icon: 'manage_accounts', roles: ['GiaoVu'] },
    { path: '/giaovu/requests', label: 'Yêu cầu của tôi', icon: 'assignment', roles: ['GiaoVu'] }
  ];

  const menuItems = computed(() =>
    allItems.filter((item) => !item.roles || item.roles.includes(userRole.value))
  );

  const isActive = (path) => route.path === path;

  const askLogout = () => {
    menuOpen.value = false;
    showLogoutConfirm.value = true;
  };

  const confirmLogout = () => {
    showLogoutConfirm.value = false;
    authStore.logout();
  };

  // E15.2.6 đăng xuất đồng bộ giữa các tab + E15.7 hết phiên
  const onStorage = (e) => {
    if (e.key === 'auth-event' && e.newValue?.startsWith('logout')) {
      authStore.clearLocal();
      router.replace('/login');
    }
  };
  const onSessionExpired = () => {
    router.replace('/login?expired=1');
  };
  const activityEvents = ['mousedown', 'keydown', 'scroll', 'touchstart'];
  onMounted(() => {
    window.addEventListener('storage', onStorage);
    window.addEventListener('auth:session-expired', onSessionExpired);
    activityEvents.forEach((ev) => window.addEventListener(ev, onActivity, { passive: true }));
    scheduleTimers();
  });
  onUnmounted(() => {
    window.removeEventListener('storage', onStorage);
    window.removeEventListener('auth:session-expired', onSessionExpired);
    activityEvents.forEach((ev) => window.removeEventListener(ev, onActivity));
    clearTimers();
  });
</script>
