<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center p-4">
    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl border border-slate-100 p-8">
      <div class="text-center mb-8">
        <div class="w-16 h-16 bg-blue-600 rounded-2xl flex items-center justify-center text-white mx-auto mb-4 shadow-lg shadow-blue-500/30">
          <span class="material-symbols-outlined text-[36px]">school</span>
        </div>
        <h1 class="text-2xl font-bold text-slate-900">Hệ thống Quản lý Thực tập</h1>
        <p class="text-sm text-slate-500 mt-1">Đăng nhập tài khoản trường DUE để tiếp tục</p>
      </div>

      <div v-if="errorMessage" class="mb-6 p-4 rounded-xl bg-rose-50 border border-rose-200 text-rose-700 text-sm flex items-start gap-2">
        <span class="material-symbols-outlined text-[20px] text-rose-500 shrink-0 mt-0.5">error</span>
        <span>{{ errorMessage }}</span>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-5">
        <div>
          <label class="block text-sm font-semibold text-slate-700 mb-2">Mã định danh / Tên đăng nhập</label>
          <div class="relative">
            <input
              v-model="form.maDinhDanh"
              type="text"
              required
              placeholder="Nhập mã sinh viên, giảng viên hoặc admin"
              class="w-full px-4 py-3 pl-11 rounded-xl border border-slate-300 focus:ring-2 focus:ring-blue-600 focus:border-transparent text-slate-900 text-sm outline-none transition"
            />
            <span class="material-symbols-outlined absolute left-3.5 top-3.5 text-slate-400 text-[20px]">person</span>
          </div>
        </div>

        <div>
          <div class="flex items-center justify-between mb-2">
            <label class="block text-sm font-semibold text-slate-700">Mật khẩu</label>
            <router-link to="/forgot-password" class="text-xs font-semibold text-blue-600 hover:underline">Quên mật khẩu?</router-link>
          </div>
          <div class="relative">
            <input
              v-model="form.matKhau"
              :type="showPassword ? 'text' : 'password'"
              required
              placeholder="Nhập mật khẩu"
              class="w-full px-4 py-3 pl-11 pr-11 rounded-xl border border-slate-300 focus:ring-2 focus:ring-blue-600 focus:border-transparent text-slate-900 text-sm outline-none transition"
            />
            <span class="material-symbols-outlined absolute left-3.5 top-3.5 text-slate-400 text-[20px]">lock</span>
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="absolute right-3.5 top-3.5 text-slate-400 hover:text-slate-600"
            >
              <span class="material-symbols-outlined text-[20px]">{{ showPassword ? 'visibility_off' : 'visibility' }}</span>
            </button>
          </div>
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full py-3.5 px-4 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-lg shadow-blue-500/25 transition duration-200 flex items-center justify-center gap-2 disabled:opacity-50"
        >
          <span v-if="loading" class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
          <span>{{ loading ? 'Đang đăng nhập...' : 'Đăng nhập' }}</span>
        </button>
      </form>

      <!-- Quick Test Login Buttons -->
      <div class="mt-8 pt-6 border-t border-slate-100">
        <p class="text-xs font-bold text-slate-400 uppercase tracking-wider text-center mb-3">Tài khoản demo thử nghiệm</p>
        <div class="grid grid-cols-2 gap-2 text-xs">
          <button @click="fillDemo('admin', 'Admin@123')" class="p-2.5 rounded-lg border border-slate-200 hover:bg-slate-50 text-slate-700 font-medium text-left">
            👑 <span class="font-bold">Admin</span> (admin)
          </button>
          <button @click="fillDemo('gvu001', 'Test@1234')" class="p-2.5 rounded-lg border border-slate-200 hover:bg-slate-50 text-slate-700 font-medium text-left">
            📚 <span class="font-bold">Giáo vụ</span> (gvu001)
          </button>
          <button @click="fillDemo('gv001', 'Test@1234')" class="p-2.5 rounded-lg border border-slate-200 hover:bg-slate-50 text-slate-700 font-medium text-left">
            👨‍🏫 <span class="font-bold">Giảng viên</span> (gv001)
          </button>
          <button @click="fillDemo('sv001', 'Test@1234')" class="p-2.5 rounded-lg border border-slate-200 hover:bg-slate-50 text-slate-700 font-medium text-left">
            🎓 <span class="font-bold">Sinh viên</span> (sv001)
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const authStore = useAuthStore();

const form = ref({ maDinhDanh: '', matKhau: '' });
const showPassword = ref(false);
const loading = ref(false);
const errorMessage = ref('');

function fillDemo(user, pass) {
  form.value.maDinhDanh = user;
  form.value.matKhau = pass;
}

async function handleLogin() {
  errorMessage.value = '';
  loading.value = true;
  try {
    const res = await authStore.login(form.value);
    if (res.redirectTo) {
      router.push(res.redirectTo);
    } else {
      router.push('/dashboard');
    }
  } catch (err) {
    errorMessage.value = err.response?.data?.message || 'Đăng nhập không thành công. Vui lòng kiểm tra lại.';
  } finally {
    loading.value = false;
  }
}
</script>
