<template>
  <div class="login-container">
    <el-card class="login-card">
      <h2>Đăng nhập IMS</h2>
      <el-form :model="loginForm" label-position="top">
        <el-form-item label="Mã định danh">
          <el-input v-model="loginForm.maDinhDanh" placeholder="Nhập mã sinh viên/giảng viên" />
        </el-form-item>
        <el-form-item label="Mật khẩu">
          <el-input v-model="loginForm.matKhau" type="password" placeholder="Nhập mật khẩu" show-password />
        </el-form-item>
        <el-button type="primary" @click="handleLogin" style="width: 100%">Đăng nhập</el-button>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
  import { reactive } from 'vue';
  import { useRouter } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';

  const loginForm = reactive({ maDinhDanh: '', matKhau: '' });
  const router = useRouter();
  const authStore = useAuthStore();

  const handleLogin = async () => {
    try {
      // Sửa lỗi: Thêm dấu } đóng object và ) đóng hàm login
      await authStore.login({
        maDinhDanh: loginForm.maDinhDanh,
        matKhau: loginForm.matKhau
      });

      // Chuyển hướng sau khi login thành công
      router.push('/dashboard');
    } catch (err) {
      console.error(err);
      alert("Sai tên đăng nhập hoặc mật khẩu!");
    }
  };
</script>

<style scoped>
  .login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    background-color: #f0f2f5;
  }

  .login-card {
    width: 400px;
    padding: 20px;
  }
</style>
