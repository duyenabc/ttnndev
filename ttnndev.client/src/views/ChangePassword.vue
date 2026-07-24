<template>
  <div class="ims-scope max-w-[560px]">
    <h1 class="font-headline-sm text-2xl font-bold text-on-surface mb-1">Đổi mật khẩu</h1>
    <p class="text-body-md text-on-surface-variant mb-6">Cập nhật mật khẩu đăng nhập của bạn.</p>

    <div v-if="forced" class="mb-5 rounded-lg bg-amber-50 border border-amber-200 text-amber-800 text-body-sm p-3">
      Bạn đang dùng mật khẩu tạm. Vui lòng đổi mật khẩu để tiếp tục sử dụng hệ thống.
    </div>

    <div class="bg-white rounded-2xl shadow-sm border border-outline-variant p-6">
      <form class="space-y-5" @submit.prevent="submit">
        <PasswordInput v-model="hienTai" placeholder="Mật khẩu hiện tại" />
        <PasswordInput v-model="matKhauMoi" placeholder="Mật khẩu mới" />
        <PasswordInput v-model="xacNhan" placeholder="Xác nhận mật khẩu mới" />
        <p class="text-[12px] text-on-surface-variant">Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số.</p>

        <p v-if="errorMsg" class="text-error text-body-sm">{{ errorMsg }}</p>
        <p v-if="successMsg" class="text-green-600 text-body-sm">{{ successMsg }}</p>

        <div class="flex justify-end">
          <button
            type="submit"
            :disabled="isSubmitting"
            class="text-white py-3 px-6 rounded-lg font-semibold hover:bg-blue-800 transition-all flex items-center justify-center gap-2 shadow-md disabled:opacity-70"
            style="background-color:#005ea3;"
          >
            <span v-if="isSubmitting" class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
            {{ isSubmitting ? 'Đang xử lý...' : 'Đổi mật khẩu' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import api from '@/api/api';
  import { useAuthStore } from '@/stores/auth';
  import { validatePassword } from '@/utils/password';
  import PasswordInput from '@/components/PasswordInput.vue';

  const route = useRoute();
  const router = useRouter();
  const authStore = useAuthStore();
  const forced = computed(() => route.query.forced === '1');

  const hienTai = ref('');
  const matKhauMoi = ref('');
  const xacNhan = ref('');
  const isSubmitting = ref(false);
  const errorMsg = ref('');
  const successMsg = ref('');

  const submit = async () => {
    errorMsg.value = '';
    successMsg.value = '';
    if (!validatePassword(matKhauMoi.value)) {
      errorMsg.value = 'Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số';
      return;
    }
    if (matKhauMoi.value === hienTai.value) {
      errorMsg.value = 'Mật khẩu mới không được trùng mật khẩu hiện tại';
      return;
    }
    if (matKhauMoi.value !== xacNhan.value) {
      errorMsg.value = 'Mật khẩu xác nhận không khớp';
      return;
    }
    isSubmitting.value = true;
    try {
      await api.post('/auth/change-password', {
        matKhauHienTai: hienTai.value,
        matKhauMoi: matKhauMoi.value,
        xacNhanMatKhau: xacNhan.value
      });
      successMsg.value = 'Đổi mật khẩu thành công';
      // Backend thu hồi phiên cũ → đăng nhập lại bằng mật khẩu mới để làm mới token
      const maDinhDanh = authStore.user?.maDinhDanh;
      const result = await authStore.login({ maDinhDanh, matKhau: matKhauMoi.value });
      setTimeout(() => router.push(result.redirectTo || '/dashboard'), 800);
    } catch (err) {
      errorMsg.value = err.response?.data?.message || 'Có lỗi xảy ra.';
    } finally {
      isSubmitting.value = false;
    }
  };
</script>
