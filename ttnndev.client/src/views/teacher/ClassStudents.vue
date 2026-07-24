<template>
  <div class="ims-scope">
    <!-- Header -->
    <div class="flex items-center gap-3 mb-6">
      <button class="p-2 rounded-lg hover:bg-surface-container" @click="$router.push('/teacher/classes')">
        <span class="material-symbols-outlined text-on-surface-variant">arrow_back</span>
      </button>
      <div class="flex-1">
        <h1 class="font-headline-sm text-2xl font-bold text-on-surface">{{ cls?.tenLop || 'Lớp' }}</h1>
        <p class="text-body-md text-on-surface-variant">{{ cls?.tenKy }}</p>
      </div>
      <button
        class="px-4 py-2.5 rounded-lg font-medium text-body-md border border-outline-variant text-on-surface hover:bg-surface-container flex items-center gap-1"
        @click="$router.push(`/teacher/classes/${maLop}/grading`)"
      >
        <span class="material-symbols-outlined text-[18px]">grade</span>
        Chấm điểm
      </button>
    </div>

    <!-- Cards ghi danh -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 mb-6">
      <div class="bg-white rounded-2xl border border-outline-variant p-5">
        <div class="flex items-center gap-2 mb-3">
          <span class="material-symbols-outlined text-[20px] text-on-surface-variant">key</span>
          <h3 class="font-title-md font-semibold text-on-surface">Mã tham gia lớp</h3>
        </div>
        <div class="flex items-center gap-2 mb-3">
          <span class="font-headline-sm text-2xl font-bold tracking-wider text-primary">{{ cls?.maThamGia }}</span>
        </div>
        <div class="flex flex-wrap gap-2">
          <button class="px-3 py-2 rounded-lg text-body-sm border border-outline-variant hover:bg-surface-container flex items-center gap-1" @click="copy(cls?.maThamGia)">
            <span class="material-symbols-outlined text-[16px]">content_copy</span> Sao chép mã
          </button>
          <button class="px-3 py-2 rounded-lg text-body-sm border border-outline-variant hover:bg-surface-container flex items-center gap-1" @click="copy(joinLink)">
            <span class="material-symbols-outlined text-[16px]">link</span> Sao chép link
          </button>
          <button class="px-3 py-2 rounded-lg text-body-sm border border-outline-variant hover:bg-surface-container flex items-center gap-1 text-error" @click="showReset = true">
            <span class="material-symbols-outlined text-[16px]">refresh</span> Reset mã
          </button>
        </div>
      </div>

      <div class="bg-white rounded-2xl border border-outline-variant p-5">
        <div class="flex items-center gap-2 mb-3">
          <span class="material-symbols-outlined text-[20px] text-on-surface-variant">how_to_reg</span>
          <h3 class="font-title-md font-semibold text-on-surface">Thiết lập ghi danh</h3>
        </div>
        <div class="flex items-center gap-2 mb-1">
          <span
            class="px-2.5 py-1 rounded-full text-body-sm font-medium"
            :class="cls?.ghiDanhMo ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-600'"
          >
            {{ cls?.ghiDanhMo ? 'Đang mở ghi danh' : 'Đã đóng ghi danh' }}
          </span>
        </div>
        <p v-if="cls?.hanGhiDanh" class="text-body-sm text-on-surface-variant mb-3">
          Hạn ghi danh: {{ formatDate(cls.hanGhiDanh) }}
        </p>
        <p v-else class="text-body-sm text-on-surface-variant mb-3">Chưa đặt hạn ghi danh</p>
        <button class="px-3 py-2 rounded-lg text-body-sm border border-outline-variant hover:bg-surface-container flex items-center gap-1" @click="openEnroll">
          <span class="material-symbols-outlined text-[16px]">edit</span> Chỉnh sửa
        </button>
      </div>
    </div>

    <!-- Danh sách sinh viên -->
    <div class="bg-white rounded-2xl border border-outline-variant overflow-hidden mb-6">
      <div class="flex flex-wrap items-center gap-3 p-4 border-b border-outline-variant">
        <button
          class="text-white py-2 px-4 rounded-lg font-medium text-body-md flex items-center gap-1 shadow-sm"
          style="background-color:#005ea3;"
          @click="openImport"
        >
          <span class="material-symbols-outlined text-[18px]">upload</span> Import sinh viên
        </button>
        <div class="relative flex-1 min-w-[220px]">
          <span class="material-symbols-outlined text-[20px] absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant">search</span>
          <input
            v-model="search"
            placeholder="Tìm theo MSSV hoặc họ tên"
            class="w-full pl-10 pr-3 py-2 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none"
            @input="reloadStudents"
          />
        </div>
        <select v-model="statusFilter" class="px-3 py-2 border border-outline-variant rounded-lg text-body-md" @change="reloadStudents">
          <option value="all">Tất cả trạng thái</option>
          <option value="ChoGhiDanh">Chờ ghi danh</option>
          <option value="DangThucTap">Đang thực tập</option>
          <option value="HoanThanh">Hoàn thành</option>
          <option value="DungThucTap">Dừng thực tập</option>
        </select>
        <span class="text-body-sm text-on-surface-variant">{{ students.length }} sinh viên</span>
        <button
          class="px-4 py-2 rounded-lg font-medium text-body-md border border-outline-variant flex items-center gap-1 disabled:opacity-40 disabled:cursor-not-allowed"
          :disabled="selected.length < 2"
          @click="createGroup"
        >
          <span class="material-symbols-outlined text-[18px]">group_add</span> Gom nhóm [{{ selected.length }}]
        </button>
      </div>

      <table class="w-full text-body-md">
        <thead>
          <tr class="text-left text-body-sm text-on-surface-variant border-b border-outline-variant">
            <th class="px-4 py-3 w-10">
              <input type="checkbox" :checked="allChecked" @change="toggleAll" />
            </th>
            <th class="px-4 py-3">Sinh viên</th>
            <th class="px-4 py-3">Nhóm</th>
            <th class="px-4 py-3">Đơn vị thực tập</th>
            <th class="px-4 py-3">Trạng thái</th>
            <th class="px-4 py-3">Tiến độ</th>
            <th class="px-4 py-3 text-right">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="loadingStudents"><td colspan="7" class="px-4 py-10 text-center text-on-surface-variant">Đang tải...</td></tr>
          <tr v-else-if="!students.length"><td colspan="7" class="px-4 py-12 text-center text-on-surface-variant">
            <span class="material-symbols-outlined text-[40px] block mb-2 opacity-50">group</span>
            Chưa có sinh viên. Nhấn "Import sinh viên" để thêm.
          </td></tr>
          <tr v-for="s in students" :key="s.maGhiDanh" class="border-b border-outline-variant/60 hover:bg-surface-container/40">
            <td class="px-4 py-3"><input type="checkbox" :value="s.maGhiDanh" v-model="selected" /></td>
            <td class="px-4 py-3">
              <p class="font-medium text-on-surface">{{ s.hoTen }}</p>
              <p class="text-body-sm text-on-surface-variant">{{ s.maSoSinhVien }}</p>
            </td>
            <td class="px-4 py-3 text-on-surface-variant">{{ s.tenNhom || '-' }}</td>
            <td class="px-4 py-3 text-on-surface-variant">{{ s.donViThucTap || '-' }}</td>
            <td class="px-4 py-3">
              <span class="px-2.5 py-1 rounded-full text-body-sm font-medium" :class="statusCls[s.trangThaiThucTap] || 'bg-gray-100 text-gray-600'">
                {{ statusLabels[s.trangThaiThucTap] || s.trangThaiThucTap }}
              </span>
            </td>
            <td class="px-4 py-3">
              <span v-if="s.tinhTrangTienDo" class="px-2.5 py-1 rounded-full text-body-sm font-medium" :class="progressCls[s.tinhTrangTienDo] || 'bg-gray-100 text-gray-600'">
                {{ progressLabels[s.tinhTrangTienDo] || s.tinhTrangTienDo }}
              </span>
              <span v-else class="text-on-surface-variant">-</span>
            </td>
            <td class="px-4 py-3">
              <div class="flex items-center justify-end gap-1">
                <button class="p-1.5 rounded-lg hover:bg-surface-container" title="Xem hồ sơ" @click="openDetail(s.maGhiDanh)">
                  <span class="material-symbols-outlined text-[20px] text-on-surface-variant">visibility</span>
                </button>
                <button
                  v-if="s.trangThaiThucTap !== 'DungThucTap'"
                  class="p-1.5 rounded-lg hover:bg-surface-container"
                  title="Dừng thực tập"
                  @click="openStop(s)"
                >
                  <span class="material-symbols-outlined text-[20px] text-error">block</span>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Nhóm -->
    <div class="bg-white rounded-2xl border border-outline-variant overflow-hidden">
      <div class="flex items-center justify-between p-4 border-b border-outline-variant">
        <h3 class="font-title-md font-semibold text-on-surface">Danh sách nhóm</h3>
      </div>
      <div v-if="!groups.length" class="px-4 py-10 text-center text-on-surface-variant">
        <span class="material-symbols-outlined text-[36px] block mb-2 opacity-50">diversity_3</span>
        Chưa có nhóm. Chọn ít nhất 2 sinh viên ở bảng trên rồi nhấn "Gom nhóm".
      </div>
      <table v-else class="w-full text-body-md">
        <thead>
          <tr class="text-left text-body-sm text-on-surface-variant border-b border-outline-variant">
            <th class="px-4 py-3">Nhóm</th>
            <th class="px-4 py-3">Thành viên</th>
            <th class="px-4 py-3">Số lượng</th>
            <th class="px-4 py-3 text-right">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="g in groups" :key="g.maNhom" class="border-b border-outline-variant/60">
            <td class="px-4 py-3 font-medium text-on-surface">{{ g.tenNhom }}</td>
            <td class="px-4 py-3 text-on-surface-variant">{{ g.thanhVien.map((m) => m.hoTen).join(', ') }}</td>
            <td class="px-4 py-3 text-on-surface-variant">{{ g.thanhVien.length }}</td>
            <td class="px-4 py-3 text-right">
              <button class="p-1.5 rounded-lg hover:bg-surface-container text-error" title="Giải tán nhóm" @click="disband(g)">
                <span class="material-symbols-outlined text-[20px]">delete</span>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Toast -->
    <div v-if="toast" class="fixed bottom-6 right-6 z-[90] bg-on-surface text-white px-4 py-3 rounded-lg shadow-lg text-body-md">{{ toast }}</div>

    <!-- Reset mã -->
    <div v-if="showReset" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="showReset = false">
      <div class="w-full max-w-[420px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-2">Reset mã tham gia lớp?</h3>
        <p class="text-body-md text-on-surface-variant mb-6">
          Mã tham gia hiện tại sẽ bị vô hiệu hóa và sinh mã mới. Sinh viên đang sử dụng mã cũ sẽ không thể tham gia lớp nữa.
        </p>
        <div class="flex justify-end gap-3">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="showReset = false">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md bg-error" @click="resetCode">Reset mã</button>
        </div>
      </div>
    </div>

    <!-- Thiết lập ghi danh -->
    <div v-if="showEnroll" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="showEnroll = false">
      <div class="w-full max-w-[440px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-4">Thiết lập ghi danh</h3>
        <div class="space-y-4">
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Trạng thái ghi danh</label>
            <select v-model="enrollForm.ghiDanhMo" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md">
              <option :value="true">Đang mở ghi danh</option>
              <option :value="false">Đã đóng ghi danh</option>
            </select>
          </div>
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Hạn ghi danh</label>
            <input v-model="enrollForm.hanGhiDanh" type="datetime-local" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          </div>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="showEnroll = false">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md" style="background-color:#005ea3;" @click="saveEnroll">Lưu</button>
        </div>
      </div>
    </div>

    <!-- Import/Thêm sinh viên -->
    <div v-if="showImport" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="showImport = false">
      <div class="w-full max-w-[480px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-2">Thêm sinh viên vào lớp</h3>
        <p class="text-body-sm text-on-surface-variant mb-4">Nhập danh sách mã số sinh viên (mỗi dòng một MSSV).</p>
        <textarea
          v-model="importText"
          rows="6"
          placeholder="22120001&#10;22120002"
          class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none font-mono"
        ></textarea>
        <div v-if="importResults.length" class="mt-4 max-h-40 overflow-auto border border-outline-variant rounded-lg divide-y divide-outline-variant/60">
          <div v-for="(r, i) in importResults" :key="i" class="flex items-center gap-2 px-3 py-2 text-body-sm">
            <span class="material-symbols-outlined text-[18px]" :class="r.thanhCong ? 'text-green-600' : 'text-error'">
              {{ r.thanhCong ? 'check_circle' : 'cancel' }}
            </span>
            <span class="font-medium">{{ r.maSoSinhVien }}</span>
            <span class="text-on-surface-variant">{{ r.hoTen || r.lyDo }}</span>
          </div>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="showImport = false">Đóng</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md" style="background-color:#005ea3;" @click="doImport">Thêm sinh viên</button>
        </div>
      </div>
    </div>

    <!-- Dừng thực tập -->
    <div v-if="stopTarget" class="fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4" @click.self="stopTarget = null">
      <div class="w-full max-w-[440px] bg-white rounded-2xl shadow-xl p-6">
        <h3 class="font-title-lg text-title-lg font-semibold text-on-surface mb-2">Dừng thực tập</h3>
        <p class="text-body-sm text-on-surface-variant mb-4">
          Dữ liệu lịch sử được giữ lại, sinh viên sẽ bị loại khỏi thống kê tiến độ đang hoạt động.
        </p>
        <div class="space-y-4">
          <div>
            <label class="block text-body-sm text-on-surface-variant mb-1">Lý do dừng</label>
            <select v-model="stopForm.lyDo" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md">
              <option value="Xin rút thực tập">Xin rút thực tập</option>
              <option value="Vi phạm">Vi phạm</option>
              <option value="Mất liên lạc">Mất liên lạc</option>
              <option value="Khác">Khác</option>
            </select>
          </div>
          <div v-if="stopForm.lyDo === 'Khác'">
            <input v-model="stopForm.lyDoKhac" placeholder="Nêu rõ lý do" class="w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none" />
          </div>
        </div>
        <div class="flex justify-end gap-3 mt-6">
          <button class="px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container" @click="stopTarget = null">Hủy</button>
          <button class="px-5 py-2.5 rounded-lg font-medium text-white shadow-md bg-error" @click="confirmStop">Xác nhận</button>
        </div>
      </div>
    </div>

    <!-- Drawer chi tiết sinh viên -->
    <div v-if="detail" class="fixed inset-0 z-[85] flex justify-end bg-black/40" @click.self="detail = null">
      <div class="w-full max-w-[460px] h-full bg-white shadow-xl overflow-auto">
        <div class="flex items-center justify-between p-5 border-b border-outline-variant sticky top-0 bg-white">
          <h3 class="font-title-lg text-title-lg font-semibold text-on-surface">Hồ sơ sinh viên</h3>
          <button class="p-2 rounded-lg hover:bg-surface-container" @click="detail = null">
            <span class="material-symbols-outlined text-on-surface-variant">close</span>
          </button>
        </div>
        <div class="p-5 space-y-5">
          <div>
            <p class="font-title-lg text-title-lg font-semibold text-on-surface">{{ detail.hoTen }}</p>
            <p class="text-body-md text-on-surface-variant">{{ detail.maSoSinhVien }}</p>
          </div>
          <div class="space-y-2 text-body-md">
            <div class="flex justify-between"><span class="text-on-surface-variant">Lớp sinh hoạt</span><span class="text-on-surface">{{ detail.lopSinhHoat || '-' }}</span></div>
            <div class="flex justify-between"><span class="text-on-surface-variant">Email</span><span class="text-on-surface">{{ detail.email || '-' }}</span></div>
            <div class="flex justify-between"><span class="text-on-surface-variant">Số điện thoại</span><span class="text-on-surface">{{ detail.soDienThoai || '-' }}</span></div>
            <div class="flex justify-between"><span class="text-on-surface-variant">Nhóm</span><span class="text-on-surface">{{ detail.tenNhom || '-' }}</span></div>
            <div class="flex justify-between"><span class="text-on-surface-variant">Đơn vị thực tập</span><span class="text-on-surface">{{ detail.donViThucTap || '-' }}</span></div>
            <div class="flex justify-between"><span class="text-on-surface-variant">Vị trí</span><span class="text-on-surface">{{ detail.viTriThucTap || '-' }}</span></div>
            <div class="flex justify-between"><span class="text-on-surface-variant">Trạng thái</span><span class="text-on-surface">{{ statusLabels[detail.trangThaiThucTap] || detail.trangThaiThucTap }}</span></div>
          </div>
          <div v-if="detail.deTai">
            <h4 class="font-title-md font-semibold text-on-surface mb-1">Đề tài</h4>
            <p class="text-body-md text-on-surface">{{ detail.deTai.tenDeTai }}</p>
            <p class="text-body-sm text-on-surface-variant">{{ detail.deTai.moTa }}</p>
          </div>
          <div>
            <h4 class="font-title-md font-semibold text-on-surface mb-2">Điểm</h4>
            <div v-if="!detail.diem.length" class="text-body-sm text-on-surface-variant">Chưa có cột điểm.</div>
            <div v-else class="space-y-1">
              <div v-for="d in detail.diem" :key="d.maCotDiem" class="flex justify-between text-body-md">
                <span class="text-on-surface-variant">{{ d.tenCot }}</span>
                <span class="text-on-surface font-medium">{{ d.diemSo ?? '-' }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import api from '@/api/api';

  const route = useRoute();
  const maLop = route.params.maLop;

  const cls = ref(null);
  const students = ref([]);
  const groups = ref([]);
  const selected = ref([]);
  const loadingStudents = ref(false);
  const search = ref('');
  const statusFilter = ref('all');
  const toast = ref('');

  const statusLabels = { ChoGhiDanh: 'Chờ ghi danh', DangThucTap: 'Đang thực tập', HoanThanh: 'Hoàn thành', DungThucTap: 'Dừng thực tập' };
  const statusCls = { ChoGhiDanh: 'bg-amber-100 text-amber-700', DangThucTap: 'bg-blue-100 text-blue-700', HoanThanh: 'bg-green-100 text-green-700', DungThucTap: 'bg-red-100 text-red-700' };
  const progressLabels = { DungTienDo: 'Đúng tiến độ', ChamTienDo: 'Chậm tiến độ', CanhBao: 'Cảnh báo', CanXuLy: 'Cần xử lý' };
  const progressCls = { DungTienDo: 'bg-green-100 text-green-700', ChamTienDo: 'bg-amber-100 text-amber-700', CanhBao: 'bg-orange-100 text-orange-700', CanXuLy: 'bg-red-100 text-red-700' };

  const joinLink = computed(() => `${window.location.origin}/register?code=${cls.value?.maThamGia || ''}`);

  function showToast(m) { toast.value = m; setTimeout(() => (toast.value = ''), 3000); }
  function formatDate(d) { return new Date(d).toLocaleString('vi-VN'); }
  function copy(text) { navigator.clipboard?.writeText(text || ''); showToast('Đã sao chép'); }

  const allChecked = computed(() => students.value.length > 0 && selected.value.length === students.value.length);
  function toggleAll(e) { selected.value = e.target.checked ? students.value.map((s) => s.maGhiDanh) : []; }

  async function loadClass() {
    const res = await api.get(`/giangvien/classes/${maLop}`);
    cls.value = res.data;
  }

  async function reloadStudents() {
    loadingStudents.value = true;
    try {
      const res = await api.get(`/giangvien/classes/${maLop}/students`, {
        params: { search: search.value || undefined, trangThai: statusFilter.value }
      });
      students.value = res.data;
      selected.value = selected.value.filter((id) => students.value.some((s) => s.maGhiDanh === id));
    } finally { loadingStudents.value = false; }
  }

  async function loadGroups() {
    const res = await api.get(`/giangvien/classes/${maLop}/groups`);
    groups.value = res.data;
  }

  // Reset mã
  const showReset = ref(false);
  async function resetCode() {
    const res = await api.post(`/giangvien/classes/${maLop}/reset-code`);
    cls.value.maThamGia = res.data.maThamGia;
    showReset.value = false;
    showToast(res.data.message);
  }

  // Ghi danh
  const showEnroll = ref(false);
  const enrollForm = ref({ ghiDanhMo: true, hanGhiDanh: '' });
  function openEnroll() {
    enrollForm.value = {
      ghiDanhMo: cls.value.ghiDanhMo,
      hanGhiDanh: cls.value.hanGhiDanh ? cls.value.hanGhiDanh.slice(0, 16) : ''
    };
    showEnroll.value = true;
  }
  async function saveEnroll() {
    const res = await api.put(`/giangvien/classes/${maLop}/enrollment`, {
      ghiDanhMo: enrollForm.value.ghiDanhMo,
      hanGhiDanh: enrollForm.value.hanGhiDanh || null
    });
    showEnroll.value = false;
    await loadClass();
    showToast(res.data.message);
  }

  // Import
  const showImport = ref(false);
  const importText = ref('');
  const importResults = ref([]);
  function openImport() { importText.value = ''; importResults.value = []; showImport.value = true; }
  async function doImport() {
    const codes = importText.value.split(/[\n,;\s]+/).map((x) => x.trim()).filter(Boolean);
    if (!codes.length) { showToast('Chưa nhập MSSV nào'); return; }
    const res = await api.post(`/giangvien/classes/${maLop}/students`, { maSoSinhViens: codes });
    importResults.value = res.data.ketQua;
    showToast(res.data.message);
    await Promise.all([reloadStudents(), loadClass()]);
  }

  // Nhóm
  async function createGroup() {
    if (selected.value.length < 2) return;
    const res = await api.post(`/giangvien/classes/${maLop}/groups`, { maGhiDanhs: selected.value });
    selected.value = [];
    showToast(res.data.message);
    await Promise.all([reloadStudents(), loadGroups()]);
  }
  async function disband(g) {
    await api.delete(`/giangvien/groups/${g.maNhom}`);
    showToast('Đã giải tán nhóm');
    await Promise.all([reloadStudents(), loadGroups()]);
  }

  // Dừng thực tập
  const stopTarget = ref(null);
  const stopForm = ref({ lyDo: 'Xin rút thực tập', lyDoKhac: '' });
  function openStop(s) { stopTarget.value = s; stopForm.value = { lyDo: 'Xin rút thực tập', lyDoKhac: '' }; }
  async function confirmStop() {
    const lyDo = stopForm.value.lyDo === 'Khác' ? (stopForm.value.lyDoKhac || 'Khác') : stopForm.value.lyDo;
    const res = await api.post(`/giangvien/students/${stopTarget.value.maGhiDanh}/stop`, { lyDo });
    stopTarget.value = null;
    showToast(res.data.message);
    await reloadStudents();
  }

  // Chi tiết
  const detail = ref(null);
  async function openDetail(maGhiDanh) {
    const res = await api.get(`/giangvien/students/${maGhiDanh}`);
    detail.value = res.data;
  }

  onMounted(async () => {
    await loadClass();
    await Promise.all([reloadStudents(), loadGroups()]);
  });
</script>
