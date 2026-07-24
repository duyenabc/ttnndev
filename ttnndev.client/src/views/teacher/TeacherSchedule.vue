<template>
  <div class="ims-scope">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="font-headline-sm text-2xl font-bold text-on-surface mb-1">Lịch hướng dẫn</h1>
        <p class="text-body-md text-on-surface-variant">Lên lịch và theo dõi các buổi hướng dẫn với sinh viên.</p>
      </div>
      <button
        class="text-white py-2.5 px-5 rounded-lg font-semibold flex items-center gap-2 shadow-md"
        style="background-color:#005ea3;"
        @click="openCreate"
      >
        <span class="material-symbols-outlined text-[20px]">add</span>
        Tạo lịch hẹn
      </button>
    </div>

    <div class="bg-white rounded-2xl border border-outline-variant overflow-hidden">
      <table class="w-full text-body-md">
        <thead>
          <tr class="text-left text-body-sm text-on-surface-variant border-b border-outline-variant">
            <th class="px-4 py-3">Tiêu đề</th>
            <th class="px-4 py-3">Sinh viên</th>
            <th class="px-4 py-3">Thời gian</th>
            <th class="px-4 py-3">Link</th>
            <th class="px-4 py-3">Trạng thái</th>
            <th class="px-4 py-3 text-right">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loading"><td colspan="6" class="px-4 py-10 text-center text-on-surface-variant">Đang tải...</td></tr>
          <tr v-else-if="!items.length"><td colspan="6" class="px-4 py-12 text-center text-on-surface-variant">
            <span class="material-symbols-outlined text-[40px] block mb-2 opacity-50">event</span>
            Chưa có lịch hẹn nào.
          </td></tr>
          <tr v-for="s in items" :key="s.id" class="border-b border-outline-variant/60">
            <td class="px-4 py-3">
              <p class="font-medium text-on-surface">{{ s.tieuDe }}</p>
              <p class="text-body-sm text-on-surface-variant">{{ s.noiDung }}</p>
            </td>
            <td class="px-4 py-3">
              <p class="text-on-surface">{{ s.tenSinhVien }}</p>
              <p class="text-body-sm text-on-surface-variant">{{ s.maSoSinhVien }}</p>
            </td>
            <td class="px-4 py-3 text-on-surface-variant">{{ formatDate(s.thoiGianHop) }}</td>
            <td class="px-4 py-3">
              <a v-if="s.linkMeeting" :href="s.linkMeeting" target="_blank" class="text-primary hover:underline flex items-center gap-1">
                <span class="material-symbols-outlined text-[16px]">videocam</span> Tham gia
              </a>
              <span v-else class="text-on-surface-variant">-</span>
            </td>
            <td class="px-4 py-3">
              <span class="px-2.5 py-1 rounded-full text-body-sm font-medium bg-blue-100 text-blue-700">{{ s.trangThai }}</span>
            </td>
            <td class="px-4 py-3 text-right">
              <button class="p-1.5 rounded-lg hover:bg-surface-container text-error" title="Xóa" @click="remove(s)">
                <span class="material-symbols-outlined text-[20px]">delete</span>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="toast" class="fixed bottom-6 right-6 z-[90] bg-on-surface text-white px-4 py-3 rounded-lg shadow-lg text-body-md">{{ toast }}</div>

    <!-- Tạo lịch hẹn -->
    <div v-if="showCreate" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="showCreate = false">
      <div class="w-full max-w-[460px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-4">Tạo lịch hẹn</h3>
        <div class="space-y-4">
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Tiêu đề</label>
            <input v-model="form.tieuDe" placeholder="VD: Họp tiến độ tuần 3" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Sinh viên</label>
            <select v-model="form.sinhVienId" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md">
              <option v-for="s in studentOptions" :key="s.maSinhVien" :value="s.maSinhVien">{{ s.hoTen }} ({{ s.maSoSinhVien }})</option>
            </select>
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Thời gian</label>
            <input v-model="form.thoiGianHop" type="datetime-local" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Link cuộc họp</label>
            <input v-model="form.linkMeeting" placeholder="https://meet.google.com/..." class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Nội dung</label>
            <textarea v-model="form.noiDung" rows="2" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none"></textarea>
          </div>
          <p v-if="formError" class="text-error text-body-sm">{{ formError }}</p>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="showCreate = false">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md" style="background-color:#005ea3;" @click="create">Tạo</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import api from '@/api/api';

  const items = ref([]);
  const loading = ref(false);
  const toast = ref('');
  const studentOptions = ref([]);

  function showToast(m) { toast.value = m; setTimeout(() => (toast.value = ''), 3000); }
  function formatDate(d) { return new Date(d).toLocaleString('vi-VN'); }

  async function reload() {
    loading.value = true;
    try {
      const res = await api.get('/giangvien/schedule');
      items.value = res.data;
    } finally { loading.value = false; }
  }

  async function loadStudentOptions() {
    const cls = await api.get('/giangvien/classes');
    const all = [];
    for (const c of cls.data) {
      const res = await api.get(`/giangvien/classes/${c.maLop}/students`);
      res.data.forEach((s) => { if (!all.some((x) => x.maSinhVien === s.maSinhVien)) all.push(s); });
    }
    studentOptions.value = all;
  }

  const showCreate = ref(false);
  const form = ref({});
  const formError = ref('');
  async function openCreate() {
    formError.value = '';
    if (!studentOptions.value.length) await loadStudentOptions();
    form.value = { tieuDe: '', sinhVienId: studentOptions.value[0]?.maSinhVien ?? null, thoiGianHop: '', linkMeeting: '', noiDung: '' };
    showCreate.value = true;
  }
  async function create() {
    formError.value = '';
    if (!form.value.tieuDe) { formError.value = 'Vui lòng nhập tiêu đề'; return; }
    if (!form.value.sinhVienId) { formError.value = 'Vui lòng chọn sinh viên'; return; }
    if (!form.value.thoiGianHop) { formError.value = 'Vui lòng chọn thời gian'; return; }
    try {
      const res = await api.post('/giangvien/schedule', form.value);
      showCreate.value = false;
      showToast(res.data.message);
      await reload();
    } catch (e) { formError.value = e.response?.data?.message || 'Không thể tạo lịch hẹn'; }
  }

  async function remove(s) {
    await api.delete(`/giangvien/schedule/${s.id}`);
    showToast('Đã xóa lịch hẹn');
    await reload();
  }

  onMounted(reload);
</script>
