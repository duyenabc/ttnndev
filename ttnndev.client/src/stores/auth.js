import { defineStore } from 'pinia';
import api from '@/api/api';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user') || 'null'),
    accessToken: localStorage.getItem('accessToken') || null,
    refreshToken: localStorage.getItem('refreshToken') || null
  }),

  getters: {
    isAuthenticated: (state) => !!state.accessToken,
    role: (state) => state.user?.vaiTro || null
  },

  actions: {
    setSession(data) {
      this.user = data.user;
      this.accessToken = data.accessToken;
      this.refreshToken = data.refreshToken;
      localStorage.setItem('accessToken', data.accessToken);
      localStorage.setItem('refreshToken', data.refreshToken);
      localStorage.setItem('user', JSON.stringify(data.user));
    },

    // E15.1 đăng nhập
    async login(credentials) {
      const res = await api.post('/auth/login', credentials);
      this.setSession(res.data);
      return res.data; // { redirectTo, buocDoiMatKhau, user, ... }
    },

    // E15.2 đăng xuất (huỷ phiên phía server, đồng bộ đa tab)
    async logout({ redirect = true } = {}) {
      const token = this.refreshToken;
      try {
        if (token) await api.post('/auth/logout', { refreshToken: token });
      } catch {
        // bỏ qua lỗi mạng khi đăng xuất
      }
      this.clearLocal();
      // Thông báo cho các tab khác (E15.2.6)
      localStorage.setItem('auth-event', `logout-${Date.now()}`);
      if (redirect) window.location.replace('/login');
    },

    clearLocal() {
      this.user = null;
      this.accessToken = null;
      this.refreshToken = null;
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
    }
  }
});
