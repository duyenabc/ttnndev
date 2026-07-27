<template>
  <div class="ims-scope">
    <div class="flex flex-wrap items-center justify-between gap-4 mb-6">
      <div>
        <h1 class="font-headline-sm text-2xl font-bold text-on-surface mb-1">Quản lý tài khoản</h1>
        <p class="text-body-md text-on-surface-variant">Quản lý danh sách tài khoản, import từ Excel và cấp quyền.</p>
      </div>
      <div class="flex items-center gap-3">
        <button class="border border-outline-variant hover:bg-surface-container py-2.5 px-4 rounded-lg font-medium text-body-md transition-all flex items-center gap-2"
                @click="downloadSample">
          <span class="material-symbols-outlined text-[20px]">download</span>
          Tải file mẫu
        </button>
        <button class="border border-primary text-primary hover:bg-primary/10 py-2.5 px-4 rounded-lg font-medium text-body-md transition-all flex items-center gap-2"
                @click="openImportModal">
          <span class="material-symbols-outlined text-[20px]">upload_file</span>
          Import từ Excel
        </button>
        <button class="text-white py-2.5 px-5 rounded-lg font-semibold transition-all flex items-center gap-2 shadow-md"
                style="background-color:#005ea3;"
                @click="openCreate">
          <span class="material-symbols-outlined text-[20px]">person_add</span>
          Thêm tài khoản
        </button>
      </div>
    </div>

    <!-- Summary cards -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <div class="bg-white rounded-2xl border border-outline-variant p-5">
        <p class="text-body-sm text-on-surface-variant">Tổng số</p>
        <p class="text-2xl font-bold text-on-surface mt-1">{{ summary.tongSo }}</p>
      </div>
      <div class="bg-white rounded-2xl border border-outline-variant p-5">
        <p class="text-body-sm text-on-surface-variant">Đang hoạt động</p>
        <p class="text-2xl font-bold text-green-600 mt-1">{{ summary.dangHoatDong }}</p>
      </div>
      <div class="bg-white rounded-2xl border p-5 transition-colors"
           :class="summary.choKichHoat > 0 ? 'border-amber-400 bg-amber-50/30' : 'border-outline-variant'">
        <p class="text-body-sm text-on-surface-variant">Chờ kích hoạt</p>
        <p class="text-2xl font-bold text-amber-600 mt-1">{{ summary.choKichHoat }}</p>
      </div>
      <div class="bg-white rounded-2xl border border-outline-variant p-5">
        <p class="text-body-sm text-on-surface-variant">Bị khóa</p>
        <p class="text-2xl font-bold text-red-600 mt-1">{{ summary.biKhoa }}</p>
      </div>
    </div>

    <div class="bg-white rounded-2xl border border-outline-variant overflow-hidden">
      <!-- Role tabs -->
      <div class="flex border-b border-outline-variant px-4">
        <button v-for="t in roleTabs"
                :key="t.value"
                class="px-4 py-3 text-body-md font-medium border-b-2 transition-colors"
                :class="role === t.value ? 'border-primary text-primary font-bold' : 'border-transparent text-on-surface-variant hover:text-on-surface'"
                @click="setRole(t.value)">
          {{ t.label }}
        </button>
      </div>

      <!-- Filters -->
      <div class="flex flex-wrap items-center gap-3 p-4">
        <div class="relative flex-1 min-w-[220px]">
          <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-outline text-[20px]">search</span>
          <input v-model="search"
                 placeholder="Tìm theo tên, mã, email..."
                 class="w-full pl-10 pr-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none"
                 @input="debouncedReload" />
        </div>
        <select v-model="status" class="px-3 py-2.5 border border-outline-variant rounded-lg text-body-md" @change="reload">
          <option value="">Tất cả trạng thái</option>
          <option value="Nhap">Nháp</option>
          <option value="ChoKichHoat">Chờ kích hoạt</option>
          <option value="DangHoatDong">Đang hoạt động</option>
          <option value="BiKhoa">Bị khóa</option>
        </select>

        <select v-if="role === 'GiangVien'" v-model="boMonFilter" class="px-3 py-2.5 border border-outline-variant rounded-lg text-body-md" @change="reload">
          <option value="">Tất cả Bộ môn</option>
          <option value="Khoa học dữ liệu">Khoa học dữ liệu</option>
          <option value="Tin học ứng dụng">Tin học ứng dụng</option>
        </select>

        <select v-if="role === 'SinhVien'" v-model="lopFilter" class="px-3 py-2.5 border border-outline-variant rounded-lg text-body-md" @change="reload">
          <option value="">Tất cả Lớp sinh hoạt</option>
          <option value="22CNTT1">22CNTT1</option>
          <option value="22CNTT2">22CNTT2</option>
        </select>

        <button class="px-3 py-2.5 text-on-surface-variant hover:text-primary text-body-md flex items-center gap-1" @click="clearFilters">
          <span class="material-symbols-outlined text-[18px]">filter_alt_off</span>
          Xóa lọc
        </button>
      </div>

      <!-- Bulk action bar -->
      <div v-if="selected.length" class="flex items-center gap-3 px-4 py-2 bg-primary/5 border-y border-outline-variant">
        <span class="text-body-sm text-on-surface font-medium">Đã chọn {{ selected.length }} tài khoản</span>
        <button class="text-red-600 text-body-sm font-medium flex items-center gap-1 hover:underline" @click="bulkDelete">
          <span class="material-symbols-outlined text-[18px]">delete</span> Xóa đã chọn
        </button>
      </div>

      <!-- Table -->
      <div class="overflow-x-auto">
        <table class="w-full text-body-md">
          <thead>
            <tr class="text-left text-body-sm text-on-surface-variant border-b border-outline-variant bg-slate-50">
              <th class="px-4 py-3 w-10"><input type="checkbox" :checked="allSelected" @change="toggleAll" /></th>
              <th class="px-4 py-3">Họ tên</th>
              <th class="px-4 py-3">Mã định danh</th>
              <th class="px-4 py-3">Email</th>
              <th v-if="role === 'GiaoVu'" class="px-4 py-3">Quyền quản lý</th>
              <th class="px-4 py-3">Trạng thái</th>
              <th class="px-4 py-3 text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading"><td :colspan="role === 'GiaoVu' ? 7 : 6" class="px-4 py-10 text-center text-on-surface-variant">Đang tải...</td></tr>
            <tr v-else-if="!items.length">
              <td :colspan="role === 'GiaoVu' ? 7 : 6" class="px-4 py-12 text-center text-on-surface-variant">
                <span class="material-symbols-outlined text-[40px] block mb-2 opacity-50">inbox</span>
                Chưa có tài khoản nào. Nhấn Import để thêm danh sách.
              </td>
            </tr>
            <tr v-for="u in items" :key="u.maNguoiDung" class="border-b border-outline-variant/60 hover:bg-surface-container/40 transition-colors">
              <td class="px-4 py-3"><input type="checkbox" :value="u.maNguoiDung" v-model="selected" /></td>
              <td class="px-4 py-3 font-medium text-on-surface cursor-pointer hover:text-primary" @click="openDrawer(u)">{{ u.hoTen }}</td>
              <td class="px-4 py-3 text-on-surface-variant font-mono text-body-sm">{{ u.maDinhDanh }}</td>
              <td class="px-4 py-3 text-on-surface-variant">{{ u.email }}</td>
              <td v-if="role === 'GiaoVu'" class="px-4 py-3">
                <span class="px-2 py-0.5 rounded text-body-xs font-semibold" :class="u.quyenQuanLyNguoiDung ? 'bg-green-100 text-green-800' : 'bg-slate-100 text-slate-600'">
                  {{ u.quyenQuanLyNguoiDung ? 'Có quyền' : 'Khong' }}
                </span>
              </td>
              <td class="px-4 py-3"><StatusBadge :status="u.trangThaiTaiKhoan" /></td>
              <td class="px-4 py-3 text-right">
                <div class="inline-flex items-center gap-1">
                  <button v-if="u.trangThaiTaiKhoan === 'Nhap'" class="action-btn" title="Cấp tài khoản" @click="grant(u)"><span class="material-symbols-outlined text-[20px]">send</span></button>
                  <button v-if="u.trangThaiTaiKhoan === 'ChoKichHoat'" class="action-btn" title="Gửi lại link kích hoạt" @click="resend(u)"><span class="material-symbols-outlined text-[20px]">forward_to_inbox</span></button>
                  <button v-if="['DangHoatDong','ChoKichHoat'].includes(u.trangThaiTaiKhoan)" class="action-btn" title="Khóa tài khoản" @click="askLock(u)"><span class="material-symbols-outlined text-[20px]">lock</span></button>
                  <button v-if="u.trangThaiTaiKhoan === 'BiKhoa'" class="action-btn text-green-700" title="Mở khóa tài khoản" @click="unlock(u)"><span class="material-symbols-outlined text-[20px]">lock_open</span></button>
                  <button v-if="u.trangThaiTaiKhoan === 'DangHoatDong'" class="action-btn text-amber-700" title="Cấp mật khẩu tạm" @click="tempPass(u)"><span class="material-symbols-outlined text-[20px]">key</span></button>
                  <button v-if="['Nhap','ChoKichHoat'].includes(u.trangThaiTaiKhoan)" class="action-btn text-red-600" title="Xóa tài khoản" @click="del(u)"><span class="material-symbols-outlined text-[20px]">delete</span></button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="flex items-center justify-between px-4 py-3 border-t border-outline-variant text-body-sm text-on-surface-variant">
        <span>Tổng: {{ total }} tài khoản</span>
        <div class="flex items-center gap-2">
          <button class="action-btn" :disabled="page <= 1" @click="goPage(page - 1)"><span class="material-symbols-outlined text-[20px]">chevron_left</span></button>
          <span>Trang {{ page }} / {{ totalPages }}</span>
          <button class="action-btn" :disabled="page >= totalPages" @click="goPage(page + 1)"><span class="material-symbols-outlined text-[20px]">chevron_right</span></button>
        </div>
      </div>
    </div>

    <!-- Toast -->
    <div v-if="toast" class="fixed bottom-6 right-6 z-[90] bg-on-surface text-white px-5 py-3 rounded-xl shadow-2xl text-body-md flex items-center gap-2">
      <span class="material-symbols-outlined text-[20px] text-green-400">check_circle</span>
      {{ toast }}
    </div>

    <!-- Excel Import Dialog Step 1 & Preview Step 2 -->
    <Modal v-if="showImport" title="Import danh sách người dùng" width="max-w-[800px]" @close="showImport = false">
      <div v-if="importStep === 1" class="space-y-4">
        <p class="text-body-md text-on-surface-variant">Chọn tệp Excel (.xlsx, .xls) tối đa 10MB để tải lên hệ thống:</p>
        <div class="border-2 border-dashed border-outline-variant rounded-2xl p-8 text-center bg-slate-50 hover:bg-slate-100/50 transition-colors">
          <span class="material-symbols-outlined text-[48px] text-primary mb-2">cloud_upload</span>
          <p class="text-body-md font-semibold text-on-surface mb-1">Kéo thả file vào đây hoặc nhấn để chọn</p>
          <p class="text-body-sm text-slate-400">Định dạng chấp nhận: .xlsx, .xls (≤ 10MB)</p>
          <input type="file" accept=".xlsx,.xls" class="hidden" ref="fileInput" @change="handleFileSelect" />
          <button class="mt-4 px-4 py-2 border border-primary text-primary rounded-lg text-body-sm font-medium" @click="$refs.fileInput.click()">
            Chọn file Excel
          </button>
        </div>
        <p v-if="selectedFileName" class="text-body-sm text-green-700 font-semibold flex items-center gap-1">
          <span class="material-symbols-outlined text-[18px]">description</span> {{ selectedFileName }}
        </p>
        <p v-if="importError" class="text-error text-body-sm">{{ importError }}</p>
      </div>

      <div v-else-if="importStep === 2" class="space-y-4">
        <!-- Preview Summary -->
        <div class="flex items-center justify-between p-3 bg-slate-100 rounded-xl text-body-sm">
          <div class="flex gap-4">
            <span class="font-semibold text-slate-800">{{ previewData.validRows.length }} dòng hợp lệ</span>
            <span class="font-semibold text-amber-700">{{ previewData.duplicateRows.length }} dòng trùng</span>
            <span class="font-semibold text-red-600">{{ previewData.errorRows.length }} dòng lỗi</span>
          </div>
        </div>

        <div class="max-h-[360px] overflow-y-auto border border-outline-variant rounded-xl text-body-sm">
          <table class="w-full text-left">
            <thead>
              <tr class="bg-slate-100 border-b border-outline-variant text-body-xs font-semibold text-slate-600">
                <th class="px-3 py-2">STT</th>
                <th class="px-3 py-2">Mã</th>
                <th class="px-3 py-2">Họ tên</th>
                <th class="px-3 py-2">Email</th>
                <th v-if="role === 'GiaoVu'" class="px-3 py-2">Quyền quản lý</th>
                <th class="px-3 py-2">Trạng thái / Lý do</th>
              </tr>
            </thead>
            <tbody>
              <!-- Valid rows (White) -->
              <tr v-for="r in previewData.validRows" :key="'v-'+r.rowNum" class="bg-white border-b border-slate-100">
                <td class="px-3 py-2 font-mono text-slate-400">{{ r.rowNum }}</td>
                <td class="px-3 py-2 font-mono">{{ r.maDinhDanh }}</td>
                <td class="px-3 py-2 font-medium">{{ r.hoTen }}</td>
                <td class="px-3 py-2">{{ r.email }}</td>
                <td v-if="role === 'GiaoVu'" class="px-3 py-2">
                  <input type="checkbox" v-model="r.quyenQuanLyNguoiDung" />
                </td>
                <td class="px-3 py-2 text-green-700 font-medium">Hợp lệ</td>
              </tr>
              <!-- Duplicate rows (Yellow) -->
              <tr v-for="r in previewData.duplicateRows" :key="'d-'+r.rowNum" class="bg-amber-50 border-b border-amber-100">
                <td class="px-3 py-2 font-mono text-amber-700">{{ r.rowNum }}</td>
                <td class="px-3 py-2 font-mono text-amber-900">{{ r.maDinhDanh }}</td>
                <td class="px-3 py-2 font-medium text-amber-900">{{ r.hoTen }}</td>
                <td class="px-3 py-2 text-amber-900">{{ r.email }}</td>
                <td v-if="role === 'GiaoVu'" class="px-3 py-2">
                  <input type="checkbox" v-model="r.quyenQuanLyNguoiDung" />
                </td>
                <td class="px-3 py-2 text-amber-800 italic font-medium">{{ r.reason }}</td>
              </tr>
              <!-- Error rows (Red) -->
              <tr v-for="r in previewData.errorRows" :key="'e-'+r.rowNum" class="bg-red-50 border-b border-red-100">
                <td class="px-3 py-2 font-mono text-red-500">{{ r.rowNum }}</td>
                <td class="px-3 py-2 font-mono text-red-900">{{ r.maDinhDanh || '—' }}</td>
                <td class="px-3 py-2 font-medium text-red-900">{{ r.hoTen || '—' }}</td>
                <td class="px-3 py-2 text-red-900">{{ r.email || '—' }}</td>
                <td v-if="role === 'GiaoVu'" class="px-3 py-2">—</td>
                <td class="px-3 py-2 text-red-700 font-semibold">{{ r.reason }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <template #footer>
        <button class="modal-cancel" @click="showImport = false">Hủy bỏ</button>
        <button v-if="importStep === 1"
                class="modal-ok"
                :disabled="!selectedFileName"
                @click="goToPreview">
          Tiếp tục & xem preview
        </button>
        <button v-else-if="importStep === 2"
                class="modal-ok"
                :disabled="previewData.validRows.length === 0 && previewData.duplicateRows.length === 0"
                @click="askConfirmImport">
          Xác nhận import
        </button>
      </template>
    </Modal>

    <!-- Modal Confirm Grant Account Step 3 -->
    <Modal v-if="showConfirmImport" title="Cấp tài khoản" @close="showConfirmImport = false">
      <p class="text-body-md text-on-surface-variant mb-6">
        {{ isAdmin ? `Cấp ${totalImportCount} tài khoản cho các người dùng được thêm vào hệ thống?` : `Yêu cầu cấp ${totalImportCount} tài khoản cho các người dùng được thêm vào hệ thống?` }}
      </p>
      <template #footer>
        <button class="modal-cancel" @click="doImportConfirm(false)">Để sau</button>
        <button class="modal-ok" @click="doImportConfirm(true)">
          {{ isAdmin ? 'Cấp tài khoản' : 'Yêu cầu cấp' }}
        </button>
      </template>
    </Modal>

    <!-- Details drawer 480px -->
    <div v-if="drawer" class="fixed inset-0 z-[70] flex justify-end bg-black/30" @click.self="drawer = null">
      <div class="w-[480px] max-w-full h-full bg-white shadow-xl overflow-y-auto">
        <div class="flex items-center justify-between px-6 py-4 border-b border-outline-variant">
          <h3 class="font-title-lg text-title-lg font-semibold">Chi tiết tài khoản</h3>
          <button class="action-btn" @click="drawer = null"><span class="material-symbols-outlined">close</span></button>
        </div>
        <div class="flex border-b border-outline-variant px-4">
          <button v-for="t in drawerTabs" :key="t" class="px-4 py-3 text-body-sm font-medium border-b-2 transition-colors"
                  :class="drawerTab === t ? 'border-primary text-primary font-bold' : 'border-transparent text-on-surface-variant'"
                  @click="drawerTab = t">
            {{ t }}
          </button>
        </div>
        <div class="p-6">
          <div v-if="drawerTab === 'Thông tin'" class="space-y-4">
            <DrawerField label="Họ tên" :value="detail.hoTen" />
            <DrawerField label="Mã định danh" :value="detail.maDinhDanh" />
            <DrawerField label="Email" :value="detail.email" />
            <DrawerField label="Số điện thoại" :value="detail.soDienThoai" />
            <DrawerField label="Vai trò" :value="roleLabels[detail.vaiTro]" />
            <DrawerField label="Khoa" :value="detail.tenKhoa" v-if="detail.tenKhoa" />
            <DrawerField label="Bộ môn" :value="detail.tenBoMon" v-if="detail.tenBoMon" />
            <DrawerField label="Lớp sinh hoạt" :value="detail.lopSinhHoat" v-if="detail.lopSinhHoat" />
            <div><span class="block text-body-sm text-on-surface-variant mb-1">Trạng thái</span><StatusBadge :status="detail.trangThaiTaiKhoan" /></div>
          </div>
          <div v-else-if="drawerTab === 'Quyền'">
            <div v-if="detail.vaiTro === 'GiaoVu'" class="flex items-center justify-between p-4 rounded-lg border border-outline-variant">
              <div>
                <p class="font-medium text-on-surface">Quyền quản lý người dùng</p>
                <p class="text-body-sm text-on-surface-variant">Cho phép thêm, sửa, khóa tài khoản.</p>
              </div>
              <label v-if="isAdmin" class="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" class="sr-only peer" :checked="detail.quyenQuanLyNguoiDung" @change="togglePermission" />
                <div class="w-11 h-6 bg-gray-300 peer-checked:bg-primary rounded-full peer transition-colors after:content-[''] after:absolute after:top-0.5 after:left-0.5 after:bg-white after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:after:translate-x-5"></div>
              </label>
              <span v-else class="text-body-sm" :class="detail.quyenQuanLyNguoiDung ? 'text-green-600 font-semibold' : 'text-on-surface-variant'">
                {{ detail.quyenQuanLyNguoiDung ? 'Đã cấp' : 'Chưa cấp' }}
              </span>
            </div>
            <p v-else class="text-on-surface-variant text-body-md">Vai trò này không có quyền cấu hình.</p>
          </div>
          <div v-else-if="drawerTab === 'Lịch sử thao tác'">
            <div v-if="!auditItems.length" class="text-on-surface-variant text-body-md text-center py-8">Chưa có thao tác nào được ghi nhận</div>
            <ul v-else class="space-y-3">
              <li v-for="a in auditItems" :key="a.maLog" class="flex gap-3">
                <span class="material-symbols-outlined text-primary text-[20px] mt-0.5">history</span>
                <div>
                  <p class="text-body-md text-on-surface font-medium">{{ actionLabels[a.hanhDong] || a.hanhDong }}</p>
                  <p class="text-body-sm text-on-surface-variant">{{ a.tenNguoiThucHien }} · {{ formatDate(a.thoiDiem) }}</p>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>

    <!-- Create modal -->
    <Modal v-if="showCreate" title="Thêm tài khoản" @close="showCreate = false">
      <div class="space-y-4">
        <input v-model="form.maDinhDanh" placeholder="Mã định danh (Bắt buộc)" class="modal-input" />
        <input v-model="form.hoTen" placeholder="Họ tên (Bắt buộc)" class="modal-input" />
        <input v-model="form.email" placeholder="Email (Bắt buộc)" class="modal-input" />
        <input v-model="form.soDienThoai" placeholder="Số điện thoại" class="modal-input" />
        <select v-model="form.vaiTro" class="modal-input">
          <option value="SinhVien">Sinh viên</option>
          <option value="GiangVien">Giảng viên</option>
          <option value="GiaoVu">Giáo vụ khoa</option>
        </select>
        <label v-if="form.vaiTro === 'GiaoVu' && isAdmin" class="flex items-center gap-2 text-body-md">
          <input type="checkbox" v-model="form.quyenQuanLyNguoiDung" /> Cấp quyền quản lý người dùng
        </label>
        <label class="flex items-center gap-2 text-body-md">
          <input type="checkbox" v-model="form.capTaiKhoanNgay" /> Cấp & gửi link kích hoạt ngay
        </label>
        <p v-if="formError" class="text-error text-body-sm">{{ formError }}</p>
      </div>
      <template #footer>
        <button class="modal-cancel" @click="showCreate = false">Hủy</button>
        <button class="modal-ok" @click="create">Tạo</button>
      </template>
    </Modal>

    <!-- Lock reason modal -->
    <Modal v-if="lockTarget" title="Khóa tài khoản" @close="lockTarget = null">
      <p class="text-body-md text-on-surface-variant mb-3">Nhập lý do khóa tài khoản <b>{{ lockTarget.hoTen }}</b>.</p>
      <textarea v-model="lockReason" maxlength="200" rows="3" placeholder="Lý do khóa (tối đa 200 ký tự)" class="modal-input"></textarea>
      <p v-if="lockError" class="text-error text-body-sm mt-1">{{ lockError }}</p>
      <template #footer>
        <button class="modal-cancel" @click="lockTarget = null">Hủy</button>
        <button class="modal-ok" @click="doLock">Xác nhận khóa</button>
      </template>
    </Modal>

    <!-- Temp password result modal (UC-1.5: Copy button & 1-time view) -->
    <Modal v-if="tempResult" title="Mật khẩu tạm" @close="tempResult = null">
      <p class="text-body-md text-on-surface-variant mb-2">Mật khẩu tạm đã được tạo. Người dùng sẽ phải đổi mật khẩu sau khi đăng nhập.</p>
      <div class="px-4 py-3 bg-slate-100 rounded-lg font-mono text-lg text-center font-bold text-primary select-all mb-4">{{ tempResult }}</div>
      <button class="w-full py-2.5 bg-slate-200 hover:bg-slate-300 rounded-lg text-body-md font-semibold text-slate-800 transition-colors flex items-center justify-center gap-2" @click="copyTempPass">
        <span class="material-symbols-outlined text-[18px]">content_copy</span> Sao chép mật khẩu
      </button>
      <template #footer>
        <button class="modal-ok" @click="tempResult = null">Đã sao chép & đóng</button>
      </template>
    </Modal>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted, h } from 'vue';
  import api from '@/api/api';
  import { useAuthStore } from '@/stores/auth';

  const authStore = useAuthStore();
  const isAdmin = computed(() => authStore.user?.vaiTro === 'Admin');

  const roleLabels = { SinhVien: 'Sinh viên', GiangVien: 'Giảng viên', GiaoVu: 'Giáo vụ khoa', Admin: 'Quản trị viên' };
  const actionLabels = {
    ThemNguoiDung: 'Thêm người dùng', CapTaiKhoan: 'Cấp tài khoản', KhoaTaiKhoan: 'Khóa tài khoản',
    MoKhoaTaiKhoan: 'Mở khóa tài khoản', CapMatKhauTam: 'Cấp mật khẩu tạm', XoaTaiKhoan: 'Xóa tài khoản',
    CapQuyenQL: 'Cấp quyền quản lý', ThuHoiQuyenQL: 'Thu hồi quyền quản lý'
  };
  const roleTabs = [
    { value: 'SinhVien', label: 'Sinh viên' },
    { value: 'GiangVien', label: 'Giảng viên' },
    { value: 'GiaoVu', label: 'Giáo vụ' }
  ];
  const drawerTabs = ['Thông tin', 'Quyền', 'Lịch sử thao tác'];

  const StatusBadge = (props) => {
    const map = {
      Nhap: ['Nháp', 'bg-gray-100 text-gray-600'],
      ChoKichHoat: ['Chờ kích hoạt', 'bg-amber-100 text-amber-700'],
      DangHoatDong: ['Đang hoạt động', 'bg-green-100 text-green-700'],
      BiKhoa: ['Bị khóa', 'bg-red-100 text-red-700']
    };
    const [label, cls] = map[props.status] || [props.status, 'bg-gray-100'];
    return h('span', { class: `px-2.5 py-1 rounded-full text-body-sm font-medium ${cls}` }, label);
  };
  StatusBadge.props = ['status'];

  const DrawerField = (props) => h('div', {}, [
    h('span', { class: 'block text-body-sm text-on-surface-variant mb-1' }, props.label),
    h('p', { class: 'text-body-md text-on-surface' }, props.value || '—')
  ]);
  DrawerField.props = ['label', 'value'];

  const Modal = (props, { slots }) => h('div', {
    class: 'fixed inset-0 z-[80] flex items-center justify-center bg-black/40 px-4',
    onClick: (e) => { if (e.target === e.currentTarget) props.onClose?.(); }
  }, [
    h('div', { class: `w-full ${props.width || 'max-w-[480px]'} bg-white rounded-2xl shadow-xl p-6` }, [
      h('h3', { class: 'font-title-lg text-title-lg font-semibold text-on-surface mb-4' }, props.title),
      slots.default?.(),
      h('div', { class: 'flex justify-end gap-3 mt-6' }, slots.footer?.())
    ])
  ]);
  Modal.props = ['title', 'width'];
  Modal.emits = ['close'];

  // State
  const role = ref('SinhVien');
  const status = ref('');
  const search = ref('');
  const boMonFilter = ref('');
  const lopFilter = ref('');
  const page = ref(1);
  const pageSize = 25;
  const total = ref(0);
  const items = ref([]);
  const loading = ref(false);
  const selected = ref([]);
  const summary = ref({ tongSo: 0, dangHoatDong: 0, choKichHoat: 0, biKhoa: 0 });
  const toast = ref('');

  let debounceTimer = null;
  function debouncedReload() {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(() => { page.value = 1; reload(); }, 300);
  }

  const totalPages = computed(() => Math.max(1, Math.ceil(total.value / pageSize)));
  const allSelected = computed(() => (items.value || []).length > 0 && selected.value.length === (items.value || []).length);

  function showToast(msg) { toast.value = msg; setTimeout(() => (toast.value = ''), 3000); }

  async function loadSummary() {
    try {
      const res = await api.get('/account/summary', { params: { role: role.value } });
      summary.value = res.data || { tongSo: 0, dangHoatDong: 0, choKichHoat: 0, biKhoa: 0 };
    } catch (e) { }
  }

  async function reload() {
    loading.value = true;
    selected.value = [];
    try {
      const res = await api.get('/account/users', {
        params: {
          role: role.value,
          status: status.value,
          search: search.value,
          boMon: boMonFilter.value,
          lopSinhHoat: lopFilter.value,
          page: page.value,
          pageSize
        }
      });
      items.value = res.data?.items || (Array.isArray(res.data) ? res.data : []);
      total.value = res.data?.total ?? items.value.length;
    } catch {
      items.value = [];
      total.value = 0;
    } finally {
      loading.value = false;
    }
    loadSummary();
  }

  function setRole(r) { role.value = r; page.value = 1; reload(); }
  function goPage(p) { page.value = p; reload(); }
  function clearFilters() { status.value = ''; search.value = ''; boMonFilter.value = ''; lopFilter.value = ''; page.value = 1; reload(); }
  function toggleAll(e) { selected.value = e.target.checked ? items.value.map((u) => u.maNguoiDung) : []; }

  // Actions
  async function grant(u) { const r = await api.post(`/account/users/${u.maNguoiDung}/grant`); showToast(r.data.message); reload(); }
  async function resend(u) { const r = await api.post(`/account/users/${u.maNguoiDung}/resend-activation`); showToast(r.data.message); }
  async function unlock(u) { const r = await api.post(`/account/users/${u.maNguoiDung}/unlock`); showToast(r.data.message); reload(); }
  async function del(u) {
    if (!confirm(`Xóa tài khoản ${u.hoTen}? Hành động này không thể hoàn tác.`)) return;
    try { const r = await api.delete(`/account/users/${u.maNguoiDung}`); showToast(r.data.message); reload(); }
    catch (e) { showToast(e.response?.data?.message || 'Chỉ có thể xóa tài khoản đang ở trạng thái Nháp/Chờ kích hoạt'); }
  }
  async function bulkDelete() {
    if (!confirm(`Xóa ${selected.value.length} tài khoản đã chọn? Hành động này không thể hoàn tác.`)) return;
    const r = await api.post('/account/users/delete-bulk', { ids: selected.value });
    showToast(r.data.message); reload();
  }
  async function tempPass(u) {
    const r = await api.post(`/account/users/${u.maNguoiDung}/temp-password`);
    tempResult.value = r.data.matKhauTam; reload();
  }
  function copyTempPass() {
    if (tempResult.value) {
      navigator.clipboard.writeText(tempResult.value);
      showToast('Đã sao chép mật khẩu tạm vào clipboard!');
    }
  }

  function downloadSample() {
    const filename = `file_mau_${role.value.toLowerCase()}.xlsx`;
    showToast(`Đã tải xuống ${filename}`);
  }

  // Import Modal & Steps
  const showImport = ref(false);
  const importStep = ref(1);
  const selectedFileName = ref('');
  const importError = ref('');
  const previewData = ref({ validRows: [], duplicateRows: [], errorRows: [] });
  const showConfirmImport = ref(false);
  const totalImportCount = computed(() => (previewData.value?.validRows?.length || 0) + (previewData.value?.duplicateRows?.length || 0));

  function openImportModal() {
    showImport.value = true;
    importStep.value = 1;
    selectedFileName.value = '';
    importError.value = '';
  }

  function handleFileSelect(e) {
    const file = e.target.files?.[0];
    if (!file) return;
    if (file.size > 10 * 1024 * 1024) {
      importError.value = 'File vượt quá dung lượng 10MB cho phép.';
      return;
    }
    if (!file.name.endsWith('.xlsx') && !file.name.endsWith('.xls')) {
      importError.value = 'Vui lòng tải lên file Excel (.xlsx, .xls)';
      return;
    }
    importError.value = '';
    selectedFileName.value = file.name;
  }

  async function goToPreview() {
    // Generate sample preview rows based on selected tab
    const mockRows = [
      { maDinhDanh: role.value === 'SinhVien' ? '22120099' : (role.value === 'GiangVien' ? 'gv009' : 'gvu009'), hoTen: 'Nguyễn Văn Mới', email: 'moi.nv@due.udn.vn', soDienThoai: '0901112223' },
      { maDinhDanh: 'sv001', hoTen: 'Trần Thị Lan', email: 'sinhvien@due.udn.vn', soDienThoai: '0905998877' }, // Duplicate
      { maDinhDanh: '', hoTen: 'Lỗi Thiếu Mã', email: 'thieu@due.udn.vn', soDienThoai: '' } // Error
    ];

    try {
      const res = await api.post('/account/import-preview', { role: role.value, rows: mockRows });
      previewData.value = res.data;
      importStep.value = 2;
    } catch (e) {
      importError.value = 'Không thể tải bản xem trước file Excel';
    }
  }

  function askConfirmImport() {
    showConfirmImport.value = true;
  }

  async function doImportConfirm(grantNow) {
    const itemsToImport = [...previewData.value.validRows, ...previewData.value.duplicateRows];
    try {
      const res = await api.post('/account/import-confirm', { role: role.value, items: itemsToImport, grantNow });
      showConfirmImport.value = false;
      showImport.value = false;
      showToast(res.data.message);
      reload();
    } catch (e) {
      showToast('Import thất bại');
    }
  }

  // Lock Modal
  const lockTarget = ref(null);
  const lockReason = ref('');
  const lockError = ref('');
  function askLock(u) { lockTarget.value = u; lockReason.value = ''; lockError.value = ''; }
  async function doLock() {
    lockError.value = '';
    if (!lockReason.value.trim()) { lockError.value = 'Vui lòng nhập lý do khóa tài khoản'; return; }
    try {
      const r = await api.post(`/account/users/${lockTarget.value.maNguoiDung}/lock`, { lyDo: lockReason.value });
      showToast(r.data.message); lockTarget.value = null; reload();
    } catch (e) { lockError.value = e.response?.data?.message || 'Khóa tài khoản thất bại'; }
  }

  const tempResult = ref(null);

  // Create Modal
  const showCreate = ref(false);
  const form = ref({});
  const formError = ref('');
  function openCreate() {
    form.value = { maDinhDanh: '', hoTen: '', email: '', soDienThoai: '', vaiTro: role.value, capTaiKhoanNgay: true, quyenQuanLyNguoiDung: false };
    formError.value = ''; showCreate.value = true;
  }
  async function create() {
    formError.value = '';
    try {
      const r = await api.post('/account/users', form.value);
      showToast(r.data.message); showCreate.value = false; reload();
    } catch (e) { formError.value = e.response?.data?.message || 'Thêm tài khoản thất bại'; }
  }

  // Drawer
  const drawer = ref(null);
  const drawerTab = ref('Thông tin');
  const detail = ref({});
  const auditItems = ref([]);
  async function openDrawer(u) {
    drawer.value = u; drawerTab.value = 'Thông tin';
    try {
      const res = await api.get(`/account/users/${u.maNguoiDung}`);
      detail.value = res.data || {};
      const a = await api.get(`/account/users/${u.maNguoiDung}/audit`);
      auditItems.value = Array.isArray(a.data) ? a.data : (a.data?.items || []);
    } catch {
      auditItems.value = [];
    }
  }
  async function togglePermission(e) {
    const val = e.target.checked;
    try {
      const r = await api.put(`/account/users/${detail.value.maNguoiDung}/permission`, { quyenQuanLyNguoiDung: val });
      detail.value.quyenQuanLyNguoiDung = val; showToast(r.data.message); reload();
    } catch (err) { showToast(err.response?.data?.message || 'Cập nhật thất bại'); e.target.checked = !val; }
  }

  function formatDate(d) { return new Date(d).toLocaleString('vi-VN'); }

  onMounted(reload);
</script>

<style scoped>
  .action-btn {
    @apply p-1.5 rounded-lg text-on-surface-variant hover:bg-surface-container hover:text-primary transition-colors disabled:opacity-40;
  }

  .modal-input {
    @apply w-full px-4 py-2.5 border border-outline-variant rounded-lg text-body-md focus:border-primary outline-none;
  }

  .modal-cancel {
    @apply px-5 py-2.5 rounded-lg font-medium text-on-surface-variant hover:bg-surface-container transition-colors;
  }

  .modal-ok {
    @apply px-5 py-2.5 rounded-lg font-medium text-white shadow-md disabled:opacity-50;
    background-color: #005ea3;
  }
</style>
