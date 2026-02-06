<template>
    <div class="container mx-auto p-4 max-w-4xl">
        <div class="flex justify-between items-center mb-6">
            <h1 class="text-3xl font-bold text-gray-800 dark:text-gray-100">Interview #{{ id }}</h1>
            <button @click="endInterview" :disabled="processing" 
                    class="bg-red-600 text-white px-6 py-2 rounded-full font-semibold hover:bg-red-700 transition shadow-lg disabled:opacity-50">
                {{ processing ? 'Ending...' : 'End Interview' }}
            </button>
        </div>
        
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

            <div v-if="history.length === 0 && !processingAi" class="text-gray-500 text-center mt-20 italic">
                The interviewer is waiting. Click the microphone to start.
            </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-4 rounded-xl shadow-lg border border-gray-200 dark:border-gray-700">
            <div class="flex items-center gap-4">
               <input 
                  type="text" 
                  v-model="textInput" 
                  @keyup.enter="sendText"
                  placeholder="Type your response..." 
                  class="flex-grow p-3 border dark:border-gray-600 rounded-lg dark:bg-gray-700 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none transition"
                  :disabled="isActiveRecording || processingAi"
               />
               
               <button 
                  @click="toggleRecording"
                  :disabled="processingAi"
                  :class="[
                      'p-4 rounded-full transition shadow-md flex items-center justify-center',
                      isActiveRecording ? 'bg-red-600 animate-pulse text-white' : 'bg-blue-600 text-white hover:bg-blue-700'
                  ]"
                  title="Toggle Microphone"
               >
                  <svg v-if="!isActiveRecording" xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11a7 7 0 01-7 7m0 0a7 7 0 01-7-7m7 7v4m0 0H8m4 0h4m-4-8a3 3 0 01-3-3V5a3 3 0 116 0v6a3 3 0 01-3 3z" />
                  </svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 10a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1H10a1 1 0 01-1-1v-4z" />
                  </svg>
               </button>

               <button 
                  @click="sendText"
                  :disabled="!textInput.trim() || isActiveRecording || processingAi"
                  class="bg-green-600 text-white p-4 rounded-full hover:bg-green-700 transition disabled:opacity-50 shadow-md"
               >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                  </svg>
               </button>
            </div>
            <div v-if="isActiveRecording" class="text-center mt-2 text-red-600 text-sm font-bold animate-pulse">
                Recording... {{ recordingDuration }}s
            </div>
        </div>
        </div>
            <div class="mt-4 flex items-center gap-6 text-sm text-gray-600 dark:text-gray-300 justify-center">
                <label class="flex items-center gap-2 cursor-pointer select-none">
                    <input type="checkbox" v-model="manualMode" class="w-4 h-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500">
                    <span>Manual Recording Mode</span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer select-none">
                    <input type="checkbox" v-model="reviewMode" class="w-4 h-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500">
                    <span>Review Text Before Sending</span>
                </label>
            </div>
    
</template>

<script setup lang="ts">
 
import { computed, ref, onMounted, nextTick, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSpeechRecognition, useStorage } from '@vueuse/core'
import { client } from '@/lib/gateway'
import { 
    GetInterview, 
    FinishInterview, 
    AddChatMessage,
    TranscribeAudioRequest,
    TextToSpeechRequest,
    type InterviewChatHistoryDto 
} from '@/lib/dtos'
import { useVocal } from '@/composables/useVocal'
import { useSiteConfig } from '@/composables/useSiteConfig'

const route = useRoute('/interviews/[id]/')
const router = useRouter()
const id = computed(() => Number(route.params.id))
const history = ref<InterviewChatHistoryDto[]>([])
const processing = ref(false)
const processingAi = ref(false)
const textInput = ref('')
const chatContainer = ref<HTMLDivElement | null>(null)

const { isRecording, startRecording, stopRecording, blobToBase64 } = useVocal()
const { siteConfig } = useSiteConfig()
const recordingDuration = ref(0)
let durationInterval: any = null
const lastTranscript = ref('')

// User Preferences
const manualMode = useStorage('interview-manual-mode', false)
const reviewMode = useStorage('interview-review-mode', false)

const transcriptionProvider = computed(() => siteConfig.value?.transcriptionProvider || 'Gemini')
const useBrowserTranscription = computed(() => transcriptionProvider.value === 'Browser')

const {
    result: speechResult,
    isListening,
    isSupported: isSpeechSupported,
    start: startListening,
    stop: stopListening
} = useSpeechRecognition({
    continuous: manualMode,
    interimResults: false
})

