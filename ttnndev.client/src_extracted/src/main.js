import { createApp } from 'vue';
import App from './App.vue';
import ElementPlus from 'element-plus';
import 'element-plus/dist/index.css';
import router from './router';
import { createPinia } from 'pinia';

// 1. Khởi tạo app trước
const app = createApp(App);

// 2. Sau đó mới dùng app để .use() các plugin
app.use(createPinia());
app.use(router);
app.use(ElementPlus);

// 3. Cuối cùng mới mount
app.mount('#app');
