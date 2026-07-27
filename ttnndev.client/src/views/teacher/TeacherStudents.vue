<template>
  <div class="teacher-students-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <h2>Quản lý sinh viên & Duyệt đề tài</h2>
          <el-input v-model="searchQuery"
                    placeholder="Tìm kiếm MSSV hoặc Tên..."
                    style="width: 250px;"
                    clearable />
        </div>
      </template>

      <!-- Bảng danh sách sinh viên (E04) -->
      <el-table :data="filteredStudents" style="width: 100%" border stripe>
        <el-table-column prop="mssv" label="MSSV" width="120" />
        <el-table-column prop="hoTen" label="Họ và Tên" width="200" />

        <el-table-column label="Đơn vị thực tập" width="250">
          <template #default="scope">
            <strong>{{ scope.row.congTy }}</strong>
            <br>
            <span style="font-size: 12px; color: #888;">{{ scope.row.viTri }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="tenDeTai" label="Đề tài đề xuất" min-width="250" />

        <el-table-column label="Trạng thái đề tài" width="150" align="center">
          <template #default="scope">
            <el-tag :type="getStatusType(scope.row.trangThaiDeTai)">
              {{ scope.row.trangThaiDeTai }}
            </el-tag>
          </template>
        </el-table-column>

        <!-- Thao tác Duyệt đề tài (E05) -->
        <el-table-column label="Thao tác" width="200" align="center" fixed="right">
          <template #default="scope">
            <el-button v-if="scope.row.trangThaiDeTai === 'Chờ duyệt'"
                       type="success"
                       size="small"
                       @click="openApproveDialog(scope.row)">
              Duyệt / Từ chối
            </el-button>
            <el-button type="primary"
                       link
                       size="small"
                       @click="viewDetails(scope.row)">
              Hồ sơ chi tiết
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Dialog Duyệt đề tài -->
    <el-dialog v-model="dialogVisible" title="Xử lý đề tài thực tập" width="500px">
      <div v-if="selectedStudent">
        <p><strong>Sinh viên:</strong> {{ selectedStudent.hoTen }} ({{ selectedStudent.mssv }})</p>
        <p><strong>Đề tài:</strong> {{ selectedStudent.tenDeTai }}</p>

        <el-form label-position="top" style="margin-top: 20px;">
          <el-form-item label="Ghi chú / Nhận xét cho sinh viên">
            <el-input v-model="feedbackText"
                      type="textarea"
                      rows="4"
                      placeholder="Nhập yêu cầu chỉnh sửa hoặc lý do từ chối..." />
          </el-form-item>
        </el-form>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button type="danger" @click="handleDecision('Từ chối')">Từ chối</el-button>
          <el-button type="warning" @click="handleDecision('Yêu cầu sửa')">Yêu cầu sửa</el-button>
          <el-button type="success" @click="handleDecision('Đã duyệt')">Duyệt đề tài</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { ElMessage } from 'element-plus';

// Dữ liệu mẫu (Sẽ được thay thế bằng API call tới GhiDanhSinhVien & DeTai)
const students = ref([
  { id: 1, mssv: '49K21.2', hoTen: 'Nguyễn Văn A', congTy: 'Techcombank', viTri: 'Data Analyst', tenDeTai: 'Dự đoán rời mạng khách hàng', trangThaiDeTai: 'Chờ duyệt' },
  { id: 2, mssv: '49K21.3', hoTen: 'Trần Thị B', congTy: 'FPT Software', viTri: 'VueJS Developer', tenDeTai: 'Xây dựng hệ thống quản lý IMS', trangThaiDeTai: 'Đã duyệt' },
]);

const searchQuery = ref('');
const dialogVisible = ref(false);
const selectedStudent = ref(null);
const feedbackText = ref('');

// Lọc sinh viên theo ô tìm kiếm
const filteredStudents = computed(() => {
  return students.value.filter(s =>
    s.mssv.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    s.hoTen.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

const getStatusType = (status) => {
  const map = {
    'Chờ duyệt': 'warning',
    'Đã duyệt': 'success',
    'Từ chối': 'danger',
    'Yêu cầu sửa': 'info'
  };
  return map[status] || 'info';
};

const openApproveDialog = (student) => {
  selectedStudent.value = student;
  feedbackText.value = '';
  dialogVisible.value = true;
};

// Xử lý API Duyệt đề tài (Kết nối với DeTaiController)
const handleDecision = async (decision) => {
  try {
    // API Call giả định cập nhật trạng thái đề tài
    // await api.put(`/detai/${selectedStudent.value.id}/status`, {
    //   trangThai: decision,
    //   nhanXet: feedbackText.value
    // });

    // Cập nhật UI tạm thời
    selectedStudent.value.trangThaiDeTai = decision;
    ElMessage.success(`Đã đánh giá đề tài: ${decision}`);
    dialogVisible.value = false;
  } catch {
    ElMessage.error('Có lỗi xảy ra khi xử lý đề tài');
  }
};

const viewDetails = (student) => {
  ElMessage.info(`Đang chuyển đến hồ sơ chi tiết của ${student.hoTen}`);
  // router.push(`/students/${student.id}`);
};
</script>

<style scoped>
  .teacher-students-container {
    padding: 20px;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .dialog-footer {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
  }
</style>
