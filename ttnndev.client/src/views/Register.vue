<template>
  <div class="register-container">
    <el-card class="register-card">
      <h2>Đăng ký tài khoản</h2>
      <el-form :model="registerForm" label-position="top">
        <el-form-item label="Mã định danh (MSSV/MSGV)">
          <el-input v-model="registerForm.maDinhDanh" />
        </el-form-item>

        <el-form-item label="Họ và tên">
          <el-input v-model="registerForm.hoTen" />
        </el-form-item>

        <el-form-item label="Email">
          <el-input v-model="registerForm.email" type="email" />
        </el-form-item>

        <el-form-item label="Mật khẩu">
          <el-input v-model="registerForm.matKhau" type="password" show-password />
        </el-form-item>

        <el-form-item label="Vai trò">
          <el-select v-model="registerForm.vaiTro" placeholder="Chọn vai trò">
            <el-option label="Sinh viên" value="SinhVien" />
            <el-option label="Giảng viên" value="GiangVien" />
          </el-select>
        </el-form-item>

        <el-button type="primary" style="width: 100%" @click="handleRegister">
          Đăng ký
        </el-button>

        <p class="login-link">
          Đã có tài khoản? <router-link to="/login">Đăng nhập ngay</router-link>
        </p>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
  import { reactive } from 'vue';
  import { useRouter } from 'vue-router';
  import axios from 'axios'; // Đảm bảo bạn đã import axios hoặc instance api của bạn

  const router = useRouter();
  const registerForm = reactive({
    maDinhDanh: '',
    hoTen: '',
    email: '',
    matKhau: '',
    vaiTro: 'SinhVien'
  });

  const handleRegister = async () => {
    try {
      // SỬA DÒNG NÀY: Thêm /api/account/ vào đường dẫn
      await axios.post('http://localhost:5097/api/account/register', registerForm);

      alert("Đăng ký thành công!");
      router.push('/login');
    } catch (err) {
      // Để biết lỗi cụ thể (ví dụ 400 Bad Request, 500 Server Error), hãy xem console
      console.error(err);
      alert("Đăng ký thất bại: " + (err.response?.data || "Có lỗi xảy ra"));
    }
  };
</script>

<style scoped>
  .register-container {
    display: flex;
    justify-content: center;
    padding-top: 50px;
  }

  .register-card {
    width: 400px;
  }

  .login-link {
    text-align: center;
    margin-top: 15px;
    font-size: 14px;
  }
</style>
