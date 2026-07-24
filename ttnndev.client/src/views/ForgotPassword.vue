<template>
  <div class="ims-scope flex min-h-screen items-center justify-center bg-surface px-4 text-on-surface">
    <div class="w-full max-w-[440px] bg-white rounded-2xl shadow-lg border border-outline-variant p-8">
      <div class="text-center mb-6">
        <div class="w-16 h-16 rounded-full bg-primary/10 flex items-center justify-center mx-auto mb-4">
          <span class="material-symbols-outlined text-primary text-[36px]">lock_reset</span>
        </div>
        <h1 class="font-title-lg text-2xl font-bold">Quên mật khẩu</h1>
        <p class="text-body-md text-on-surface-variant mt-1">Nhập email đã đăng ký để nhận link đặt lại mật khẩu.</p>
      </div>

      <div v-if="successMsg" class="mb-4 rounded-lg bg-green-50 border border-green-200 text-green-700 text-body-sm p-3">
        {{ successMsg }}
      </div>

      <form v-if="!successMsg" class="space-y-5" @submit.prevent="submit">
        <div class="relative">
          <span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-outline text-[20px]">mail</span>
          <input
            v-model="email"
            type="email"
            required
            placeholder="Email"
            class="w-full pl-12 pr-4 py-4 bg-white border border-outline-variant rounded-lg font-body-md focus:ring-2 focus:ring-blue-500/20 focus:border-primary outline-none transition-all placeholder:text-outline"
          />
        </div>

        <p v-if="errorMsg" class="text-error text-body-sm text-center">{{ errorMsg }}</p>

        <button
          type="submit"
          :disabled="isSubmitting"
          class="w-full text-white py-3.5 px-6 rounded-lg font-semibold hover:bg-blue-800 transition-all flex items-center justify-center gap-2 shadow-md disabled:opacity-70"
          style="background-color:#005ea3;"
        >
          <span v-if="isSubmitting" class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
          {{ isSubmitting ? 'Đang gửi...' : 'Gửi link đặt lại' }}
        </button>
      </form>

      <!-- Dev: hiển thị link reset khi chưa cấu hình email -->
      <div v-if="devLink" class="mt-4 rounded-lg bg-amber-50 border border-amber-200 p-3 text-body-sm">
        <p class="font-medium text-amber-800 mb-1">Link đặt lại (môi trường dev):</p>
        <router-link :to="devLink" class="text-blue-600 break-all hover:underline">{{ devLink }}</router-link>
      </div>

      <div class="text-center mt-6">
        <router-link to="/login" class="text-body-sm font-medium hover:underline" style="color:#005ea3;">← Quay lại đăng nhập</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import api from '@/api/api';

  const email = ref('');
  const isSubmitting = ref(false);
  const errorMsg = ref('');
  const successMsg = ref('');
  const devLink = ref('');

  const submit = async () => {
    errorMsg.value = '';
    devLink.value = '';
    isSubmitting.value = true;
    try {
      const res = await api.post('/auth/forgot-password', { email: email.value });
      successMsg.value = res.data.message;
      if (res.data.devResetLink) devLink.value = res.data.devResetLink;
    } catch (err) {
      errorMsg.value = err.response?.data?.message || 'Có lỗi xảy ra. Vui lòng thử lại.';
    } finally {
      isSubmitting.value = false;
    }
  };
</script>
