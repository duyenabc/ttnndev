<template>
  <el-container class="layout-container">
    <el-aside width="200px" class="aside">
      <div class="logo">IMS Management</div>

      <el-menu router :default-active="$route.path" class="custom-menu">
        <el-menu-item index="/dashboard">
          <el-icon><Monitor /></el-icon> <span>Tổng quan</span>
        </el-menu-item>

        <el-menu-item v-if="userRole === 'SinhVien'" index="/diaries">
          <el-icon><Document /></el-icon> <span>Nhật ký thực tập</span>
        </el-menu-item>

        <el-menu-item v-if="userRole === 'GiangVien'" index="/scores">
          <el-icon><Star /></el-icon> <span>Quản lý điểm số</span>
        </el-menu-item>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header class="header">
        <span style="margin-right: 15px;">Xin chào, {{ authStore.user?.hoTen }}</span>
        <el-button link @click="handleLogout">Đăng xuất</el-button>
      </el-header>
      <el-main>
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
  import { computed } from 'vue';
  import { Monitor, Document, Star } from '@element-plus/icons-vue';
  import { useAuthStore } from '@/stores/auth';
  import { useRouter } from 'vue-router';

  const authStore = useAuthStore();
  const router = useRouter();

  // Lấy role từ store một cách an toàn
  const userRole = computed(() => authStore.user?.vaiTro);

  const handleLogout = () => {
    authStore.logout();
  };
</script>

<style scoped>
  .layout-container {
    height: 100vh;
  }

  .aside {
    background-color: #304156;
    color: white;
  }

  .logo {
    padding: 20px;
    text-align: center;
    font-weight: bold;
    color: white;
  }

  .header {
    border-bottom: 1px solid #ddd;
    display: flex;
    align-items: center;
    justify-content: flex-end;
  }

  .custom-menu {
    border: none;
    background-color: #304156;
  }

  .el-menu-item {
    color: #bfcbd9;
  }

    .el-menu-item.is-active {
      background-color: #263445 !important;
      color: #409eff;
    }
</style>
