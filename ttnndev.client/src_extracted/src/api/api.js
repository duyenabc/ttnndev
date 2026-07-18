import axios from 'axios';

const api = axios.create({
  // Đảm bảo cổng này đúng với cổng .NET đang chạy (Xem trong launchSettings.json)
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Tự động gắn Token vào mọi Request
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default api;
