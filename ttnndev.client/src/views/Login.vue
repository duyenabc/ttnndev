<template>
  <div class="ims-scope flex min-h-screen bg-white text-on-surface">
    <!-- Left brand panel -->
    <aside class="hidden lg:block w-[30%] relative overflow-hidden">
      <div class="absolute inset-0 bg-gradient-to-br from-primary via-primary-container to-surface-tint"></div>
      <div class="absolute bottom-0 left-0 right-0 bg-blue-900/70 p-10 text-white backdrop-blur-sm">
        <h2 class="font-bold text-2xl mb-2">Chào mừng đến với IMS</h2>
        <p class="text-sm opacity-90 leading-relaxed">
          Hệ thống Quản lý Thực tập số, nơi kết nối, theo dõi tiến trình và tối ưu hóa
          chu kỳ thực tập của bạn.
        </p>
      </div>
    </aside>

    <!-- Right form panel -->
    <main class="w-full lg:w-[70%] bg-white flex flex-col">
      <div class="flex-grow flex items-center justify-center px-6 py-12">
        <div class="w-full max-w-[480px] flex flex-col items-center">
          <!-- Brand -->
          <div class="mb-10 text-center">
            <div class="flex items-center justify-center mb-6">
              <div class="w-24 h-24 rounded-full bg-primary/10 flex items-center justify-center">
                <span class="material-symbols-outlined text-primary text-[56px]">school</span>
              </div>
            </div>
            <h1 class="font-display-lg text-4xl font-bold mb-1" style="color:#f7b011;">IMS</h1>
            <p class="font-body-md text-on-surface-variant">Hệ thống Quản lý Thực tập</p>
          </div>

          <!-- Thông báo hết phiên (E15.7) -->
          <p v-if="expiredNotice" class="w-full mb-4 rounded-lg bg-amber-50 border border-amber-200 text-amber-800 text-body-sm p-3 text-center">
            Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại
          </p>

          <!-- Form -->
          <form class="w-full space-y-6" @submit.prevent="handleLogin">
            <div class="relative">
              <span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-outline text-[20px]">person</span>
              <input
                v-model="loginForm.maDinhDanh"
                type="text"
                required
                placeholder="Mã sinh viên / giảng viên"
                class="w-full pl-12 pr-4 py-4 bg-white border border-outline-variant rounded-lg font-body-md text-on-surface focus:ring-2 focus:ring-blue-500/20 focus:border-primary outline-none transition-all placeholder:text-outline"
              />
            </div>

            <div class="relative">
              <span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-outline text-[20px]">lock</span>
              <input
                v-model="loginForm.matKhau"
                :type="showPassword ? 'text' : 'password'"
                required
                placeholder="Mật khẩu"
                class="w-full pl-12 pr-12 py-4 bg-slate-50 border border-outline-variant rounded-lg font-body-md text-on-surface focus:ring-2 focus:ring-blue-500/20 focus:border-primary outline-none transition-all placeholder:text-outline"
              />
              <button
                type="button"
                aria-label="Ẩn hiện mật khẩu"
                class="absolute right-4 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-primary transition-colors p-1"
                @click="showPassword = !showPassword"
              >
                <span class="material-symbols-outlined text-[20px]">{{ showPassword ? 'visibility_off' : 'visibility' }}</span>
              </button>
            </div>

            <p v-if="errorMessage" class="text-error text-body-sm text-center">{{ errorMessage }}</p>

            <button
              type="submit"
              :disabled="isSubmitting"
              class="w-full text-white py-4 px-6 rounded-lg font-semibold text-lg hover:bg-blue-800 transition-all active:scale-[0.99] flex items-center justify-center gap-2 shadow-md disabled:opacity-70"
              style="background-color:#005ea3;"
            >
              <span v-if="isSubmitting" class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
              {{ isSubmitting ? 'Đang xử lý...' : 'Đăng nhập' }}
            </button>

            <div class="text-center">
              <router-link class="font-body-sm font-medium hover:underline" to="/forgot-password" style="color:#005ea3;">Quên mật khẩu?</router-link>
            </div>

            <p class="text-[12px] text-gray-500 text-center mt-8 leading-relaxed">
              Để được trợ giúp, hãy liên hệ với bộ phận hỗ trợ kỹ thuật của Trung tâm Số và Học liệu:
              <a class="text-blue-600 hover:underline" href="https://www.facebook.com/CenterITC" target="_blank">https://www.facebook.com/CenterITC</a>
            </p>
          </form>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
  import { reactive, ref, computed } from 'vue';
  import { useRouter, useRoute } from 'vue-router';
  import { useAuthStore } from '@/stores/auth';

  const loginForm = reactive({ maDinhDanh: '', matKhau: '' });
  const showPassword = ref(false);
  const isSubmitting = ref(false);
  const errorMessage = ref('');
  const router = useRouter();
  const route = useRoute();
  const authStore = useAuthStore();
  const expiredNotice = computed(() => route.query.expired === '1');

  const handleLogin = async () => {
    errorMessage.value = '';
    isSubmitting.value = true;
    try {
      const result = await authStore.login({
        maDinhDanh: loginForm.maDinhDanh,
        matKhau: loginForm.matKhau
      });
      // E15.3.6: mật khẩu tạm buộc đổi trước khi vào hệ thống
      if (result.buocDoiMatKhau) {
        router.push('/change-password?forced=1');
        return;
      }
      // E15.1.2: điều hướng theo vai trò
      router.push(result.redirectTo || '/dashboard');
    } catch (err) {
      errorMessage.value = err.response?.data?.message
        || 'Mã định danh hoặc mật khẩu không đúng';
    } finally {
      isSubmitting.value = false;
    }
  };
</script>
