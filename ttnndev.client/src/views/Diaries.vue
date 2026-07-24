<template>
  <div class="diaries-container">
    <el-card shadow="never" class="box-card">
      <template #header>
        <div class="card-header">
          <h2>Nhật ký thực tập của tôi</h2>
          <el-button type="primary" @click="openCreateDialog">
            <el-icon><Plus /></el-icon> Viết nhật ký tuần này
          </el-button>
        </div>
</template>

      <el-timeline v-if="diaries.length > 0" style="max-width: 800px; margin-top: 20px;">
        <el-timeline-item v-for="(diary, index) in diaries"
                          :key="index"
                          :timestamp="'Tuần ' + diary.tuanThucTap + ' - ' + formatDate(diary.ngayTao)"
                          placement="top"
                          type="primary">
          <el-card shadow="hover">
            <h3>Nội dung công việc:</h3>
            <p>{{ diary.noiDungCongViec }}</p>

            <el-divider border-style="dashed" />

            <div class="diary-details">
              <p><strong>Kết quả đạt được:</strong> {{ diary.ketQuaDatDuoc || 'Không có' }}</p>
              <p><strong>Khó khăn:</strong> {{ diary.khoKhanVongMac || 'Không có' }}</p>
              <p><strong>Kế hoạch tuần tới:</strong> {{ diary.keHoachTuanToi || 'Không có' }}</p>
            </div>

            <el-button v-if="diary.minhChung" type="info" link size="small" style="margin-top: 10px;">
              <el-icon><Paperclip /></el-icon> Xem minh chứng
            </el-button>
          </el-card>
        </el-timeline-item>
      </el-timeline>

      <el-empty v-else description="Bạn chưa có bài nhật ký nào. Hãy tạo mới!" />
    </el-card>

    <el-dialog v-model="dialogVisible" title="Viết nhật ký thực tập" width="600px">
      <el-form :model="diaryForm" :rules="rules" ref="formRef" label-position="top">
        <el-form-item label="Tuần thực tập số" prop="tuanThucTap">
          <el-input-number v-model="diaryForm.tuanThucTap" :min="1" :max="15" />
        </el-form-item>

        <el-form-item label="Nội dung công việc đã làm" prop="noiDungCongViec">
          <el-input v-model="diaryForm.noiDungCongViec" type="textarea" :rows="3" placeholder="Mô tả chi tiết công việc..." />
        </el-form-item>

        <el-form-item label="Kết quả đạt được">
          <el-input v-model="diaryForm.ketQuaDatDuoc" type="textarea" :rows="2" />
        </el-form-item>

        <el-form-item label="Khó khăn / Vướng mắc (nếu có)">
          <el-input v-model="diaryForm.khoKhanVongMac" type="textarea" :rows="2" />
        </el-form-item>

        <el-form-item label="Kế hoạch tuần tới">
          <el-input v-model="diaryForm.keHoachTuanToi" type="textarea" :rows="2" />
        </el-form-item>
      </el-form>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">Hủy</el-button>
          <el-button type="primary" @click="submitDiary" :loading="isSubmitting">Nộp nhật ký</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import { Plus, Paperclip } from '@element-plus/icons-vue';
  import { ElMessage } from 'element-plus';
  import api from '@/api/api';

  const diaries = ref([]);
  const dialogVisible = ref(false);
  const isSubmitting = ref(false);
  const formRef = ref(null);

  // Form data maps with NhatKy.cs Model in Backend
  const diaryForm = ref({
    tuanThucTap: 1,
    noiDungCongViec: '',
    ketQuaDatDuoc: '',
    khoKhanVongMac: '',
    keHoachTuanToi: '',
    minhChung: ''
  });

  const rules = {
    tuanThucTap: [{ required: true, message: 'Vui lòng chọn tuần', trigger: 'blur' }],
    noiDungCongViec: [{ required: true, message: 'Vui lòng nhập nội dung công việc', trigger: 'blur' }]
  };

  // Format ngày tháng cho Timeline
  const formatDate = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  };

  // Gọi API lấy danh sách nhật ký
  const fetchDiaries = async () => {
    try {
      // API GET này cần được viết trong DiaryController.cs
      const res = await api.get('/diary/my-diaries');
      diaries.value = res.data;
    } catch (error) {
      console.error(error);
      // Tạm thời dùng dữ liệu giả nếu API chưa sẵn sàng
      diaries.value = [
        { tuanThucTap: 1, ngayTao: new Date().toISOString(), noiDungCongViec: 'Làm quen môi trường, setup máy tính.', ketQuaDatDuoc: 'Cài xong VS Code, chạy được source code.', khoKhanVongMac: '', keHoachTuanToi: 'Đọc hiểu logic hệ thống.' }
      ];
    }
  };

  const openCreateDialog = () => {
    diaryForm.value = { tuanThucTap: diaries.value.length + 1, noiDungCongViec: '', ketQuaDatDuoc: '', khoKhanVongMac: '', keHoachTuanToi: '', minhChung: '' };
    dialogVisible.value = true;
  };

  // Submit dữ liệu lên Backend
  const submitDiary = async () => {
    if (!formRef.value) return;
    await formRef.value.validate(async (valid) => {
      if (valid) {
        isSubmitting.value = true;
        try {
          // API POST này tương ứng với DiaryController.cs
          await api.post('/diary', diaryForm.value);
          ElMessage.success('Nộp nhật ký thành công!');
          dialogVisible.value = false;
          fetchDiaries(); // Lấy lại danh sách mới
        } catch {
          ElMessage.error('Có lỗi xảy ra khi nộp!');
        } finally {
          isSubmitting.value = false;
        }
      }
    });
  };

  onMounted(() => {
    fetchDiaries();
  });
</script>

<style scoped>
  .diaries-container {
    padding: 20px;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .diary-details p {
    margin: 5px 0;
    font-size: 14px;
    color: #555;
  }
</style>
