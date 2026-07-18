import { defineStore } from 'pinia';
import api from '@/api/api';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('token') || null,
  }),

  actions: {
    async login(credentials) {
      const res = await api.post('/account/login', credentials);

      // Cập nhật đúng cấu trúc backend đã thống nhất
      this.user = res.data.user;
      this.token = res.data.token || "default-token";

      localStorage.setItem('token', this.token);
      localStorage.setItem('user', JSON.stringify(this.user));

      api.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
    },

    logout() {
      this.user = null;
      this.token = null;
      localStorage.clear();
      delete api.defaults.headers.common['Authorization']; // Xóa header khi logout
      window.location.href = '/login';
    }
  }
});
