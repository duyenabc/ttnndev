<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center p-4">
    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl border border-slate-100 p-8 text-center">
      <span class="material-symbols-outlined text-[48px] text-emerald-500 mb-2">task_alt</span>
      <h1 class="text-xl font-bold text-slate-900 mb-2">Kích hoạt Tài khoản</h1>
      <p class="text-sm text-slate-500 mb-6">Thiết lập mật khẩu mới cho tài khoản của bạn</p>

      <form @submit.prevent="handleActivate" class="space-y-4 text-left">
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1">Mật khẩu mới</label>
          <input v-model="password" type="password" required class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600" />
        </div>
        <div>
          <label class="block text-xs font-semibold text-slate-700 mb-1">Xác nhận mật khẩu mới</label>
          <input v-model="confirmPassword" type="password" required class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600" />
        </div>
        <button type="submit" class="w-full py-3 bg-emerald-600 hover:bg-emerald-700 text-white font-bold rounded-xl shadow-md text-sm">
          Kích hoạt & Đăng nhập
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import api from '@/api/api';
  import { useAuthStore } from '@/stores/auth';

  const router = useRouter();
  const authStore = useAuthStore();
  const password = ref('');
  const confirmPassword = ref('');

  async function handleActivate() {
    if (password.value !== confirmPassword.value) {
      alert('Mật khẩu xác nhận không khớp');
      return;
    }
    try {
      const res = await api.post('/auth/activate', { matKhauMoi: password.value });
      authStore.setAuth(res.data);
      router.push('/dashboard');
    } catch (err) {
      alert(err.response?.data?.message || 'Có lỗi xảy ra');
    }
  }
</script>
