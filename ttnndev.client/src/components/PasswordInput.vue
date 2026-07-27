<template>
  <div class="relative">
    <span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-outline text-[20px]">lock</span>
    <input
      :value="modelValue"
      :type="show ? 'text' : 'password'"
      :placeholder="placeholder"
      required
      class="w-full pl-12 pr-12 py-4 bg-white border border-outline-variant rounded-lg font-body-md text-on-surface focus:ring-2 focus:ring-blue-500/20 focus:border-primary outline-none transition-all placeholder:text-outline"
      @input="$emit('update:modelValue', $event.target.value)"
    />
    <button
      type="button"
      aria-label="Ẩn hiện mật khẩu"
      class="absolute right-4 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-primary transition-colors p-1"
      @click="show = !show"
    >
      <span class="material-symbols-outlined text-[20px]">{{ show ? 'visibility_off' : 'visibility' }}</span>
    </button>
  </div>
</template>

<script setup>
  import { ref } from 'vue';

  defineProps({
    modelValue: { type: String, default: '' },
    placeholder: { type: String, default: 'Mật khẩu' }
  });
  defineEmits(['update:modelValue']);

  const show = ref(false);
</script>
