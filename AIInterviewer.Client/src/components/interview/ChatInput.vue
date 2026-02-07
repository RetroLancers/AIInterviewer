<template>
    <div class="bg-white dark:bg-gray-800 p-4 rounded-xl shadow-lg border border-gray-200 dark:border-gray-700">
        <div class="flex items-center gap-4">
           <textarea
              v-model="textEntryValue"
              @keyup.enter.exact.prevent="sendText"
              @blur="markUserFinished"
              placeholder="Type your response..."
              rows="3"
              class="flex-grow p-3 border dark:border-gray-600 rounded-lg dark:bg-gray-700 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none transition resize-none"
              :disabled="isActiveRecording || processingAi || isTransmitting || disabled"
           />

           <button
              @click="toggleRecording"
              :disabled="processingAi || disabled"
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
              :disabled="!textEntryValue.trim() || isActiveRecording || processingAi || isTransmitting || disabled"
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
                :checked="skipVoicePlayback"
                @change="$emit('update:skipVoicePlayback', ($event.target as HTMLInputElement).checked)"
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

    <div class="mt-4 flex items-center gap-6 text-sm text-gray-600 dark:text-gray-300 justify-center">
        <label class="flex items-center gap-2 cursor-pointer select-none">
            <input type="checkbox" :checked="manualMode" @change="$emit('update:manualMode', ($event.target as HTMLInputElement).checked)" class="w-4 h-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500">
            <span>Manual Recording Mode</span>
        </label>
        <label class="flex items-center gap-2 cursor-pointer select-none">
            <input type="checkbox" :checked="reviewMode" @change="$emit('update:reviewMode', ($event.target as HTMLInputElement).checked)" class="w-4 h-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500">
            <span>Review Text Before Sending</span>
        </label>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, toRef } from 'vue'
import { useSpeechRecognition } from '@vueuse/core'
import { useVocal } from '@/composables/useVocal'

const props = defineProps<{
    disabled: boolean,
    processingAi: boolean,
    manualMode: boolean,
    reviewMode: boolean,
    skipVoicePlayback: boolean,
    transcriptionProvider: string,
    transcriber: (base64: string, mimeType: string) => Promise<{success: boolean, transcript?: string, error?: any}>,
    onSend: (text: string) => Promise<{success: boolean, error?: string}>
}>()

const emit = defineEmits<{
    (e: 'update:manualMode', value: boolean): void
    (e: 'update:reviewMode', value: boolean): void
    (e: 'update:skipVoicePlayback', value: boolean): void
}>()

const { isRecording, startRecording, stopRecording, blobToBase64 } = useVocal()
const recordingDuration = ref(0)
let durationInterval: any = null
const lastTranscript = ref('')

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
const useBrowserTranscription = computed(() => props.transcriptionProvider === 'Browser')

// Pass manualMode ref to useSpeechRecognition if supported, otherwise it uses initial value
const {
    result: speechResult,
    isListening,
    isSupported: isSpeechSupported,
    start: startListening,
    stop: stopListening
} = useSpeechRecognition({
    continuous: props.manualMode,
    interimResults: false
})

const isActiveRecording = computed(() =>
    useBrowserTranscription.value ? isListening.value : isRecording.value
)

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
            if (props.manualMode && speechResult.value) {
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
    if (props.reviewMode) {
        applyTranscriptToTextbox(text)
    } else {
        await sendText(text)
    }
}

const processAudioResponse = async (blob: Blob, mimeType: string) => {
    if (blob.size === 0) return

    const base64 = await blobToBase64(blob)
    const result = await props.transcriber(base64, mimeType)

    if (result.success && result.transcript) {
        await handleTranscript(result.transcript)
    } else {
        console.error('Transcription failed', result.error)
        textEntry.value.errorMessage = 'Transcription failed. Please try again.'
    }
}

const sendText = async (overrideText?: any) => {
    const text = (typeof overrideText === 'string' ? overrideText : textEntryValue.value).trim()
    if (!text) return

    textEntry.value.status = TextEntryStatus.Transmitting
    textEntry.value.errorMessage = ''

    const result = await props.onSend(text)

    if (result.success) {
        textEntry.value.value = ''
        textEntry.value.status = TextEntryStatus.Idle
    } else {
        textEntry.value.errorMessage = result.error || 'Failed to send message.'
        textEntry.value.status = TextEntryStatus.TransmissionError
    }
}

const markUserFinished = () => {
    if (textEntry.value.value.trim()) {
        textEntry.value.status = TextEntryStatus.UserFinished
    }
}

watch(isListening, (listening) => {
    if (!listening) {
        clearInterval(durationInterval)
    }
})

watch(speechResult, async (value) => {
    if (!useBrowserTranscription.value) return

    const transcript = value?.trim()
    if (!transcript || transcript === lastTranscript.value) return
    appendTranscriptDelta(transcript)
})

watch(() => props.manualMode, (newVal) => {
    // If manual mode changes and we are listening, we might need to restart?
    // VueUse implementation might handle it or not.
    // Usually options are read on start.
    // Ideally we stop and start if currently listening.
    if (isListening.value) {
        stopListening()
        // We don't restart automatically to avoid confusion
    }
})

</script>
