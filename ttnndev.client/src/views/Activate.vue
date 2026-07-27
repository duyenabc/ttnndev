<template>
  <div class="ims-scope flex min-h-screen items-center justify-center bg-surface px-4 text-on-surface">
    <div class="w-full max-w-[460px] bg-white rounded-2xl shadow-lg border border-outline-variant p-8">
      <div class="text-center mb-6">
        <div class="w-16 h-16 rounded-full bg-primary/10 flex items-center justify-center mx-auto mb-4">
          <span class="material-symbols-outlined text-primary text-[36px]">verified_user</span>
        </div>
        <h1 class="font-title-lg text-2xl font-bold">Kích hoạt tài khoản</h1>
        <p class="text-body-md text-on-surface-variant mt-1">Đặt mật khẩu để bắt đầu sử dụng hệ thống.</p>
      </div>

      <div v-if="loading" class="text-center py-8 text-on-surface-variant">
        <span class="material-symbols-outlined animate-spin text-[32px]">progress_activity</span>
      </div>

      <!-- Link lỗi -->
      <div v-else-if="loadError" class="rounded-lg bg-red-50 border border-red-200 text-red-700 text-body-sm p-4 text-center">
        <p class="mb-3">{{ loadError }}</p>
        <button
          v-if="canResend"
          class="inline-flex items-center gap-2 px-4 py-2 rounded-lg text-white text-body-sm font-medium"
          style="background-color:#005ea3;"
          @click="resend"
        >
          Gửi lại link kích hoạt
        </button>
        <router-link v-else to="/login" class="text-blue-600 hover:underline">Về trang đăng nhập</router-link>
      </div>

      <div v-else-if="successMsg" class="rounded-lg bg-green-50 border border-green-200 text-green-700 text-body-sm p-4 text-center">
        {{ successMsg }}
      </div>

      <form v-else class="space-y-5" @submit.prevent="submit">
        <div>
          <label class="block text-body-sm text-on-surface-variant mb-1">Họ tên</label>
          <input :value="info.hoTen" readonly class="w-full px-4 py-3 bg-surface-container border border-outline-variant rounded-lg text-on-surface" />
        </div>
        <div>
          <label class="block text-body-sm text-on-surface-variant mb-1">Mã định danh</label>
          <input :value="info.maDinhDanh" readonly class="w-full px-4 py-3 bg-surface-container border border-outline-variant rounded-lg text-on-surface" />
        </div>
        <PasswordInput v-model="matKhauMoi" placeholder="Mật khẩu mới" />
        <PasswordInput v-model="xacNhan" placeholder="Xác nhận mật khẩu" />
        <p class="text-[12px] text-on-surface-variant">Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số.</p>
        <p v-if="errorMsg" class="text-error text-body-sm text-center">{{ errorMsg }}</p>
        <button
          type="submit"
          :disabled="isSubmitting"
          class="w-full text-white py-3.5 px-6 rounded-lg font-semibold hover:bg-blue-800 transition-all flex items-center justify-center gap-2 shadow-md disabled:opacity-70"
          style="background-color:#005ea3;"
        >
          <span v-if="isSubmitting" class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
          {{ isSubmitting ? 'Đang kích hoạt...' : 'Kích hoạt & Đăng nhập' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import api from '@/api/api';
  import { useAuthStore } from '@/stores/auth';
  import { validatePassword } from '@/utils/password';
  import PasswordInput from '@/components/PasswordInput.vue';

  const route = useRoute();
  const router = useRouter();
  const authStore = useAuthStore();
  const token = route.query.token;

  const loading = ref(true);
  const loadError = ref('');
  const canResend = ref(false);
  const info = ref({});
  const matKhauMoi = ref('');
  const xacNhan = ref('');
  const isSubmitting = ref(false);
  const errorMsg = ref('');
  const successMsg = ref('');

  onMounted(async () => {
    if (!token) {
      loadError.value = 'Link kích hoạt không hợp lệ.';
      loading.value = false;
      return;
    }
    try {
      const res = await api.get(`/auth/activation/${token}`);
      info.value = res.data;
    } catch (err) {
      loadError.value = err.response?.data?.message || 'Link kích hoạt không hợp lệ.';
      canResend.value = err.response?.data?.code === 'EXPIRED';
    } finally {
      loading.value = false;
    }
  });

  const submit = async () => {
    errorMsg.value = '';
    if (!validatePassword(matKhauMoi.value)) {
      errorMsg.value = 'Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số';
      return;
    }
    if (matKhauMoi.value !== xacNhan.value) {
      errorMsg.value = 'Mật khẩu xác nhận không khớp';
      return;
    }
    isSubmitting.value = true;
    try {
      const res = await api.post('/auth/activate', {
        token,
        matKhauMoi: matKhauMoi.value,
        xacNhanMatKhau: xacNhan.value
      });
      // E15.6.8: đăng nhập ngay sau kích hoạt
      authStore.setSession(res.data);
      successMsg.value = res.data.welcomeMessage || 'Kích hoạt thành công!';
      setTimeout(() => router.push(res.data.redirectTo || '/dashboard'), 1200);
    } catch (err) {
      errorMsg.value = err.response?.data?.message || 'Có lỗi xảy ra.';
    } finally {
      isSubmitting.value = false;
    }
  };

  const resend = async () => {
    if (!info.value.email) {
      loadError.value = 'Vui lòng liên hệ quản trị viên để được cấp lại link.';
      return;
    }
    try {
      await api.post('/auth/resend-activation', { email: info.value.email });
      loadError.value = '';
      successMsg.value = 'Đã gửi lại link kích hoạt. Vui lòng kiểm tra email.';
    } catch (err) {
      loadError.value = err.response?.data?.message || 'Không thể gửi lại link.';
    }
  };
</script>
