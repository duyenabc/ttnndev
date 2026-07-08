import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'https://localhost:63365/api', // Đảm bảo cổng này khớp với cổng của Server .NET của bạn
  headers: {
    'Content-Type': 'application/json',
  },
});

export default apiClient;
