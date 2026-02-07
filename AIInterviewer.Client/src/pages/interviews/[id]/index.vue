<template>
    <div class="container mx-auto p-4 max-w-4xl">
        <div class="flex justify-between items-center mb-6">
            <h1 class="text-3xl font-bold text-gray-800 dark:text-gray-100">Interview #{{ id }}</h1>
            <button @click="endInterview" :disabled="processing" 
                    class="bg-red-600 text-white px-6 py-2 rounded-full font-semibold hover:bg-red-700 transition shadow-lg disabled:opacity-50">
                {{ processing ? 'Ending...' : 'End Interview' }}
            </button>
        </div>
        
        <ChatHistory
            :history="history"
            :processing-ai="processingAi"
            :auto-starting="autoStarting"
        />

        <ChatInput
            :disabled="processing"
            :processing-ai="processingAi"
            v-model:manual-mode="manualMode"
            v-model:review-mode="reviewMode"
            v-model:skip-voice-playback="skipVoicePlayback"
            :transcription-provider="transcriptionProvider"
            :transcriber="transcribeAudio"
            :on-send="sendMessage"
        />
    </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useStorage } from '@vueuse/core'
import { useSiteConfig } from '@/composables/useSiteConfig'
import { useInterview } from '@/composables/useInterview'
import ChatHistory from '@/components/interview/ChatHistory.vue'
import ChatInput from '@/components/interview/ChatInput.vue'

const route = useRoute()
const id = computed(() => Number((route.params as any).id))

const {
    history,
    processing,
    processingAi,
    autoStarting,
    skipVoicePlayback,
    loadInterview,
    startInterview,
    sendMessage,
    transcribeAudio,
    endInterview
} = useInterview(id)

const { siteConfig } = useSiteConfig()

// User Preferences (Persisted here)
const manualMode = useStorage('interview-manual-mode', false)
const reviewMode = useStorage('interview-review-mode', false)

const transcriptionProvider = computed(() => siteConfig.value?.transcriptionProvider || 'Gemini')

const shouldAutoStart = computed(() => {
    const value = route.query.autostart
    return value === '1' || value === 'true'
})

onMounted(async () => {
    await loadInterview()
    await startInterview(shouldAutoStart.value)
})
</script>
