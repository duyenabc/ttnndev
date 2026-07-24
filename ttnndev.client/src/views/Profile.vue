<template>
  <div class="ims-scope max-w-[720px]">
    <h1 class="font-headline-sm text-2xl font-bold text-on-surface mb-1">Hồ sơ cá nhân</h1>
    <p class="text-body-md text-on-surface-variant mb-6">Thông tin định danh do quản trị viên quản lý, chỉ ảnh đại diện có thể chỉnh sửa.</p>

    <div v-if="loading" class="text-on-surface-variant">Đang tải...</div>

    <div v-else class="bg-white rounded-2xl shadow-sm border border-outline-variant p-6">
      <!-- Avatar -->
      <div class="flex items-center gap-5 mb-6 pb-6 border-b border-outline-variant">
        <div class="w-20 h-20 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden shrink-0">
          <img v-if="avatarPreview" :src="avatarPreview" alt="avatar" class="w-full h-full object-cover" />
          <span v-else class="material-symbols-outlined text-primary text-[48px]" style="font-variation-settings:'FILL' 1;">account_circle</span>
        </div>
        <div>
          <input ref="fileInput" type="file" accept=".jpg,.jpeg,.png" class="hidden" @change="onFile" />
          <button
            class="px-4 py-2 rounded-lg border border-outline-variant text-on-surface hover:bg-surface-container transition-colors text-body-sm font-medium flex items-center gap-2"
            @click="$refs.fileInput.click()"
          >
            <span class="material-symbols-outlined text-[18px]">photo_camera</span>
            Đổi ảnh đại diện
          </button>
          <p class="text-[12px] text-on-surface-variant mt-1">JPG, JPEG hoặc PNG, tối đa 5MB.</p>
          <p v-if="avatarError" class="text-error text-[12px] mt-1">{{ avatarError }}</p>
        </div>
      </div>

      <!-- Fields theo vai trò -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Field v-for="f in fields" :key="f.label" :label="f.label" :value="f.value" />
      </div>

      <div class="flex items-center justify-end gap-3 mt-6">
        <p v-if="successMsg" class="text-green-600 text-body-sm mr-auto">{{ successMsg }}</p>
        <button
          :disabled="!dirty || isSaving"
          class="text-white py-2.5 px-6 rounded-lg font-semibold transition-all flex items-center gap-2 shadow-md disabled:opacity-50"
          style="background-color:#005ea3;"
          @click="save"
        >
          <span v-if="isSaving" class="material-symbols-outlined animate-spin text-[20px]">progress_activity</span>
          Lưu thay đổi
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted, h } from 'vue';
  import api from '@/api/api';
  import { useAuthStore } from '@/stores/auth';

  const authStore = useAuthStore();
  const loading = ref(true);
  const isSaving = ref(false);
  const successMsg = ref('');
  const avatarError = ref('');
  const profile = ref({});
  const avatarPreview = ref(null);
  const dirty = ref(false);

  const Field = (props) => h('div', {}, [
    h('label', { class: 'block text-body-sm text-on-surface-variant mb-1' }, props.label),
    h('input', {
      value: props.value || '—',
      readonly: true,
      class: 'w-full px-4 py-3 bg-surface-container border border-outline-variant rounded-lg text-on-surface'
    })
  ]);
  Field.props = ['label', 'value'];

  const roleLabels = {
    SinhVien: 'Sinh viên', GiangVien: 'Giảng viên', GiaoVu: 'Giáo vụ khoa', Admin: 'Quản trị viên'
  };

  const fields = computed(() => {
    const p = profile.value;
    const role = p.vaiTro;
    if (role === 'Admin') {
      return [
        { label: 'Họ tên', value: p.hoTen },
        { label: 'Email', value: p.email },
        { label: 'Vai trò', value: roleLabels[role] },
        { label: 'Số điện thoại', value: p.soDienThoai }
      ];
    }
    if (role === 'GiaoVu') {
      return [
        { label: 'Họ tên', value: p.hoTen },
        { label: 'Mã giáo vụ', value: p.maDinhDanh },
        { label: 'Email', value: p.email },
        { label: 'Khoa', value: p.tenKhoa },
        { label: 'Số điện thoại', value: p.soDienThoai }
      ];
    }
    if (role === 'GiangVien') {
      return [
        { label: 'Họ tên', value: p.hoTen },
        { label: 'Mã giảng viên', value: p.maDinhDanh },
        { label: 'Email', value: p.email },
        { label: 'Bộ môn', value: p.tenBoMon },
        { label: 'Khoa', value: p.tenKhoa },
        { label: 'Số điện thoại', value: p.soDienThoai }
      ];
    }
    // SinhVien
    return [
      { label: 'Họ tên', value: p.hoTen },
      { label: 'Mã số sinh viên', value: p.maDinhDanh },
      { label: 'Lớp sinh hoạt', value: p.lopSinhHoat },
      { label: 'Email', value: p.email },
      { label: 'Số điện thoại', value: p.soDienThoai }
    ];
  });

  onMounted(async () => {
    try {
      const res = await api.get('/auth/me');
      profile.value = res.data;
      avatarPreview.value = res.data.anhDaiDien || null;
    } finally {
      loading.value = false;
    }
  });

  const onFile = (e) => {
    avatarError.value = '';
    successMsg.value = '';
    const file = e.target.files?.[0];
    if (!file) return;
    const okType = ['image/jpeg', 'image/png'].includes(file.type)
      || /\.(jpg|jpeg|png)$/i.test(file.name);
    if (!okType) {
      avatarError.value = 'Chỉ chấp nhận file ảnh .jpg, .jpeg, .png';
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      avatarError.value = 'Ảnh không được vượt quá 5MB';
      return;
    }
    const reader = new FileReader();
    reader.onload = () => {
      avatarPreview.value = reader.result;
      dirty.value = true;
    };
    reader.readAsDataURL(file);
  };

  const save = async () => {
    isSaving.value = true;
    successMsg.value = '';
    try {
      await api.put('/auth/me', { anhDaiDien: avatarPreview.value });
      successMsg.value = 'Cập nhật hồ sơ thành công';
      dirty.value = false;
      // cập nhật avatar trong store
      if (authStore.user) {
        authStore.user.anhDaiDien = avatarPreview.value;
        localStorage.setItem('user', JSON.stringify(authStore.user));
      }
    } catch (err) {
      avatarError.value = err.response?.data?.message || 'Không thể lưu hồ sơ.';
    } finally {
      isSaving.value = false;
    }
  };
</script>
