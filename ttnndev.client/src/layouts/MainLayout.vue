<template>
  <el-container class="layout-container">
    <el-aside width="200px" class="aside">
      <div class="logo">IMS Management</div>
      <el-menu router :default-active="$route.path">
        <el-menu-item index="/dashboard">
          <el-icon><Monitor /></el-icon> Dashboard
        </el-menu-item>
        <el-menu-item index="/diaries">
          <el-icon><Document /></el-icon> Nhật ký
        </el-menu-item>
        <el-menu-item index="/scores">
          <el-icon><Star /></el-icon> Điểm số
        </el-menu-item>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header class="header">
        <el-button link @click="logout">Đăng xuất</el-button>
      </el-header>
      <el-main>
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { Monitor, Document, Star } from '@element-plus/icons-vue';

const logout = () => {
  localStorage.removeItem('token');
  window.location.href = '/login';
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

  .el-menu {
    border: none;
    background-color: #304156;
  }

  .el-menu-item {
    color: #bfcbd9;
  }

    .el-menu-item:hover {
      background-color: #263445;
      color: white;
    }
</style>
