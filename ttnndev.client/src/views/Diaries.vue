<template>
  <div class="diary-container">
    <div class="header-actions">
      <h2>Nhật ký thực tập</h2>
      <el-button type="primary" @click="dialogVisible = true">+ Nộp nhật ký mới</el-button>
    </div>

    <!-- Bảng danh sách nhật ký -->
    <el-table :data="diaries" style="width: 100%" v-loading="loading">
      <el-table-column prop="tuan" label="Tuần" width="80" />
      <el-table-column prop="ngayThucHien" label="Ngày thực hiện" width="150" />
      <el-table-column prop="congViecDaLam" label="Công việc" />
      <el-table-column prop="trangThaiDuyet" label="Trạng thái" width="120">
        <template #default="scope">
          <el-tag :type="scope.row.trangThaiDuyet === 'DaDuyet' ? 'success' : 'warning'">
            {{ scope.row.trangThaiDuyet }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>

    <!-- Dialog Form nộp nhật ký -->
    <el-dialog v-model="dialogVisible" title="Nộp nhật ký tuần" width="500px">
      <el-form :model="form">
        <el-form-item label="Tuần số">
          <el-input-number v-model="form.tuan" :min="1" :max="20" />
        </el-form-item>
        <el-form-item label="Công việc đã làm">
          <el-input v-model="form.congViecDaLam" type="textarea" />
        </el-form-item>
        <el-form-item label="Khó khăn">
          <el-input v-model="form.khoKhan" type="textarea" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">Hủy</el-button>
        <el-button type="primary" @click="submitDiary">Gửi</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import api from '@/api/api'; // Đảm bảo đường dẫn import đúng

  const diaries = ref([]);
  const loading = ref(false);
  const dialogVisible = ref(false);

  const form = ref({
    tuan: 1,
    congViecDaLam: '',
    khoKhan: '',
    maGhiDanh: 1 // Tạm để ID cứng, sau này lấy từ Pinia/Auth
  });

  const fetchDiaries = async () => {
    loading.value = true;
    try {
      // Gọi API đã viết: GetWeeklyDiary (Bạn cần chỉnh lại route API cho khớp)
      const res = await api.get('/diary/class/1/weekly/1');
      diaries.value = res.data;
    } catch (error) {
      console.error("Lỗi tải nhật ký:", error);
    } finally {
      loading.value = false;
    }
  };

  const submitDiary = async () => {
    // Tạo object payload chứa dữ liệu từ form và lấy thêm maGhiDanh từ Store
    const payload = {
      ...form.value,
      maGhiDanh: authStore.user.maGhiDanh
    };

    try {
      // Gửi payload thay vì gửi trực tiếp form.value
      await api.post('/diary/submit', payload);

      // Thành công: đóng dialog và làm mới danh sách
      dialogVisible.value = false;
      fetchDiaries();

      // Reset form để lần sau mở lên không bị sót dữ liệu cũ
      form.value = {
        tuan: 1,
        congViecDaLam: '',
        khoKhan: ''
      };

      alert("Nộp nhật ký thành công!");
    } catch (error) {
      console.error("Lỗi khi gửi nhật ký:", error);
      alert("Có lỗi xảy ra, vui lòng kiểm tra lại dữ liệu!");
    }
  };

  onMounted(fetchDiaries);
</script>

<style scoped>
  .diary-container {
    padding: 20px;
  }

  .header-actions {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
  }
</style>
