<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center p-4">
    <div class="max-w-md w-full bg-white rounded-2xl shadow-xl border border-slate-100 p-8">
      <div class="text-center mb-6">
        <h1 class="text-2xl font-bold text-slate-900">Đăng ký Tài khoản</h1>
        <p class="text-sm text-slate-500 mt-1">Gửi yêu cầu khởi tạo tài khoản hệ thống IMS</p>
      </div>

      <div v-if="successMsg" class="p-4 rounded-xl bg-emerald-50 border border-emerald-200 text-emerald-800 text-sm mb-6">
        {{ successMsg }}
      </div>

      <form v-else @submit.prevent="handleRegister" class="space-y-4">
        <div>
          <label class="block text-sm font-semibold text-slate-700 mb-1">Mã định danh (SV / GV)</label>
          <input v-model="form.maDinhDanh" type="text" required class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600" placeholder="VD: 22120099" />
        </div>
        <div>
          <label class="block text-sm font-semibold text-slate-700 mb-1">Họ và tên</label>
          <input v-model="form.hoTen" type="text" required class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600" placeholder="Nguyễn Văn A" />
        </div>
        <div>
          <label class="block text-sm font-semibold text-slate-700 mb-1">Email trường</label>
          <input v-model="form.email" type="email" required class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600" placeholder="email@student.due.udn.vn" />
        </div>
        <div>
          <label class="block text-sm font-semibold text-slate-700 mb-1">Vai trò</label>
          <select v-model="form.vaiTro" class="w-full px-4 py-2.5 rounded-xl border border-slate-300 text-sm outline-none focus:ring-2 focus:ring-blue-600">
            <option value="SinhVien">Sinh viên</option>
            <option value="GiangVien">Giảng viên</option>
          </select>
        </div>

        <button type="submit" :disabled="loading" class="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-md transition text-sm">
          {{ loading ? 'Đang gửi...' : 'Gửi yêu cầu đăng ký' }}
        </button>
      </form>

      <div class="mt-6 text-center text-xs text-slate-500">
        Đã có tài khoản? <router-link to="/login" class="text-blue-600 font-bold hover:underline">Đăng nhập</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';

  const form = ref({ maDinhDanh: '', hoTen: '', email: '', vaiTro: 'SinhVien' });
  const loading = ref(false);
  const successMsg = ref('');

  function handleRegister() {
    loading.value = true;
    setTimeout(() => {
      loading.value = false;
      successMsg.value = 'Yêu cầu đăng ký đã được gửi tới Giáo vụ khoa. Vui lòng chờ phê duyệt!';
    }, 600);
  }
</script>
