<template>
  <div class="ims-scope space-y-6">
    <!-- Top Header -->
    <div class="bg-white rounded-2xl border border-slate-200 p-6 shadow-xs flex flex-wrap items-center justify-between gap-4">
      <div>
        <div class="flex items-center gap-2 text-xs font-semibold text-blue-600 uppercase tracking-wider mb-1">
          <span class="material-symbols-outlined text-[18px]">folder_shared</span> Kho Tài liệu & Biểu mẫu IMS
        </div>
        <h1 class="text-2xl md:text-3xl font-extrabold text-slate-900">Kho Tài liệu Thực tập</h1>
        <p class="text-sm text-slate-600 mt-1">
          Tra cứu, tải biểu mẫu chính thức, quy chế thực tập và tài liệu hướng dẫn dành cho Sinh viên & Giảng viên DUE
        </p>
      </div>

      <!-- Upload Button for Teachers / GiaoVu / Admin -->
      <div v-if="canUpload" class="flex items-center gap-3">
        <button @click="showUploadModal = true"
                class="px-4 py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-md transition duration-200 flex items-center gap-2 text-sm">
          <span class="material-symbols-outlined text-[20px]">upload_file</span>
          Tải lên Tài liệu mới
        </button>
      </div>
    </div>

    <!-- Stats Bar -->
    <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
      <div class="bg-white p-4 rounded-xl border border-slate-200 shadow-xs flex items-center gap-3">
        <div class="w-10 h-10 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[22px]">description</span>
        </div>
        <div>
          <p class="text-xs text-slate-500 font-semibold uppercase">Tổng tài liệu</p>
          <p class="text-xl font-bold text-slate-900">{{ documents.length }} tài liệu</p>
        </div>
      </div>

      <div class="bg-white p-4 rounded-xl border border-slate-200 shadow-xs flex items-center gap-3">
        <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[22px]">push_pin</span>
        </div>
        <div>
          <p class="text-xs text-slate-500 font-semibold uppercase">Được ghim</p>
          <p class="text-xl font-bold text-amber-700">{{ pinnedCount }} văn bản</p>
        </div>
      </div>

      <div class="bg-white p-4 rounded-xl border border-slate-200 shadow-xs flex items-center gap-3">
        <div class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[22px]">download</span>
        </div>
        <div>
          <p class="text-xs text-slate-500 font-semibold uppercase">Lượt tải về</p>
          <p class="text-xl font-bold text-emerald-700">{{ totalDownloads }} lượt</p>
        </div>
      </div>

      <div class="bg-white p-4 rounded-xl border border-slate-200 shadow-xs flex items-center gap-3">
        <div class="w-10 h-10 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[22px]">assignment</span>
        </div>
        <div>
          <p class="text-xs text-slate-500 font-semibold uppercase">Biểu mẫu chuẩn</p>
          <p class="text-xl font-bold text-indigo-700">{{ formCount }} mẫu đơn</p>
        </div>
      </div>
    </div>

    <!-- Search & Filter Controls -->
    <div class="bg-white rounded-2xl border border-slate-200 p-5 shadow-xs space-y-4">
      <!-- Main Category Tabs + Search Bar -->
      <div class="flex flex-col md:flex-row gap-4 justify-between items-stretch md:items-center">
        <!-- Category Tabs -->
        <div class="flex flex-wrap items-center gap-2 border-b md:border-none pb-2 md:pb-0">
          <button v-for="cat in categories"
                  :key="cat.value"
                  @click="selectedCategory = cat.value"
                  :class="selectedCategory === cat.value
              ? 'bg-blue-600 text-white font-bold shadow-xs'
              : 'bg-slate-100 text-slate-600 hover:bg-slate-200 font-medium'"
                  class="px-3.5 py-2 rounded-xl text-xs transition duration-150 flex items-center gap-1.5">
            <span class="material-symbols-outlined text-[16px]">{{ cat.icon }}</span>
            {{ cat.label }}
          </button>
        </div>

        <!-- Search Input -->
        <div class="relative min-w-[260px]">
          <input v-model="searchQuery"
                 type="text"
                 placeholder="Tìm tên tài liệu, tác giả, tag..."
                 class="w-full pl-10 pr-4 py-2 rounded-xl border border-slate-300 text-xs text-slate-800 placeholder-slate-400 outline-none focus:ring-2 focus:ring-blue-600 focus:border-transparent transition" />
          <span class="material-symbols-outlined absolute left-3 top-2.5 text-slate-400 text-[18px]">search</span>
          <button v-if="searchQuery"
                  @click="searchQuery = ''"
                  class="absolute right-3 top-2.5 text-slate-400 hover:text-slate-600 text-xs">
            ✕
          </button>
        </div>
      </div>

      <!-- Advanced Filter Row: File Type & Academic Year -->
      <div class="pt-3 border-t border-slate-100 grid grid-cols-1 md:grid-cols-2 gap-4 items-center">
        <!-- Filter by File Type (PDF, DOCX, PPTX) -->
        <div class="flex flex-wrap items-center gap-2">
          <span class="text-xs font-bold text-slate-700 flex items-center gap-1">
            <span class="material-symbols-outlined text-[16px] text-slate-400">description</span> Loại tệp:
          </span>
          <button v-for="ft in fileTypeOptions"
                  :key="ft.value"
                  @click="selectedFileType = ft.value"
                  :class="selectedFileType === ft.value ? ft.activeClass : 'bg-slate-100 text-slate-600 hover:bg-slate-200'"
                  class="px-2.5 py-1 rounded-lg text-xs font-bold transition flex items-center gap-1">
            <span v-if="ft.badge" :class="ft.badge" class="w-2 h-2 rounded-full"></span>
            {{ ft.label }}
          </button>
        </div>

        <!-- Filter by Academic Year (Năm học) -->
        <div class="flex flex-wrap items-center gap-2 md:justify-end">
          <span class="text-xs font-bold text-slate-700 flex items-center gap-1">
            <span class="material-symbols-outlined text-[16px] text-slate-400">school</span> Năm học:
          </span>
          <select v-model="selectedAcademicYear"
                  class="px-3 py-1.5 rounded-xl border border-slate-300 text-xs text-slate-800 font-semibold bg-slate-50 hover:bg-white focus:ring-2 focus:ring-blue-600 outline-none transition">
            <option value="ALL">Tất cả năm học</option>
            <option value="2025-2026">Năm học 2025 - 2026</option>
            <option value="2024-2025">Năm học 2024 - 2025</option>
            <option value="2023-2024">Năm học 2023 - 2024</option>
          </select>

          <!-- Reset Filter Button if active filters -->
          <button v-if="hasActiveFilters"
                  @click="resetFilters"
                  class="px-2.5 py-1 text-rose-600 hover:bg-rose-50 rounded-lg text-xs font-semibold transition flex items-center gap-1">
            <span class="material-symbols-outlined text-[14px]">restart_alt</span> Xóa bộ lọc
          </button>
        </div>
      </div>

      <!-- Popular Tags Filter -->
      <div class="flex items-center gap-2 text-xs text-slate-500 pt-2 border-t border-slate-100 flex-wrap">
        <span class="font-bold text-slate-700 flex items-center gap-1">
          <span class="material-symbols-outlined text-[16px] text-slate-400">label</span> Thẻ từ khóa:
        </span>
        <button v-for="tag in popularTags"
                :key="tag"
                @click="toggleTag(tag)"
                :class="selectedTag === tag ? 'bg-blue-100 text-blue-800 border-blue-300 font-bold' : 'bg-slate-50 text-slate-600 border-slate-200 hover:bg-slate-100'"
                class="px-2.5 py-1 rounded-lg border text-[11px] transition">
          #{{ tag }}
        </button>
        <button v-if="selectedTag"
                @click="selectedTag = ''"
                class="text-rose-600 hover:underline text-[11px] font-semibold ml-2">
          Bỏ chọn tag
        </button>
      </div>
    </div>

    <!-- Document List Grid / Cards -->
    <div v-if="loading" class="text-center py-12 bg-white rounded-2xl border border-slate-200">
      <span class="material-symbols-outlined text-[36px] text-blue-600 animate-spin">progress_activity</span>
      <p class="text-sm text-slate-500 mt-2">Đang tải danh sách tài liệu...</p>
    </div>

    <div v-else-if="filteredDocuments.length === 0" class="text-center py-16 bg-white rounded-2xl border border-slate-200 p-8 space-y-3">
      <div class="w-16 h-16 bg-slate-100 text-slate-400 rounded-full flex items-center justify-center mx-auto">
        <span class="material-symbols-outlined text-[36px]">folder_off</span>
      </div>
      <h3 class="font-bold text-slate-800 text-base">Không tìm thấy tài liệu phù hợp</h3>
      <p class="text-xs text-slate-500 max-w-md mx-auto">Thử đổi từ khóa tìm kiếm hoặc chọn danh mục khác để xem thêm biểu mẫu.</p>
      <button @click="resetFilters" class="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold rounded-xl transition">
        Xóa bộ lọc
      </button>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-4">
      <div v-for="doc in filteredDocuments"
           :key="doc.maTaiLieu"
           :class="doc.gimDau ? 'border-amber-300 bg-amber-50/20' : 'border-slate-200 bg-white'"
           class="p-5 rounded-2xl border shadow-xs hover:shadow-md transition duration-200 flex flex-col justify-between gap-4 relative group">
        <!-- Pinned Badge -->
        <div v-if="doc.gimDau" class="absolute top-4 right-4 bg-amber-100 text-amber-800 border border-amber-300 px-2.5 py-0.5 rounded-full text-[11px] font-bold flex items-center gap-1 shadow-2xs">
          <span class="material-symbols-outlined text-[14px]">push_pin</span> Văn bản Ghim
        </div>

        <div class="space-y-3">
          <!-- File Icon + Title -->
          <div class="flex items-start gap-3 pr-20">
            <div :class="fileTypeStyle(doc.loaiTep)"
                 class="w-11 h-11 rounded-xl flex items-center justify-center font-bold text-xs shrink-0 shadow-2xs">
              {{ doc.loaiTep.toUpperCase() }}
            </div>
            <div>
              <h3 class="font-bold text-slate-900 text-base leading-snug group-hover:text-blue-600 transition">
                {{ doc.tenTaiLieu }}
              </h3>
              <p class="text-xs text-slate-500 mt-1 flex flex-wrap items-center gap-2">
                <span>Tác giả: <strong>{{ doc.tacGia }}</strong></span>
                <span>•</span>
                <span>{{ doc.ngayDang }}</span>
                <span v-if="doc.namHoc" class="ml-1 px-2 py-0.5 bg-indigo-50 text-indigo-700 border border-indigo-200 rounded-md font-bold text-[10px] inline-flex items-center gap-1">
                  <span class="material-symbols-outlined text-[12px]">school</span> {{ doc.namHoc }}
                </span>
              </p>
            </div>
          </div>

          <!-- Description -->
          <p class="text-xs text-slate-600 line-clamp-2 leading-relaxed bg-slate-50/80 p-2.5 rounded-xl border border-slate-100">
            {{ doc.moTa }}
          </p>

          <!-- Tags -->
          <div class="flex flex-wrap items-center gap-1.5 pt-1">
            <span v-for="t in doc.tags"
                  :key="t"
                  class="px-2 py-0.5 bg-slate-100 text-slate-600 rounded-md text-[10px] font-medium">
              #{{ t }}
            </span>
          </div>
        </div>

        <!-- Footer Actions -->
        <div class="pt-3 border-t border-slate-100 flex items-center justify-between">
          <div class="text-xs text-slate-500 font-medium flex items-center gap-3">
            <span class="flex items-center gap-1"><span class="material-symbols-outlined text-[16px]">folder</span> {{ categoryLabel(doc.danhMuc) }}</span>
            <span class="flex items-center gap-1"><span class="material-symbols-outlined text-[16px]">hard_drive</span> {{ doc.dungLuong }}</span>
            <span class="flex items-center gap-1"><span class="material-symbols-outlined text-[16px]">download</span> {{ doc.luotTai }} lượt</span>
          </div>

          <div class="flex items-center gap-2">
            <button @click="previewDocument(doc)"
                    class="px-3 py-1.5 text-slate-700 bg-slate-100 hover:bg-slate-200 font-semibold rounded-xl text-xs transition flex items-center gap-1">
              <span class="material-symbols-outlined text-[16px]">visibility</span> Xem
            </button>
            <button @click="downloadDocument(doc)"
                    class="px-3.5 py-1.5 text-white bg-blue-600 hover:bg-blue-700 font-bold rounded-xl text-xs transition shadow-xs flex items-center gap-1">
              <span class="material-symbols-outlined text-[16px]">download</span> Tải về
            </button>
            <button v-if="canDelete"
                    @click="deleteDocument(doc)"
                    class="p-1.5 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition"
                    title="Xóa tài liệu">
              <span class="material-symbols-outlined text-[18px]">delete</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Upload Modal -->
    <div v-if="showUploadModal"
         class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/50 backdrop-blur-xs p-4"
         @click.self="showUploadModal = false">
      <div class="w-full max-w-lg bg-white rounded-2xl shadow-2xl border border-slate-200 p-6 space-y-4">
        <div class="flex items-center justify-between border-b pb-3">
          <h3 class="font-bold text-lg text-slate-900 flex items-center gap-2">
            <span class="material-symbols-outlined text-blue-600">upload_file</span> Tải lên Tài liệu Mới
          </h3>
          <button @click="showUploadModal = false" class="text-slate-400 hover:text-slate-600">✕</button>
        </div>

        <form @submit.prevent="handleUpload" class="space-y-4">
          <div>
            <label class="block text-xs font-bold text-slate-700 mb-1">Tên tài liệu / Tên biểu mẫu *</label>
            <input v-model="uploadForm.tenTaiLieu"
                   type="text"
                   required
                   placeholder="VD: Mẫu Đơn xin Thực tập Tốt nghiệp 2026"
                   class="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 text-xs outline-none focus:ring-2 focus:ring-blue-600" />
          </div>

          <div class="grid grid-cols-3 gap-3">
            <div>
              <label class="block text-xs font-bold text-slate-700 mb-1">Danh mục *</label>
              <select v-model="uploadForm.danhMuc"
                      required
                      class="w-full px-3 py-2 rounded-xl border border-slate-300 text-xs outline-none focus:ring-2 focus:ring-blue-600">
                <option value="BieuMau">Biểu mẫu & Mẫu đơn</option>
                <option value="QuyDinh">Quy định & Quy chế</option>
                <option value="HuongDan">Hướng dẫn & Sổ tay</option>
                <option value="BaoCaoMau">Báo cáo mẫu</option>
              </select>
            </div>

            <div>
              <label class="block text-xs font-bold text-slate-700 mb-1">Định dạng tập tin *</label>
              <select v-model="uploadForm.loaiTep"
                      required
                      class="w-full px-3 py-2 rounded-xl border border-slate-300 text-xs outline-none focus:ring-2 focus:ring-blue-600">
                <option value="pdf">PDF (.pdf)</option>
                <option value="docx">Word (.docx)</option>
                <option value="pptx">PowerPoint (.pptx)</option>
                <option value="xlsx">Excel (.xlsx)</option>
              </select>
            </div>

            <div>
              <label class="block text-xs font-bold text-slate-700 mb-1">Năm học *</label>
              <select v-model="uploadForm.namHoc"
                      required
                      class="w-full px-3 py-2 rounded-xl border border-slate-300 text-xs outline-none focus:ring-2 focus:ring-blue-600 font-medium">
                <option value="2025-2026">2025 - 2026</option>
                <option value="2024-2025">2024 - 2025</option>
                <option value="2023-2024">2023 - 2024</option>
              </select>
            </div>
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-700 mb-1">Mô tả ngắn về tài liệu</label>
            <textarea v-model="uploadForm.moTa"
                      rows="3"
                      placeholder="Mô tả nội dung, mục đích sử dụng hoặc đối tượng áp dụng..."
                      class="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 text-xs outline-none focus:ring-2 focus:ring-blue-600"></textarea>
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-700 mb-1">Thẻ từ khóa (Tags, cách nhau bằng dấu phẩy)</label>
            <input v-model="uploadForm.tags"
                   type="text"
                   placeholder="VD: Sinh viên, Biểu mẫu, Quy định"
                   class="w-full px-3.5 py-2.5 rounded-xl border border-slate-300 text-xs outline-none focus:ring-2 focus:ring-blue-600" />
          </div>

          <div class="flex items-center gap-2 pt-1">
            <input v-model="uploadForm.gimDau" type="checkbox" id="gimDauCheck" class="rounded text-blue-600 focus:ring-blue-600" />
            <label for="gimDauCheck" class="text-xs font-semibold text-slate-700 cursor-pointer">
              Ghim tài liệu này lên đầu trang (Ưu tiên hiển thị)
            </label>
          </div>

          <div class="border-2 border-dashed border-slate-200 rounded-xl p-4 text-center bg-slate-50 hover:bg-slate-100 transition cursor-pointer">
            <span class="material-symbols-outlined text-[32px] text-blue-600">cloud_upload</span>
            <p class="text-xs font-semibold text-slate-700 mt-1">Chọn tệp đính kèm từ máy tính</p>
            <p class="text-[10px] text-slate-400">Chấp nhận .pdf, .docx, .xlsx (Dung lượng tối đa 25MB)</p>
          </div>

          <div class="flex justify-end gap-3 pt-2">
            <button type="button"
                    @click="showUploadModal = false"
                    class="px-4 py-2 rounded-xl text-xs font-semibold text-slate-600 hover:bg-slate-100">
              Hủy
            </button>
            <button type="submit"
                    :disabled="uploading"
                    class="px-5 py-2 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl text-xs shadow-md transition disabled:opacity-50">
              {{ uploading ? 'Đang tải lên...' : 'Tải lên ngay' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Preview Modal -->
    <div v-if="selectedDocPreview"
         class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/60 backdrop-blur-xs p-4"
         @click.self="selectedDocPreview = null">
      <div class="w-full max-w-2xl bg-white rounded-2xl shadow-2xl border border-slate-200 p-6 space-y-5">
        <div class="flex items-center justify-between border-b pb-3">
          <div class="flex items-center gap-3">
            <div :class="fileTypeStyle(selectedDocPreview.loaiTep)" class="w-10 h-10 rounded-xl flex items-center justify-center font-bold text-xs shrink-0">
              {{ selectedDocPreview.loaiTep.toUpperCase() }}
            </div>
            <div>
              <h3 class="font-bold text-slate-900 text-base">{{ selectedDocPreview.tenTaiLieu }}</h3>
              <p class="text-xs text-slate-500">{{ categoryLabel(selectedDocPreview.danhMuc) }} • {{ selectedDocPreview.dungLuong }}</p>
            </div>
          </div>
          <button @click="selectedDocPreview = null" class="text-slate-400 hover:text-slate-600">✕</button>
        </div>

        <div class="space-y-3 bg-slate-50 p-4 rounded-xl border border-slate-200 text-xs text-slate-700">
          <p class="font-bold text-slate-900 text-sm">Thông tin văn bản & Nội dung xem trước:</p>
          <p class="leading-relaxed">{{ selectedDocPreview.moTa }}</p>
          <div class="grid grid-cols-2 gap-2 text-slate-600 border-t pt-2">
            <div><strong>Đơn vị phát hành:</strong> {{ selectedDocPreview.tacGia }}</div>
            <div><strong>Ngày đăng:</strong> {{ selectedDocPreview.ngayDang }}</div>
            <div><strong>Lượt tải về:</strong> {{ selectedDocPreview.luotTai }} lượt</div>
            <div><strong>Mã tài liệu:</strong> {{ selectedDocPreview.maTaiLieu }}</div>
          </div>
        </div>

        <div class="flex justify-end gap-3 pt-2 border-t">
          <button @click="selectedDocPreview = null"
                  class="px-4 py-2 rounded-xl text-xs font-semibold text-slate-600 hover:bg-slate-100">
            Đóng
          </button>
          <button @click="downloadDocument(selectedDocPreview)"
                  class="px-5 py-2 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl text-xs shadow-md transition flex items-center gap-1.5">
            <span class="material-symbols-outlined text-[16px]">download</span> Tải xuống tệp tin
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import api from '@/api/api';

const authStore = useAuthStore();
const documents = ref([]);
const loading = ref(true);
const searchQuery = ref('');
const selectedCategory = ref('ALL');
const selectedFileType = ref('ALL');
const selectedAcademicYear = ref('ALL');
const selectedTag = ref('');

const showUploadModal = ref(false);
const selectedDocPreview = ref(null);
const uploading = ref(false);

const uploadForm = ref({
  tenTaiLieu: '',
  danhMuc: 'BieuMau',
  loaiTep: 'docx',
  namHoc: '2025-2026',
  moTa: '',
  tags: '',
  gimDau: false
});

const canUpload = computed(() => {
  const role = authStore.user?.vaiTro;
  return role === 'Admin' || role === 'GiaoVu' || role === 'GiangVien';
});

const canDelete = computed(() => {
  const role = authStore.user?.vaiTro;
  return role === 'Admin' || role === 'GiaoVu';
});

const categories = [
  { value: 'ALL', label: 'Tất cả tài liệu', icon: 'grid_view' },
  { value: 'BieuMau', label: 'Biểu mẫu & Mẫu đơn', icon: 'assignment' },
  { value: 'QuyDinh', label: 'Quy định & Quy chế', icon: 'gavel' },
  { value: 'HuongDan', label: 'Hướng dẫn & Sổ tay', icon: 'menu_book' },
  { value: 'BaoCaoMau', label: 'Báo cáo mẫu', icon: 'description' }
];

const fileTypeOptions = [
  { value: 'ALL', label: 'Tất cả tệp', activeClass: 'bg-slate-800 text-white shadow-xs', badge: null },
  { value: 'pdf', label: 'PDF', activeClass: 'bg-rose-600 text-white shadow-xs', badge: 'bg-rose-400' },
  { value: 'docx', label: 'DOCX', activeClass: 'bg-blue-600 text-white shadow-xs', badge: 'bg-blue-400' },
  { value: 'pptx', label: 'PPTX', activeClass: 'bg-amber-600 text-white shadow-xs', badge: 'bg-amber-400' },
  { value: 'xlsx', label: 'XLSX', activeClass: 'bg-emerald-600 text-white shadow-xs', badge: 'bg-emerald-400' }
];

const popularTags = ['Sinh viên', 'Biểu mẫu', 'Doanh nghiệp', 'Quy chế', 'Báo cáo mẫu', 'Chấm điểm', 'Slide'];

const hasActiveFilters = computed(() => {
  return (
    searchQuery.value !== '' ||
    selectedCategory.value !== 'ALL' ||
    selectedFileType.value !== 'ALL' ||
    selectedAcademicYear.value !== 'ALL' ||
    selectedTag.value !== ''
  );
});

async function fetchDocuments() {
  loading.value = true;
  try {
    const res = await api.get('/documents');
    documents.value = res.data;
  } catch (err) {
    console.error('Fetch documents error:', err);
  } finally {
    loading.value = false;
  }
}

const pinnedCount = computed(() => documents.value.filter(d => d.gimDau).length);
const totalDownloads = computed(() => documents.value.reduce((acc, d) => acc + (d.luotTai || 0), 0));
const formCount = computed(() => documents.value.filter(d => d.danhMuc === 'BieuMau').length);

const filteredDocuments = computed(() => {
  return documents.value.filter(doc => {
    const matchesCat = selectedCategory.value === 'ALL' || doc.danhMuc === selectedCategory.value;
    const matchesFileType =
      selectedFileType.value === 'ALL' ||
      (doc.loaiTep && doc.loaiTep.toLowerCase() === selectedFileType.value.toLowerCase());
    const matchesYear = selectedAcademicYear.value === 'ALL' || doc.namHoc === selectedAcademicYear.value;
    const matchesTag = !selectedTag.value || (doc.tags && doc.tags.includes(selectedTag.value));

    if (!searchQuery.value) return matchesCat && matchesFileType && matchesYear && matchesTag;

    const q = searchQuery.value.toLowerCase().trim();
    const matchesQuery =
      doc.tenTaiLieu.toLowerCase().includes(q) ||
      doc.moTa.toLowerCase().includes(q) ||
      doc.tacGia.toLowerCase().includes(q) ||
      (doc.namHoc && doc.namHoc.toLowerCase().includes(q)) ||
      (doc.tags && doc.tags.some(t => t.toLowerCase().includes(q)));

    return matchesCat && matchesFileType && matchesYear && matchesTag && matchesQuery;
  });
});

function fileTypeStyle(type) {
  switch (type?.toLowerCase()) {
    case 'pdf':
      return 'bg-rose-100 text-rose-700 border border-rose-200';
    case 'docx':
    case 'doc':
      return 'bg-blue-100 text-blue-700 border border-blue-200';
    case 'pptx':
    case 'ppt':
      return 'bg-amber-100 text-amber-700 border border-amber-200';
    case 'xlsx':
    case 'xls':
      return 'bg-emerald-100 text-emerald-700 border border-emerald-200';
    default:
      return 'bg-slate-100 text-slate-700 border border-slate-200';
  }
}

function categoryLabel(cat) {
  const found = categories.find(c => c.value === cat);
  return found ? found.label : 'Tài liệu';
}

function toggleTag(tag) {
  if (selectedTag.value === tag) {
    selectedTag.value = '';
  } else {
    selectedTag.value = tag;
  }
}

function resetFilters() {
  searchQuery.value = '';
  selectedCategory.value = 'ALL';
  selectedFileType.value = 'ALL';
  selectedAcademicYear.value = 'ALL';
  selectedTag.value = '';
}

function previewDocument(doc) {
  selectedDocPreview.value = doc;
}

async function downloadDocument(doc) {
  try {
    await api.post(`/documents/${doc.maTaiLieu}/download`);
    doc.luotTai += 1;
  } catch (err) {
    console.error('Download log error:', err);
  }

  // Create mock download link
  const blob = new Blob([`Tài liệu: ${doc.tenTaiLieu}\nNội dung văn bản thử nghiệm thuộc hệ thống IMS - Trường Đại học Kinh tế (DUE).\nMô tả: ${doc.moTa}`], { type: 'text/plain;charset=utf-8' });
  const link = document.createElement('a');
  link.href = URL.createObjectURL(blob);
  link.download = `${doc.tenTaiLieu}.${doc.loaiTep === 'docx' ? 'txt' : doc.loaiTep}`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
}

async function deleteDocument(doc) {
  if (!confirm(`Bạn có chắc chắn muốn xóa tài liệu "${doc.tenTaiLieu}" không?`)) return;
  try {
    await api.delete(`/documents/${doc.maTaiLieu}`);
    documents.value = documents.value.filter(d => d.maTaiLieu !== doc.maTaiLieu);
  } catch (err) {
    alert(err.response?.data?.message || 'Có lỗi khi xóa tài liệu');
  }
}

async function handleUpload() {
  uploading.value = true;
  try {
    const res = await api.post('/documents', uploadForm.value);
    documents.value.unshift(res.data.document);
    showUploadModal.value = false;
    uploadForm.value = { tenTaiLieu: '', danhMuc: 'BieuMau', loaiTep: 'docx', namHoc: '2025-2026', moTa: '', tags: '', gimDau: false };
    alert('Đã tải tài liệu lên thành công!');
  } catch (err) {
    alert(err.response?.data?.message || 'Lỗi khi tải lên tài liệu');
  } finally {
    uploading.value = false;
  }
}

onMounted(() => {
  fetchDocuments();
});
</script>
