<template>
  <div class="ims-scope">
    <div class="mb-6">
      <h1 class="font-headline-sm text-2xl font-bold text-on-surface mb-1">
        Yêu cầu chờ xử lý
        <span v-if="pendingCount" class="ml-2 align-middle inline-flex items-center justify-center min-w-[24px] h-6 px-2 rounded-full bg-red-500 text-white text-body-sm font-semibold">{{ pendingCount }}</span>
      </h1>
      <p class="text-body-md text-on-surface-variant">Yêu cầu cấp / khóa / mở khóa tài khoản từ giáo vụ khoa.</p>
    </div>

    <div class="bg-white rounded-2xl border border-outline-variant overflow-hidden">
      <div class="flex flex-wrap items-center gap-3 p-4 border-b border-outline-variant">
        <div class="relative flex-1 min-w-[220px]">
          <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-outline text-[20px]">search</span>
          <input v-model="search" placeholder="Tìm theo tên, mã, email..." class="w-full pl-10 pr-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" @keyup.enter="reload" />
        </div>
        <select v-model="loai" class="px-3 py-2.5 border border-outline-variant rounded-lg text-body-md" @change="reload">
          <option value="">Tất cả loại</option>
          <option value="CapTaiKhoan">Cấp tài khoản</option>
          <option value="KhoaTaiKhoan">Khóa tài khoản</option>
          <option value="MoKhoaTaiKhoan">Mở khóa tài khoản</option>
        </select>
        <select v-model="status" class="px-3 py-2.5 border border-outline-variant rounded-lg text-body-md" @change="reload">
          <option value="ChoXuLy">Chờ xử lý</option>
          <option value="DaDuyet">Đã duyệt</option>
          <option value="TuChoi">Đã từ chối</option>
          <option value="all">Tất cả</option>
        </select>
      </div>

      <table class="w-full text-body-md">
        <thead>
          <tr class="text-left text-body-sm text-on-surface-variant border-b border-outline-variant">
            <th class="px-4 py-3">Loại</th>
            <th class="px-4 py-3">Đối tượng</th>
            <th class="px-4 py-3">Người yêu cầu</th>
            <th class="px-4 py-3">Thời gian</th>
            <th class="px-4 py-3">Trạng thái</th>
            <th class="px-4 py-3 text-right">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loading"><td colspan="6" class="px-4 py-10 text-center text-on-surface-variant">Đang tải...</td></tr>
          <tr v-else-if="!items.length"><td colspan="6" class="px-4 py-12 text-center text-on-surface-variant">
            <span class="material-symbols-outlined text-[40px] block mb-2 opacity-50">inbox</span>
            Không có yêu cầu nào
          </td></tr>
          <tr v-for="r in items" :key="r.maYeuCau" class="border-b border-outline-variant/60 hover:bg-surface-container/40">
            <td class="px-4 py-3">{{ loaiLabels[r.loaiYeuCau] || r.loaiYeuCau }}</td>
            <td class="px-4 py-3">
              <p class="font-medium text-on-surface">{{ r.tenDoiTuong }}</p>
              <p class="text-body-sm text-on-surface-variant">{{ r.maDinhDanhDoiTuong }} · {{ r.emailDoiTuong }}</p>
            </td>
            <td class="px-4 py-3 text-on-surface-variant">{{ r.tenNguoiYeuCau }}</td>
            <td class="px-4 py-3 text-on-surface-variant">{{ formatDate(r.ngayTao) }}</td>
            <td class="px-4 py-3">
              <span class="px-2.5 py-1 rounded-full text-body-sm font-medium" :class="statusCls[r.trangThai]">{{ statusLabels[r.trangThai] }}</span>
            </td>
            <td class="px-4 py-3 text-right">
              <div v-if="r.trangThai === 'ChoXuLy'" class="inline-flex items-center gap-2">
                <button class="px-3 py-1.5 rounded-lg bg-green-600 text-white text-body-sm font-medium hover:bg-green-700" @click="approve(r)">Duyệt</button>
                <button class="px-3 py-1.5 rounded-lg border border-red-300 text-red-600 text-body-sm font-medium hover:bg-red-50" @click="askReject(r)">Từ chối</button>
              </div>
              <span v-else-if="r.lyDoTuChoi" class="text-body-sm text-on-surface-variant italic">{{ r.lyDoTuChoi }}</span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="toast" class="fixed bottom-6 right-6 z-[90] bg-on-surface text-white px-4 py-3 rounded-lg shadow-lg text-body-md">{{ toast }}</div>

    <!-- Reject reason modal -->
    <div v-if="rejectTarget" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="rejectTarget = null">
      <div class="w-full max-w-[440px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-4">Từ chối yêu cầu</h3>
        <textarea v-model="rejectReason" maxlength="200" rows="3" placeholder="Lý do từ chối (tối đa 200 ký tự)" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none"></textarea>
        <p v-if="rejectError" class="text-error text-body-sm mt-1">{{ rejectError }}</p>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="rejectTarget = null">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md" style="background-color:#005ea3;" @click="doReject">Từ chối</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue';
  import api from '@/api/api';

  const loaiLabels = { CapTaiKhoan: 'Cấp tài khoản', KhoaTaiKhoan: 'Khóa tài khoản', MoKhoaTaiKhoan: 'Mở khóa tài khoản' };
  const statusLabels = { ChoXuLy: 'Chờ xử lý', DaDuyet: 'Đã duyệt', TuChoi: 'Đã từ chối' };
  const statusCls = { ChoXuLy: 'bg-amber-100 text-amber-700', DaDuyet: 'bg-green-100 text-green-700', TuChoi: 'bg-red-100 text-red-700' };

  const items = ref([]);
  const loading = ref(false);
  const search = ref('');
  const loai = ref('');
  const status = ref('ChoXuLy');
  const toast = ref('');

  const pendingCount = computed(() => items.value.filter((r) => r.trangThai === 'ChoXuLy').length);

  function showToast(m) { toast.value = m; setTimeout(() => (toast.value = ''), 3000); }
  function formatDate(d) { return new Date(d).toLocaleString('vi-VN'); }

  async function reload() {
    loading.value = true;
    try {
      const res = await api.get('/account/requests', { params: { status: status.value, loai: loai.value, search: search.value } });
      items.value = res.data;
    } finally { loading.value = false; }
  }

  async function approve(r) {
    const res = await api.post(`/account/requests/${r.maYeuCau}/approve`);
    showToast(res.data.message); reload();
  }

  const rejectTarget = ref(null);
  const rejectReason = ref('');
  const rejectError = ref('');
  function askReject(r) { rejectTarget.value = r; rejectReason.value = ''; rejectError.value = ''; }
  async function doReject() {
    rejectError.value = '';
    if (!rejectReason.value.trim()) { rejectError.value = 'Vui lòng nhập lý do'; return; }
    try {
      const res = await api.post(`/account/requests/${rejectTarget.value.maYeuCau}/reject`, { lyDo: rejectReason.value });
      showToast(res.data.message); rejectTarget.value = null; reload();
    } catch (e) { rejectError.value = e.response?.data?.message || 'Không thể từ chối'; }
  }

  onMounted(reload);
</script>
