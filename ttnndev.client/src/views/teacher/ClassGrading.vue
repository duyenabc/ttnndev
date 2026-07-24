<template>
  <div class="ims-scope">
    <div class="flex items-center gap-3 mb-6">
      <button class="p-2 rounded-lg hover:bg-surface-container" @click="$router.push('/teacher/classes')">
        <span class="material-symbols-outlined text-on-surface-variant">arrow_back</span>
      </button>
      <div class="flex-1">
        <h1 class="font-headline-sm text-2xl font-bold text-on-surface">Chấm điểm</h1>
        <p class="text-body-md text-on-surface-variant">{{ cls?.tenLop }} · {{ cls?.tenKy }}</p>
      </div>
      <button
        class="px-4 py-2.5 rounded-lg font-medium text-body-md border border-outline-variant text-on-surface hover:bg-surface-container flex items-center gap-1"
        @click="exportCsv"
      >
        <span class="material-symbols-outlined text-[18px]">download</span> Xuất bảng điểm
      </button>
    </div>

    <div v-if="locked" class="mb-4 flex items-center gap-2 bg-amber-50 border border-amber-200 text-amber-800 rounded-lg px-4 py-3 text-body-md">
      <span class="material-symbols-outlined text-[20px]">lock</span>
      Sổ điểm đã khóa. Bảng điểm ở chế độ chỉ đọc.
    </div>

    <div class="bg-white rounded-2xl border border-outline-variant overflow-hidden">
      <div class="flex flex-wrap items-center gap-3 p-4 border-b border-outline-variant">
        <h3 class="font-title-md font-semibold text-on-surface flex-1">Bảng điểm</h3>
        <button
          class="px-4 py-2 rounded-lg font-medium text-body-md border border-outline-variant flex items-center gap-1 disabled:opacity-40"
          :disabled="locked"
          @click="showAddCol = true"
        >
          <span class="material-symbols-outlined text-[18px]">add</span> Thêm cột điểm
        </button>
        <button
          class="text-white py-2 px-4 rounded-lg font-medium text-body-md flex items-center gap-1 shadow-sm disabled:opacity-40"
          style="background-color:#005ea3;"
          :disabled="locked || !dirty"
          @click="saveScores"
        >
          <span class="material-symbols-outlined text-[18px]">save</span> Lưu điểm
        </button>
      </div>

      <div class="overflow-x-auto">
        <table class="w-full text-body-md">
          <thead>
            <tr class="text-left text-body-sm text-on-surface-variant border-b border-outline-variant">
              <th class="px-4 py-3 sticky left-0 bg-white">Sinh viên</th>
              <th v-for="c in columns" :key="c.maCotDiem" class="px-4 py-3 whitespace-nowrap">
                <div class="flex items-center gap-1">
                  <span>{{ c.tenCot }}</span>
                  <span class="text-on-surface-variant">/{{ c.diemToiDa }}</span>
                  <button v-if="!locked" class="p-0.5 rounded hover:bg-surface-container text-error" title="Xóa cột" @click="deleteCol(c)">
                    <span class="material-symbols-outlined text-[16px]">close</span>
                  </button>
                </div>
              </th>
              <th class="px-4 py-3">Điểm tổng kết</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading"><td :colspan="columns.length + 2" class="px-4 py-10 text-center text-on-surface-variant">Đang tải...</td></tr>
            <tr v-else-if="!rows.length"><td :colspan="columns.length + 2" class="px-4 py-12 text-center text-on-surface-variant">
              <span class="material-symbols-outlined text-[40px] block mb-2 opacity-50">grade</span>
              Chưa có sinh viên trong lớp.
            </td></tr>
            <tr v-for="r in rows" :key="r.maGhiDanh" class="border-b border-outline-variant/60">
              <td class="px-4 py-3 sticky left-0 bg-white">
                <p class="font-medium text-on-surface">{{ r.hoTen }}</p>
                <p class="text-body-sm text-on-surface-variant">{{ r.maSoSinhVien }}</p>
              </td>
              <td v-for="c in columns" :key="c.maCotDiem" class="px-4 py-3">
                <input
                  v-model.number="r.diem[c.maCotDiem]"
                  type="number"
                  step="0.1"
                  min="0"
                  :max="c.diemToiDa"
                  :disabled="locked"
                  class="w-20 px-2 py-1.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none disabled:bg-surface-container"
                  @input="dirty = true"
                />
              </td>
              <td class="px-4 py-3 font-semibold text-on-surface">{{ rowTotal(r) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div v-if="toast" class="fixed bottom-6 right-6 z-[90] bg-on-surface text-white px-4 py-3 rounded-lg shadow-lg text-body-md">{{ toast }}</div>

    <!-- Thêm cột điểm -->
    <div v-if="showAddCol" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="showAddCol = false">
      <div class="w-full max-w-[420px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-4">Thêm cột điểm</h3>
        <div class="space-y-4">
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Tên cột</label>
            <input v-model="colForm.tenCot" placeholder="VD: Chuyên cần" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Điểm tối đa</label>
            <input v-model.number="colForm.diemToiDa" type="number" step="0.5" min="0" max="10" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          </div>
          <p v-if="colError" class="text-error text-body-sm">{{ colError }}</p>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="showAddCol = false">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md" style="background-color:#005ea3;" @click="addCol">Thêm</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import api from '@/api/api';

  const route = useRoute();
  const maLop = route.params.maLop;

  const cls = ref(null);
  const columns = ref([]);
  const rows = ref([]);
  const locked = ref(false);
  const loading = ref(false);
  const dirty = ref(false);
  const toast = ref('');

  function showToast(m) { toast.value = m; setTimeout(() => (toast.value = ''), 3000); }

  function rowTotal(r) {
    const vals = columns.value.map((c) => r.diem[c.maCotDiem]).filter((v) => v !== null && v !== undefined && v !== '');
    if (!vals.length) return '-';
    return (vals.reduce((a, b) => a + Number(b), 0) / vals.length).toFixed(2);
  }

  async function loadClass() {
    const res = await api.get(`/giangvien/classes/${maLop}`);
    cls.value = res.data;
  }

  async function loadGradebook() {
    loading.value = true;
    try {
      const res = await api.get(`/giangvien/classes/${maLop}/gradebook`);
      columns.value = res.data.cotDiem;
      locked.value = res.data.daKhoaSoDiem;
      rows.value = res.data.dongDiem.map((r) => {
        const diem = {};
        columns.value.forEach((c) => { diem[c.maCotDiem] = r.diem[c.maCotDiem] ?? null; });
        return { ...r, diem };
      });
      dirty.value = false;
    } finally { loading.value = false; }
  }

  const showAddCol = ref(false);
  const colForm = ref({ tenCot: '', diemToiDa: 10 });
  const colError = ref('');
  async function addCol() {
    colError.value = '';
    try {
      await api.post(`/giangvien/classes/${maLop}/score-columns`, colForm.value);
      showAddCol.value = false;
      colForm.value = { tenCot: '', diemToiDa: 10 };
      await loadGradebook();
      showToast('Đã thêm cột điểm');
    } catch (e) { colError.value = e.response?.data?.message || 'Không thể thêm cột điểm'; }
  }

  async function deleteCol(c) {
    await api.delete(`/giangvien/score-columns/${c.maCotDiem}`);
    await loadGradebook();
    showToast('Đã xóa cột điểm');
  }

  async function saveScores() {
    const payload = { diem: [] };
    rows.value.forEach((r) => {
      columns.value.forEach((c) => {
        const v = r.diem[c.maCotDiem];
        payload.diem.push({ maGhiDanh: r.maGhiDanh, maCotDiem: c.maCotDiem, diemSo: v === '' || v === undefined ? null : v });
      });
    });
    try {
      const res = await api.post(`/giangvien/classes/${maLop}/scores`, payload);
      dirty.value = false;
      showToast(res.data.message);
      await loadGradebook();
    } catch (e) { showToast(e.response?.data?.message || 'Lưu điểm thất bại'); }
  }

  async function exportCsv() {
    const res = await api.get(`/giangvien/classes/${maLop}/scores/export`, { responseType: 'blob' });
    const url = URL.createObjectURL(res.data);
    const a = document.createElement('a');
    a.href = url;
    a.download = `bang-diem-${cls.value?.tenLop || maLop}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  }

  onMounted(async () => {
    await loadClass();
    await loadGradebook();
  });
</script>
