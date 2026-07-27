<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center p-4">
    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl border border-slate-100 p-8">
      <h1 class="text-xl font-bold text-slate-900 mb-2 text-center">Quên Mật khẩu</h1>
      <p class="text-sm text-slate-500 mb-6 text-center">Nhập email của bạn để nhận liên kết khôi phục</p>

      <div v-if="devLink" class="p-4 bg-emerald-50 border border-emerald-200 rounded-xl mb-4 text-xs text-emerald-800">
        <p class="font-bold mb-1">Link đặt lại mật khẩu thử nghiệm:</p>
        <router-link :to="devLink" class="text-blue-600 underline font-semibold">{{ devLink }}</router-link>
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1">Email tài khoản</label>
          <input v-model="email" type="email" required class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600" />
        </div>
        <button type="submit" class="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-md text-sm">
          Gửi yêu cầu
        </button>
      </form>

      <div class="mt-6 text-center text-xs">
        <router-link to="/login" class="text-blue-600 font-bold hover:underline">Quay lại đăng nhập</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import api from '@/api/api';

  const email = ref('');
  const devLink = ref('');

  async function handleSubmit() {
    try {
      const res = await api.post('/auth/forgot-password', { email: email.value });
      devLink.value = res.data.devResetLink;
    } catch (err) {
      alert(err.response?.data?.message || 'Lỗi');
    }
  }
</script>
