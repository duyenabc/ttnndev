<template>
  <div class="ims-scope space-y-stack-gap">
    <!-- Header -->
    <div>
      <p class="text-body-md text-on-surface-variant">{{ today }}</p>
      <h1 class="font-display-lg text-[28px] text-on-surface font-bold">
        Xin chào, {{ authStore.user?.hoTen || 'bạn' }}
      </h1>
      <p class="text-body-md text-on-surface-variant mt-1">
        Chào mừng đến với Hệ thống Quản lý Thực tập ({{ roleLabel }}).
      </p>
    </div>

    <!-- Stat cards -->
    <section class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-component-gap">
      <div
        v-for="stat in stats"
        :key="stat.label"
        class="bg-white p-6 rounded-xl border border-outline-variant hover:shadow-md transition-all"
      >
        <div class="flex items-center justify-between mb-2">
          <h3 class="text-on-surface-variant text-body-sm font-medium">{{ stat.label }}</h3>
          <span class="material-symbols-outlined text-primary text-[22px]">{{ stat.icon }}</span>
        </div>
        <p class="font-display-lg text-2xl md:text-3xl text-on-surface">
          {{ stat.value }}<span class="text-on-surface-variant text-[16px] font-normal"> {{ stat.unit }}</span>
        </p>
      </div>
    </section>

    <!-- Quick access -->
    <section class="space-y-4">
      <h2 class="font-title-sm text-title-sm text-on-surface">Truy cập nhanh</h2>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-component-gap">
        <router-link
          v-for="shortcut in shortcuts"
          :key="shortcut.path"
          :to="shortcut.path"
          class="bg-white p-5 rounded-xl border border-outline-variant hover:shadow-md hover:border-primary transition-all flex items-center gap-4 group"
        >
          <div class="w-12 h-12 rounded-full bg-primary-container/40 flex items-center justify-center text-primary group-hover:bg-primary group-hover:text-on-primary transition-colors">
            <span class="material-symbols-outlined text-[24px]">{{ shortcut.icon }}</span>
          </div>
          <div>
            <p class="font-semibold text-on-surface">{{ shortcut.label }}</p>
            <p class="text-body-sm text-on-surface-variant">{{ shortcut.desc }}</p>
          </div>
          <span class="material-symbols-outlined text-outline ml-auto group-hover:text-primary">chevron_right</span>
        </router-link>
      </div>
    </section>
  </div>
</template>

<script setup>
  import { computed } from 'vue';
  import { useAuthStore } from '@/stores/auth';

  const authStore = useAuthStore();
  const userRole = computed(() => authStore.user?.vaiTro);

  const roleLabels = {
    SinhVien: 'Sinh viên',
    GiangVien: 'Giảng viên',
    GiaoVu: 'Giáo vụ khoa',
    Admin: 'Quản trị viên'
  };
  const roleLabel = computed(() => roleLabels[userRole.value] || 'Người dùng');

  const today = computed(() =>
    new Date().toLocaleDateString('vi-VN', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    })
  );

  const statsByRole = {
    SinhVien: [
      { label: 'Tuần thực tập', value: '4', unit: '/ 15', icon: 'calendar_month' },
      { label: 'Nhật ký đã nộp', value: '3', unit: 'bài', icon: 'menu_book' },
      { label: 'Điểm trung bình', value: '—', unit: '', icon: 'grade' }
    ],
    GiangVien: [
      { label: 'Sinh viên hướng dẫn', value: '—', unit: 'SV', icon: 'groups' },
      { label: 'Bài chờ chấm', value: '—', unit: 'bài', icon: 'assignment' },
      { label: 'Lớp phụ trách', value: '—', unit: 'lớp', icon: 'class' }
    ]
  };
  const defaultStats = [
    { label: 'Tổng quan', value: '—', unit: '', icon: 'insights' }
  ];
  const stats = computed(() => statsByRole[userRole.value] || defaultStats);

  const shortcutsByRole = {
    SinhVien: [
      { path: '/diaries', label: 'Nhật ký thực tập', desc: 'Viết & xem nhật ký tuần', icon: 'menu_book' }
    ],
    GiangVien: [
      { path: '/teacher/classes', label: 'Lớp của tôi', desc: 'Quản lý lớp, sinh viên & điểm', icon: 'class' },
      { path: '/teacher/schedule', label: 'Lịch hướng dẫn', desc: 'Lên lịch buổi hướng dẫn', icon: 'event' }
    ]
  };
  const shortcuts = computed(() => shortcutsByRole[userRole.value] || []);
</script>
