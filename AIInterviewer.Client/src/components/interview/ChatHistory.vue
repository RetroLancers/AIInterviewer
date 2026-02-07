<template>
    <div ref="chatContainer" class="bg-white dark:bg-gray-800 p-6 rounded-xl shadow-inner h-[60vh] overflow-y-auto mb-6 border border-gray-200 dark:border-gray-700">
        <div v-for="msg in history" :key="msg.id" class="mb-4 flex" :class="msg.role === 'User' ? 'justify-end' : 'justify-start'">
            <div :class="[
                'max-w-[80%] p-4 rounded-2xl shadow-sm',
                msg.role === 'User'
                    ? 'bg-blue-600 text-white rounded-tr-none'
                    : 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200 rounded-tl-none'
            ]">
                <div class="text-xs opacity-75 mb-1 font-bold">{{ msg.role }}</div>
                <div class="whitespace-pre-wrap">{{ msg.content }}</div>
            </div>
        </div>

        <div v-if="processingAi" class="flex justify-start mb-4">
            <div class="bg-gray-100 dark:bg-gray-700 p-4 rounded-2xl rounded-tl-none shadow-sm flex items-center space-x-2">
                <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce [animation-delay:-.3s]"></div>
                <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce [animation-delay:-.5s]"></div>
            </div>
        </div>

        <div v-if="history.length === 0 && autoStarting" class="text-gray-500 text-center mt-20 italic">
            Starting the interview...
        </div>

        <div v-else-if="history.length === 0 && !processingAi" class="text-gray-500 text-center mt-20 italic">
            The interviewer is waiting. Click the microphone to start.
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch, nextTick } from 'vue'
import type { InterviewChatHistoryDto } from '@/lib/dtos'

const props = defineProps<{
    history: InterviewChatHistoryDto[],
    processingAi: boolean,
    autoStarting: boolean
}>()

const chatContainer = ref<HTMLDivElement | null>(null)

const scrollToBottom = async () => {
    await nextTick()
    if (chatContainer.value) {
        chatContainer.value.scrollTop = chatContainer.value.scrollHeight
    }
}

watch(() => props.history, () => {
    scrollToBottom()
}, { deep: true })

watch(() => props.processingAi, (newVal) => {
    if (newVal) {
        scrollToBottom()
    }
})
</script>

<style scoped>
.whitespace-pre-wrap {
    white-space: pre-wrap;
}
</style>
