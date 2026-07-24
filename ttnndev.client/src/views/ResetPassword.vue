<template>
  <div class="ims-scope flex min-h-screen items-center justify-center bg-surface px-4 text-on-surface">
    <div class="w-full max-w-[440px] bg-white rounded-2xl shadow-lg border border-outline-variant p-8">
      <div class="text-center mb-6">
        <div class="w-16 h-16 rounded-full bg-primary/10 flex items-center justify-center mx-auto mb-4">
          <span class="material-symbols-outlined text-primary text-[36px]">password</span>
        </div>
        <h1 class="font-title-lg text-2xl font-bold">Đặt lại mật khẩu</h1>
        <p class="text-body-md text-on-surface-variant mt-1">Nhập mật khẩu mới cho tài khoản của bạn.</p>
      </div>

      <div v-if="expired" class="rounded-lg bg-red-50 border border-red-200 text-red-700 text-body-sm p-4 mb-4 text-center">
        <p class="mb-3">Link đã hết hạn.</p>
        <router-link to="/forgot-password" class="inline-block px-4 py-2 rounded-lg text-white text-body-sm font-medium" style="background-color:#005ea3;">
          Gửi lại link đặt lại
        </router-link>
      </div>

      <div v-else-if="successMsg" class="rounded-lg bg-green-50 border border-green-200 text-green-700 text-body-sm p-4 text-center">
        {{ successMsg }} Đang chuyển đến trang đăng nhập...
      </div>

      <form v-else class="space-y-5" @submit.prevent="submit">
        <PasswordInput v-model="matKhauMoi" placeholder="Mật khẩu mới" />
        <PasswordInput v-model="xacNhan" placeholder="Xác nhận mật khẩu" />
        <p class="text-[12px] text-on-surface-variant">Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số.</p>
        <p v-if="errorMsg" class="text-error text-body-sm text-center">{{ errorMsg }}</p>
        <button
          type="submit"
          :disabled="isSubmitting"
          class="w-full text-white py-3.5 px-6 rounded-lg font-semibold hover:bg-blue-800 transition-all flex items-center justify-center gap-2 shadow-md disabled:opacity-70"
          style="background-color:#005ea3;"
        >
          <span v-if="isSubmitting" class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
          {{ isSubmitting ? 'Đang xử lý...' : 'Đặt lại mật khẩu' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import api from '@/api/api';
  import { validatePassword } from '@/utils/password';
  import PasswordInput from '@/components/PasswordInput.vue';

  const route = useRoute();
  const router = useRouter();
  const token = route.query.token;

  const matKhauMoi = ref('');
  const xacNhan = ref('');
  const isSubmitting = ref(false);
  const errorMsg = ref('');
  const successMsg = ref('');
  const expired = ref(false);

  const submit = async () => {
    errorMsg.value = '';
    if (!validatePassword(matKhauMoi.value)) {
      errorMsg.value = 'Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số';
      return;
    }
    if (matKhauMoi.value !== xacNhan.value) {
      errorMsg.value = 'Mật khẩu xác nhận không khớp';
      return;
    }
    isSubmitting.value = true;
    try {
      const res = await api.post('/auth/reset-password', {
        token,
        matKhauMoi: matKhauMoi.value,
        xacNhanMatKhau: xacNhan.value
      });
      successMsg.value = res.data.message || 'Đặt lại mật khẩu thành công';
      setTimeout(() => router.push('/login'), 1500);
    } catch (err) {
      const msg = err.response?.data?.message || 'Có lỗi xảy ra.';
      if (msg.includes('hết hạn')) expired.value = true;
      else errorMsg.value = msg;
    } finally {
      isSubmitting.value = false;
    }
  };
</script>
