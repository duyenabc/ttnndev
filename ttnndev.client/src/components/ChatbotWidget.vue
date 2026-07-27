<template>
  <div class="ims-chatbot">
    <!-- Floating toggle button -->
    <button v-if="!isOpen"
            class="fixed bottom-6 right-6 z-[70] w-14 h-14 rounded-full bg-[#005ea3] text-white shadow-2xl flex items-center justify-center hover:bg-blue-800 transition-all hover:scale-105"
            aria-label="Mở AI Assistant"
            @click="toggleChat">
      <span class="material-symbols-outlined text-[28px]">smart_toy</span>
    </button>

    <!-- Chat Modal Window -->
    <div v-if="isOpen"
         class="fixed bottom-6 right-6 z-[75] w-[420px] max-w-[calc(100vw-32px)] h-[560px] max-h-[calc(100vh-100px)] bg-white rounded-2xl shadow-2xl border border-outline-variant flex flex-col overflow-hidden relative">
      <!-- Slide-out recent conversations drawer (200px width) -->
      <div v-if="showSessions"
           class="absolute left-0 top-0 bottom-0 w-[200px] bg-slate-50 border-r border-outline-variant z-20 p-3 flex flex-col transition-all duration-300 shadow-md">
        <div class="flex items-center justify-between pb-2 border-b border-outline-variant mb-2">
          <span class="font-semibold text-body-sm text-on-surface">Hội thoại gần đây</span>
          <button class="text-on-surface-variant hover:text-primary p-1" @click="showSessions = false">
            <span class="material-symbols-outlined text-[18px]">close</span>
          </button>
        </div>
        <div class="flex-1 overflow-y-auto space-y-1">
          <button v-for="(s, idx) in sessions"
                  :key="idx"
                  class="w-full text-left px-2 py-2 rounded-lg text-body-xs hover:bg-white transition-colors truncate block border border-transparent hover:border-outline-variant"
                  @click="selectSession(s)">
            <p class="font-medium text-on-surface truncate">{{ s.title }}</p>
            <p class="text-slate-400 text-[10px]">{{ s.time }} · {{ s.msgCount }} tin nhắn</p>
          </button>
        </div>
      </div>

      <!-- Header -->
      <div class="bg-[#005ea3] text-white px-4 py-3 flex items-center justify-between z-10 shadow-sm">
        <div class="flex items-center gap-2">
          <button class="p-1 rounded-full hover:bg-white/20 transition-colors"
                  title="Lịch sử hội thoại"
                  @click="showSessions = !showSessions">
            <span class="material-symbols-outlined text-[20px]">menu</span>
          </button>
          <div class="w-8 h-8 rounded-full bg-white/20 flex items-center justify-center">
            <span class="material-symbols-outlined text-[20px]">smart_toy</span>
          </div>
          <div>
            <h3 class="font-bold text-body-md leading-tight">AI Assistant</h3>
            <p class="text-[11px] text-white/80">Hỗ trợ thực tập số IMS</p>
          </div>
        </div>
        <button class="p-1 rounded-full hover:bg-white/20 transition-colors" @click="toggleChat">
          <span class="material-symbols-outlined text-[20px]">close</span>
        </button>
      </div>

      <!-- Messages Body -->
      <div ref="chatBox" class="flex-1 overflow-y-auto p-4 space-y-3 bg-slate-50 text-body-sm">
        <div v-for="(m, idx) in messages" :key="idx" :class="m.sender === 'user' ? 'text-right' : 'text-left'">
          <div class="inline-block px-3.5 py-2.5 rounded-2xl max-w-[85%] text-left whitespace-pre-line shadow-sm"
               :class="m.sender === 'user'
              ? 'bg-[#005ea3] text-white rounded-br-none'
              : 'bg-white text-on-surface border border-outline-variant rounded-bl-none'">
            {{ m.text }}
          </div>
        </div>

        <div v-if="isTyping" class="text-left">
          <div class="inline-block px-3.5 py-2 rounded-2xl bg-white border border-outline-variant text-slate-400 text-body-xs animate-pulse">
            AI đang trả lời...
          </div>
        </div>
      </div>

      <!-- Input Footer -->
      <div class="p-3 bg-white border-t border-outline-variant z-10">
        <form class="flex items-center gap-2" @submit.prevent="sendMessage">
          <input v-model="inputQuery"
                 type="text"
                 maxlength="1000"
                 placeholder="Hỏi về quy định, deadline, tiến độ sinh viên..."
                 class="flex-1 px-3.5 py-2 border border-outline-variant rounded-xl text-body-sm outline-none focus:border-primary focus:ring-1 focus:ring-primary"
                 @keyup.enter="sendMessage" />
          <button type="submit"
                  :disabled="!isValidInput || isTyping"
                  class="p-2 rounded-xl bg-[#005ea3] text-white hover:bg-blue-800 disabled:opacity-50 disabled:cursor-not-allowed transition-all flex items-center justify-center">
            <span class="material-symbols-outlined text-[20px]">send</span>
          </button>
        </form>
        <p class="text-[10px] text-slate-400 mt-1 text-center">Tối đa 1000 ký tự. Trim tự động khoảng trắng.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, nextTick } from 'vue';
  import api from '@/api/api';

  const isOpen = ref(false);
  const showSessions = ref(false);
  const inputQuery = ref('');
  const isTyping = ref(false);
  const chatBox = ref(null);

  const messages = ref([
    { sender: 'bot', text: 'Xin chào! Tôi là AI Assistant Hỗ trợ Thực tập IMS. Bạn có thể hỏi tôi về quy định thực tập, deadline, hoặc tra cứu tiến độ sinh viên.' }
  ]);

  const sessions = ref([
    { title: 'Quy trình nộp báo cáo', time: '09:15', msgCount: 4 },
    { title: 'Tra cứu tiến độ Trần Thị Lan', time: 'Hôm qua', msgCount: 6 }
  ]);

  const isValidInput = computed(() => {
    return inputQuery.value && inputQuery.value.trim().length > 0;
  });

  function toggleChat() {
    isOpen.value = !isOpen.value;
  }

  function scrollToBottom() {
    nextTick(() => {
      if (chatBox.value) {
        chatBox.value.scrollTop = chatBox.value.scrollHeight;
      }
    });
  }

  function selectSession(s) {
    showSessions.value = false;
    messages.value.push({ sender: 'bot', text: `Đã mở lại hội thoại: "${s.title}"` });
    scrollToBottom();
  }

  async function sendMessage() {
    if (!isValidInput.value || isTyping.value) return;

    const query = inputQuery.value.trim();
    inputQuery.value = '';

    messages.value.push({ sender: 'user', text: query });
    scrollToBottom();

    isTyping.value = true;

    try {
      const res = await api.post('/chatbot/query', { question: query });
      const fullText = res.data?.answer || 'Tôi đã tiếp nhận câu hỏi của bạn.';

      // Streaming typewriter effect
      messages.value.push({ sender: 'bot', text: '' });
      const botMsgIndex = messages.value.length - 1;

      let currentLen = 0;
      const interval = setInterval(() => {
        currentLen += 3;
        messages.value[botMsgIndex].text = fullText.substring(0, currentLen);
        scrollToBottom();
        if (currentLen >= fullText.length) {
          clearInterval(interval);
          isTyping.value = false;
        }
      }, 30);
    } catch (err) {
      isTyping.value = false;
      messages.value.push({ sender: 'bot', text: 'Không thể kết nối máy chủ AI Assistant. Vui lòng kiểm tra lại đường truyền.' });
      scrollToBottom();
    }
  }
</script>
