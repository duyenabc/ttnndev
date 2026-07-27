<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center p-4">
    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl border border-slate-100 p-8">
      <h1 class="text-xl font-bold text-slate-900 mb-2 text-center">Đặt lại Mật khẩu</h1>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1">Mật khẩu mới</label>
          <input v-model="pass" type="password" required class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600" />
        </div>
        <button type="submit" class="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-md text-sm">
          Lưu mật khẩu mới
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import api from '@/api/api';

  const router = useRouter();
  const pass = ref('');

  async function handleSubmit() {
    try {
      await api.post('/auth/reset-password', { matKhauMoi: pass.value });
      alert('Thành công! Hãy đăng nhập lại.');
      router.push('/login');
    } catch (err) {
      alert(err.response?.data?.message || 'Lỗi');
    }
  }
</script>
