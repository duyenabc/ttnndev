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
            <button
              class="w-full text-left px-4 py-2.5 flex items-center gap-3 text-on-surface-variant hover:bg-surface-container transition-colors"
              @click="handleLogout"
            >
              <span class="material-symbols-outlined text-[20px]">logout</span>
              Đăng xuất
            </button>
          </div>
        </div>
      </div>
    </header>

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
  import { computed, ref } from 'vue';
  import { useAuthStore } from '@/stores/auth';
  import { useRoute } from 'vue-router';

  const authStore = useAuthStore();
  const route = useRoute();
  const menuOpen = ref(false);

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
    { path: '/teacher-students', label: 'Danh sách sinh viên', icon: 'groups', roles: ['GiangVien'] }
  ];

  const menuItems = computed(() =>
    allItems.filter((item) => !item.roles || item.roles.includes(userRole.value))
  );

  const isActive = (path) => route.path === path;

  const handleLogout = () => {
    menuOpen.value = false;
    authStore.logout();
  };
</script>
