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
      this.token = res.data.token;
      this.user = res.data.user; // Giả sử API trả về đối tượng user
      localStorage.setItem('token', this.token);
      localStorage.setItem('user', JSON.stringify(this.user));
      api.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
    },
    logout() {
      this.user = null;
      this.token = null;
      localStorage.clear();
      window.location.href = '/login';
    }
  }
});
