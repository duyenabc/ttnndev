<template>
  <div class="ims-scope">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="font-headline-sm text-2xl font-bold text-on-surface mb-1">Yêu cầu của tôi</h1>
        <p class="text-body-md text-on-surface-variant">Gửi yêu cầu cấp tài khoản đến quản trị viên và theo dõi trạng thái.</p>
      </div>
      <button class="text-white py-2.5 px-5 rounded-lg font-semibold flex items-center gap-2 shadow-md" style="background-color:#005ea3;" @click="openCreate">
        <span class="material-symbols-outlined text-[20px]">add</span>
        Yêu cầu cấp tài khoản
      </button>
    </div>

    <div class="bg-white rounded-2xl border border-outline-variant overflow-hidden">
      <div class="flex items-center gap-3 p-4 border-b border-outline-variant">
        <select v-model="status" class="px-3 py-2.5 border border-outline-variant rounded-lg text-body-md" @change="reload">
          <option value="all">Tất cả</option>
          <option value="ChoXuLy">Chờ xử lý</option>
          <option value="DaDuyet">Đã duyệt</option>
          <option value="TuChoi">Đã từ chối</option>
        </select>
      </div>
      <table class="w-full text-body-md">
        <thead>
          <tr class="text-left text-body-sm text-on-surface-variant border-b border-outline-variant">
            <th class="px-4 py-3">Loại</th>
            <th class="px-4 py-3">Đối tượng</th>
            <th class="px-4 py-3">Thời gian</th>
            <th class="px-4 py-3">Trạng thái</th>
            <th class="px-4 py-3">Ghi chú</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loading"><td colspan="5" class="px-4 py-10 text-center text-on-surface-variant">Đang tải...</td></tr>
          <tr v-else-if="!items.length"><td colspan="5" class="px-4 py-12 text-center text-on-surface-variant">
            <span class="material-symbols-outlined text-[40px] block mb-2 opacity-50">assignment</span>
            Bạn chưa gửi yêu cầu nào
          </td></tr>
          <tr v-for="r in items" :key="r.maYeuCau" class="border-b border-outline-variant/60">
            <td class="px-4 py-3">{{ loaiLabels[r.loaiYeuCau] || r.loaiYeuCau }}</td>
            <td class="px-4 py-3">
              <p class="font-medium text-on-surface">{{ r.tenDoiTuong }}</p>
              <p class="text-body-sm text-on-surface-variant">{{ r.maDinhDanhDoiTuong }}</p>
            </td>
            <td class="px-4 py-3 text-on-surface-variant">{{ formatDate(r.ngayTao) }}</td>
            <td class="px-4 py-3"><span class="px-2.5 py-1 rounded-full text-body-sm font-medium" :class="statusCls[r.trangThai]">{{ statusLabels[r.trangThai] }}</span></td>
            <td class="px-4 py-3 text-on-surface-variant italic">{{ r.lyDoTuChoi || '—' }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="toast" class="fixed bottom-6 right-6 z-[90] bg-on-surface text-white px-4 py-3 rounded-lg shadow-lg text-body-md">{{ toast }}</div>

    <div v-if="showCreate" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="showCreate = false">
      <div class="w-full max-w-[440px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-4">Yêu cầu cấp tài khoản</h3>
        <div class="space-y-4">
          <input v-model="form.maDinhDanh" placeholder="Mã định danh" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          <input v-model="form.hoTen" placeholder="Họ tên" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          <input v-model="form.email" placeholder="Email" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          <input v-model="form.soDienThoai" placeholder="Số điện thoại" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          <select v-model="form.vaiTro" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md">
            <option value="SinhVien">Sinh viên</option>
            <option value="GiangVien">Giảng viên</option>
          </select>
          <p v-if="formError" class="text-error text-body-sm">{{ formError }}</p>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="showCreate = false">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md" style="background-color:#005ea3;" @click="create">Gửi yêu cầu</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import api from '@/api/api';

  const loaiLabels = { CapTaiKhoan: 'Cấp tài khoản', KhoaTaiKhoan: 'Khóa tài khoản', MoKhoaTaiKhoan: 'Mở khóa tài khoản' };
  const statusLabels = { ChoXuLy: 'Chờ xử lý', DaDuyet: 'Đã duyệt', TuChoi: 'Đã từ chối' };
  const statusCls = { ChoXuLy: 'bg-amber-100 text-amber-700', DaDuyet: 'bg-green-100 text-green-700', TuChoi: 'bg-red-100 text-red-700' };

  const items = ref([]);
  const loading = ref(false);
  const status = ref('all');
  const toast = ref('');

  function showToast(m) { toast.value = m; setTimeout(() => (toast.value = ''), 3000); }
  function formatDate(d) { return new Date(d).toLocaleString('vi-VN'); }

  async function reload() {
    loading.value = true;
    try {
      const res = await api.get('/giaovu/requests', { params: { status: status.value } });
      items.value = res.data;
    } finally { loading.value = false; }
  }

  const showCreate = ref(false);
  const form = ref({});
  const formError = ref('');
  function openCreate() {
    form.value = { maDinhDanh: '', hoTen: '', email: '', soDienThoai: '', vaiTro: 'SinhVien' };
    formError.value = ''; showCreate.value = true;
  }
  async function create() {
    formError.value = '';
    try {
      const res = await api.post('/giaovu/requests', {
        loaiYeuCau: 'CapTaiKhoan',
        nguoiDungMoi: form.value
      });
      showToast(res.data.message); showCreate.value = false; reload();
    } catch (e) { formError.value = e.response?.data?.message || 'Không thể gửi yêu cầu'; }
  }

  onMounted(reload);
</script>
