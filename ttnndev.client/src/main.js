import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

app.use(ElementPlus)

const app = createApp(App)
app.use(createPinia())
app.use(router)
app.mount('#app')
