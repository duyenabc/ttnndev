<template>
  <div class="ims-scope">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="font-headline-sm text-2xl font-bold text-on-surface mb-1">Lớp của tôi</h1>
        <p class="text-body-md text-on-surface-variant">Quản lý các lớp hướng dẫn thực tập của bạn.</p>
      </div>
      <button
        class="text-white py-2.5 px-5 rounded-lg font-semibold flex items-center gap-2 shadow-md"
        style="background-color:#005ea3;"
        @click="openCreate"
      >
        <span class="material-symbols-outlined text-[20px]">add</span>
        Tạo lớp mới
      </button>
    </div>

    <div v-if="loading" class="bg-white rounded-2xl border border-outline-variant p-12 text-center text-on-surface-variant">
      Đang tải...
    </div>

    <div
      v-else-if="!classes.length"
      class="bg-white rounded-2xl border border-outline-variant p-12 text-center text-on-surface-variant"
    >
      <span class="material-symbols-outlined text-[40px] block mb-2 opacity-50">class</span>
      Bạn chưa có lớp nào. Nhấn "Tạo lớp mới" để bắt đầu.
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
      <div
        v-for="c in classes"
        :key="c.maLop"
        class="bg-white rounded-2xl border border-outline-variant p-5 flex flex-col gap-4"
      >
        <div class="flex items-start justify-between">
          <div>
            <h3 class="font-title-lg text-title-lg font-semibold text-on-surface">{{ c.tenLop }}</h3>
            <p class="text-body-sm text-on-surface-variant mt-0.5">{{ c.tenKy }}</p>
          </div>
          <span
            class="px-2.5 py-1 rounded-full text-body-sm font-medium"
            :class="c.ghiDanhMo ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-600'"
          >
            {{ c.ghiDanhMo ? 'Đang mở ghi danh' : 'Đã đóng ghi danh' }}
          </span>
        </div>

        <div class="flex items-center gap-4 text-body-sm text-on-surface-variant">
          <span class="flex items-center gap-1">
            <span class="material-symbols-outlined text-[18px]">groups</span>
            {{ c.soSinhVien }} sinh viên
          </span>
          <span class="flex items-center gap-1">
            <span class="material-symbols-outlined text-[18px]">key</span>
            {{ c.maThamGia }}
          </span>
        </div>

        <div class="flex gap-2 mt-auto pt-2">
          <button
            class="flex-1 px-3 py-2 rounded-lg font-medium text-body-md border border-outline-variant text-on-surface hover:bg-surface-container flex items-center justify-center gap-1"
            @click="goStudents(c.maLop)"
          >
            <span class="material-symbols-outlined text-[18px]">groups</span>
            Sinh viên
          </button>
          <button
            class="flex-1 px-3 py-2 rounded-lg font-medium text-body-md border border-outline-variant text-on-surface hover:bg-surface-container flex items-center justify-center gap-1"
            @click="goGrading(c.maLop)"
          >
            <span class="material-symbols-outlined text-[18px]">grade</span>
            Chấm điểm
          </button>
        </div>
      </div>
    </div>

    <!-- Toast -->
    <div v-if="toast" class="fixed bottom-6 right-6 z-[90] bg-on-surface text-white px-4 py-3 rounded-lg shadow-lg text-body-md">
      {{ toast }}
    </div>

    <!-- Tạo lớp -->
    <div v-if="showCreate" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="showCreate = false">
      <div class="w-full max-w-[460px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-4">Tạo lớp mới</h3>
        <div class="space-y-4">
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Kỳ thực tập</label>
            <select v-model="form.maKy" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md">
              <option v-for="k in cycles" :key="k.maKy" :value="k.maKy">{{ k.tenKy }}</option>
            </select>
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Mã học phần</label>
            <input
              v-model="form.maHocPhan"
              placeholder="VD: MIS2012"
              class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none"
            />
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Số thứ tự lớp</label>
            <input
              v-model.number="form.soThuTuLop"
              type="number"
              min="1"
              placeholder="VD: 1"
              class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none"
            />
          </div>
          <div class="bg-surface-container rounded-lg px-4 py-3">
            <p class="text-body-sm text-on-surface-variant">Tên lớp sẽ được tạo</p>
            <p class="font-title-md font-semibold text-on-surface">{{ previewName || '—' }}</p>
          </div>
          <p v-if="formError" class="text-error text-body-sm">{{ formError }}</p>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="showCreate = false">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md" style="background-color:#005ea3;" @click="create">Tạo lớp</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  import api from '@/api/api';

  const router = useRouter();
  const classes = ref([]);
  const cycles = ref([]);
  const loading = ref(false);
  const toast = ref('');

  function showToast(m) { toast.value = m; setTimeout(() => (toast.value = ''), 3000); }

  async function reload() {
    loading.value = true;
    try {
      const res = await api.get('/giangvien/classes');
      classes.value = res.data;
    } finally { loading.value = false; }
  }

  const showCreate = ref(false);
  const form = ref({ maKy: null, maHocPhan: '', soThuTuLop: null });
  const formError = ref('');

  const previewName = computed(() => {
    const ky = cycles.value.find((k) => k.maKy === form.value.maKy);
    if (!ky || !form.value.soThuTuLop) return '';
    const mhp = (form.value.maHocPhan || 'TT').trim();
    return `${mhp}_${ky.namHoc}_${form.value.soThuTuLop}`;
  });

  async function openCreate() {
    formError.value = '';
    if (!cycles.value.length) {
      const res = await api.get('/giangvien/cycles');
      cycles.value = res.data;
    }
    form.value = { maKy: cycles.value[0]?.maKy ?? null, maHocPhan: '', soThuTuLop: null };
    showCreate.value = true;
  }

  async function create() {
    formError.value = '';
    if (!form.value.soThuTuLop || form.value.soThuTuLop <= 0) {
      formError.value = 'Vui lòng nhập số lớp';
      return;
    }
    try {
      const res = await api.post('/giangvien/classes', form.value);
      showToast(res.data.message);
      showCreate.value = false;
      await reload();
      goStudents(res.data.maLop);
    } catch (e) {
      formError.value = e.response?.data?.message || 'Tạo lớp thất bại';
    }
  }

  function goStudents(maLop) { router.push(`/teacher/classes/${maLop}/students`); }
  function goGrading(maLop) { router.push(`/teacher/classes/${maLop}/grading`); }

  onMounted(reload);
</script>
