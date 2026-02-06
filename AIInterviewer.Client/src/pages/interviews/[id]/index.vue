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
               <textarea
                  v-model="textEntryValue"
                  @keyup.enter.exact.prevent="sendText"
                  @blur="markUserFinished"
                  placeholder="Type your response..."
                  rows="3"
                  class="flex-grow p-3 border dark:border-gray-600 rounded-lg dark:bg-gray-700 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none transition resize-none"
                  :disabled="isActiveRecording || processingAi || isTransmitting"
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
                  :disabled="!textEntryValue.trim() || isActiveRecording || processingAi || isTransmitting"
                  class="bg-green-600 text-white p-4 rounded-full hover:bg-green-700 transition disabled:opacity-50 shadow-md"
               >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                  </svg>
               </button>
            </div>
            <div class="mt-3 flex items-center gap-2 text-sm text-gray-600 dark:text-gray-300">
                <input
                    id="skip-voice"
                    type="checkbox"
                    v-model="skipVoicePlayback"
                    class="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700"
                />
                <label for="skip-voice" class="select-none">
                    Skip voice playback for AI responses
                </label>
            </div>
            <div v-if="isActiveRecording" class="text-center mt-2 text-red-600 text-sm font-bold animate-pulse">
                Recording... {{ recordingDuration }}s
            </div>
            <div v-if="textEntry.errorMessage" class="mt-2 text-sm text-red-600 dark:text-red-400">
                {{ textEntry.errorMessage }}
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
const chatContainer = ref<HTMLDivElement | null>(null)
const skipVoicePlayback = ref(false)

const { isRecording, startRecording, stopRecording, blobToBase64 } = useVocal()
const { siteConfig } = useSiteConfig()
const recordingDuration = ref(0)
let durationInterval: any = null
const lastTranscript = ref('')

// User Preferences
const manualMode = useStorage('interview-manual-mode', false)
const reviewMode = useStorage('interview-review-mode', false)

enum TextEntryStatus {
    Idle = 'Idle',
    UserTalking = 'UserTalking',
    UserFinished = 'UserFinished',
    Transmitting = 'Transmitting',
    TransmissionError = 'TransmissionError'
}

const textEntry = ref({
    status: TextEntryStatus.Idle,
    value: '',
    errorMessage: ''
})

const textEntryValue = computed({
    get: () => textEntry.value.value,
    set: (value: string) => {
        textEntry.value.value = value
        textEntry.value.errorMessage = ''
        if (value.trim()) {
            textEntry.value.status = TextEntryStatus.UserTalking
        } else if (textEntry.value.status !== TextEntryStatus.Transmitting) {
            textEntry.value.status = TextEntryStatus.Idle
        }
    }
})

const isTransmitting = computed(() => textEntry.value.status === TextEntryStatus.Transmitting)

const transcriptionProvider = computed(() => siteConfig.value?.transcriptionProvider || 'Gemini')
const useBrowserTranscription = computed(() => transcriptionProvider.value === 'Browser')

const {
    result: speechResult,
    isListening,
    isSupported: isSpeechSupported,
    start: startListening,
    stop: stopListening
} = useSpeechRecognition({
    continuous: manualMode.value,
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
            textEntry.value.status = TextEntryStatus.UserFinished
            
            // In manual mode, we process the result when stopping
            if (manualMode.value && speechResult.value) {
                const transcript = speechResult.value.trim()
                if (transcript) {
                    applyTranscriptToTextbox(transcript)
                }
            }
        } else {
            lastTranscript.value = ''
            speechResult.value = ''
            recordingDuration.value = 0
            durationInterval = setInterval(() => recordingDuration.value++, 1000)
            textEntry.value.status = TextEntryStatus.UserTalking
            startListening()
        }
        return
    }

    if (isRecording.value) {
        clearInterval(durationInterval)
        const { blob, mimeType } = await stopRecording()
        await processAudioResponse(blob, mimeType)
        textEntry.value.status = TextEntryStatus.UserFinished
    } else {
        recordingDuration.value = 0
        durationInterval = setInterval(() => recordingDuration.value++, 1000)
        textEntry.value.status = TextEntryStatus.UserTalking
        await startRecording()
    }
}

const applyTranscriptToTextbox = (text: string) => {
    textEntryValue.value = textEntryValue.value
        ? `${textEntryValue.value} ${text}`
        : text
}

const appendTranscriptDelta = (transcript: string) => {
    const previous = lastTranscript.value
    const nextText = transcript.startsWith(previous)
        ? transcript.slice(previous.length).trim()
        : transcript

    if (nextText) {
        applyTranscriptToTextbox(nextText)
    }
    lastTranscript.value = transcript
}

const handleTranscript = async (text: string) => {
    if (reviewMode.value) {
        applyTranscriptToTextbox(text)
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
    const text = textEntryValue.value.trim()
    if (!text) return
    await sendMessage(text)
}

const sendMessage = async (message: string) => {
    const optimisticEntry = {
        id: Date.now(),
        role: 'User',
        content: message,
        entryDate: new Date().toISOString()
    }

    textEntry.value.status = TextEntryStatus.Transmitting
    textEntry.value.errorMessage = ''
    history.value = [...history.value, optimisticEntry]
    scrollToBottom()
    processingAi.value = true
    const api = await client.api(new AddChatMessage({
        interviewId: id.value,
        message: message
    }))

    if (api.succeeded && api.response) {
        history.value = api.response.history ?? []
        textEntry.value.value = ''
        textEntry.value.status = TextEntryStatus.Idle
        scrollToBottom()
        
        // Play TTS for the last AI message
        const lastMsg = history.value[history.value.length - 1]
        if (lastMsg && lastMsg.role === 'Interviewer') {
            await playAiResponse(lastMsg.content)
        }
    } else {
        history.value = history.value.filter((entry) => entry.id !== optimisticEntry.id)
        textEntry.value.errorMessage = api.error?.message || 'Failed to send message. Please try again.'
        textEntry.value.status = TextEntryStatus.TransmissionError
    }
    processingAi.value = false
}

const playAiResponse = async (text: string) => {
 
    const sanitizedText = text.replace(/```[\s\S]*?```/g, ' ').replace(/\s+/g, ' ').trim()
    if (!sanitizedText) return
 
    if (skipVoicePlayback.value) return
 
    // TextToSpeechRequest returns a Blob (WAV)
    const api = await client.api(new TextToSpeechRequest({ text: sanitizedText }))
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

const markUserFinished = () => {
    if (textEntry.value.value.trim()) {
        textEntry.value.status = TextEntryStatus.UserFinished
    }
}

watch(speechResult, async (value) => {
    if (!useBrowserTranscription.value) return
   
  
    
    const transcript = value?.trim()
    if (!transcript || transcript === lastTranscript.value) return
    appendTranscriptDelta(transcript)
})
</script>

<style scoped>
.whitespace-pre-wrap {
    white-space: pre-wrap;
}
</style>