const isActiveRecording = computed(() =>
    useBrowserTranscription.value ? isListening.value : isRecording.value
)

const scrollToBottom = async () => {
    await nextTick()
    if (chatContainer.value) {
        chatContainer.value.scrollTop = chatContainer.value.scrollHeight
    }
}

const loadInterview = async () => {
    const api = await client.api(new GetInterview({ id: id.value }))
    if (api.succeeded && api.response) {
        history.value = api.response.history ?? []
        if (api.response.result) {
            router.push(`/interviews/${id.value}/result`)
        }
        scrollToBottom()
    }
}

const toggleRecording = async () => {
    if (useBrowserTranscription.value) {
        if (!isSpeechSupported.value) {
            alert('Browser speech recognition is not supported in this browser.')
            return
        }

        if (isListening.value) {
            clearInterval(durationInterval)
            stopListening()
            
            // In manual mode, we process the result when stopping
            if (manualMode.value && speechResult.value) {
                const transcript = speechResult.value.trim()
                if (transcript) {
                    await handleTranscript(transcript)
                }
            }
        } else {
            lastTranscript.value = ''
            speechResult.value = ''
            recordingDuration.value = 0
            durationInterval = setInterval(() => recordingDuration.value++, 1000)
            startListening()
        }
        return
    }

    if (isRecording.value) {
        clearInterval(durationInterval)
        const { blob, mimeType } = await stopRecording()
        await processAudioResponse(blob, mimeType)
    } else {
        recordingDuration.value = 0
        durationInterval = setInterval(() => recordingDuration.value++, 1000)
        await startRecording()
    }
}

const handleTranscript = async (text: string) => {
    if (reviewMode.value) {
        // Append to existing text with a space if needed
        textInput.value = textInput.value 
            ? `${textInput.value} ${text}`
            : text
    } else {
        await sendMessage(text)
    }
}

const processAudioResponse = async (blob: Blob, mimeType: string) => {
    if (blob.size === 0) return

    processingAi.value = true
    const base64 = await blobToBase64(blob)
    const transcribeApi = await client.api(new TranscribeAudioRequest({
        audioData: base64,
        mimeType: mimeType
    }))

    const transcript = transcribeApi.response?.transcript
    if (transcribeApi.succeeded && transcript) {
        await handleTranscript(transcript)
    } else {
        console.error('Transcription failed', transcribeApi.error)
    }
    processingAi.value = false
}

const sendText = async () => {
    const text = textInput.value.trim()
    if (!text) return
    textInput.value = ''
    await sendMessage(text)
}

const sendMessage = async (message: string) => {
    processingAi.value = true
    const api = await client.api(new AddChatMessage({
        interviewId: id.value,
        message: message
    }))

    if (api.succeeded && api.response) {
        history.value = api.response.history ?? []
        scrollToBottom()
        
        // Play TTS for the last AI message
        const lastMsg = history.value[history.value.length - 1]
        if (lastMsg && lastMsg.role === 'Interviewer') {
            await playAiResponse(lastMsg.content)
        }
    }
    processingAi.value = false
}

const playAiResponse = async (text: string) => {
    // TextToSpeechRequest returns a Blob (WAV)
    const api = await client.api(new TextToSpeechRequest({ text }))
    if (api.succeeded && api.response) {
        const url = URL.createObjectURL(api.response)
        const audio = new Audio(url)
        await audio.play()
    } else {
        console.error('TTS failed', api.error)
    }
}

const endInterview = async () => {
    if (!confirm('Are you sure you want to end the interview? This will generate your feedback report.')) return;
    
    processing.value = true
    const api = await client.api(new FinishInterview({ id: id.value }))
    if (api.succeeded) {
        router.push(`/interviews/${id.value}/result`)
    } else {
        alert('Failed to end interview: ' + api.error?.message)
    }
    processing.value = false
}

onMounted(() => {
    loadInterview()
})

watch(history, () => {
    scrollToBottom()
}, { deep: true })

watch(isListening, (listening) => {
    if (!listening) {
        clearInterval(durationInterval)
    }
})

watch(speechResult, async (value) => {
    if (!useBrowserTranscription.value) return
    // In manual mode, we wait for the user to stop
    if (manualMode.value) return
    
    const transcript = value?.trim()
    if (!transcript || transcript === lastTranscript.value) return
    lastTranscript.value = transcript
    await handleTranscript(transcript)
})
</script>

<style scoped>
.whitespace-pre-wrap {
    white-space: pre-wrap;
}
</style>
