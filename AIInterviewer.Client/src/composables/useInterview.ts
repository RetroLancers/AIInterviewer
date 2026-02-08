import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { client } from '@/lib/gateway'
import {
    GetInterview,
    FinishInterview,
    AddChatMessage,
    StartInterview,
    TranscribeAudioRequest,
    TextToSpeechRequest,
    type InterviewChatHistoryDto
} from '@/lib/dtos'

export function useInterview(interviewId: any) {
    const router = useRouter()

    // State
    const history = ref<InterviewChatHistoryDto[]>([])
    const processing = ref(false)
    const processingAi = ref(false)
    const autoStarting = ref(false)
    const autoStartError = ref('')
    const hasResult = ref(false)
    const skipVoicePlayback = ref(false)

    // Computed for ID (handle Ref or value)
    const id = computed(() => (typeof interviewId === 'object' && 'value' in interviewId) ? Number(interviewId.value) : Number(interviewId))

    const playAiResponse = async (text: string) => {
        const sanitizedText = text.replace(/```[\s\S]*?```/g, ' ').replace(/\s+/g, ' ').trim()
        if (!sanitizedText) return

        if (skipVoicePlayback.value) return

        // TextToSpeechRequest returns a Blob (WAV)
        const api = await client.api(new TextToSpeechRequest({
            text: sanitizedText,
            interviewId: id.value
        }))
        if (api.succeeded && api.response) {
            const url = URL.createObjectURL(api.response)
            const audio = new Audio(url)
            await audio.play()
        } else {
            console.error('TTS failed', api.error)
        }
    }

    const loadInterview = async () => {
        const api = await client.api(new GetInterview({ id: id.value }))
        if (api.succeeded && api.response) {
            history.value = api.response.history ?? []
            if (api.response.result) {
                hasResult.value = true
                router.push(`/interviews/${id.value}/result`)
            }
        }
    }

    const startInterview = async (shouldAutoStart: boolean) => {
        if (autoStarting.value || history.value.length > 0 || hasResult.value || !shouldAutoStart) return

        autoStarting.value = true
        autoStartError.value = ''
        processingAi.value = true

        const api = await client.api(new StartInterview({ interviewId: id.value }))
        if (api.succeeded && api.response) {
            history.value = api.response.history ?? []

            const lastMsg = history.value[history.value.length - 1]
            if (lastMsg && lastMsg.role === 'Interviewer') {
                await playAiResponse(lastMsg.content)
            }
        } else {
            autoStartError.value = api.error?.message || 'Failed to start interview automatically.'
        }

        processingAi.value = false
        autoStarting.value = false
    }

    const sendMessage = async (message: string) => {
        const optimisticId = Date.now()
        const optimisticEntry = {
            id: optimisticId,
            role: 'User',
            content: message,
            entryDate: new Date().toISOString()
        }

        // Optimistic update
        history.value = [...history.value, optimisticEntry as any]
        processingAi.value = true

        const api = await client.api(new AddChatMessage({
            interviewId: id.value,
            message: message
        }))

        if (api.succeeded && api.response) {
            history.value = api.response.history ?? []
            const lastMsg = history.value[history.value.length - 1]
            if (lastMsg && lastMsg.role === 'Interviewer') {
                await playAiResponse(lastMsg.content)
            }
            processingAi.value = false
            return { success: true }
        } else {
            history.value = history.value.filter(entry => entry.id !== optimisticId)
            processingAi.value = false
            return { success: false, error: api.error?.message || 'Failed to send message.' }
        }
    }

    const transcribeAudio = async (base64: string, mimeType: string) => {
        processingAi.value = true
        const transcribeApi = await client.api(new TranscribeAudioRequest({
            audioData: base64,
            mimeType: mimeType
        }))
        processingAi.value = false

        if (transcribeApi.succeeded && transcribeApi.response?.transcript) {
            return { success: true, transcript: transcribeApi.response.transcript }
        } else {
            return { success: false, error: transcribeApi.error }
        }
    }

    const endInterview = async () => {
        processing.value = true
        const api = await client.api(new FinishInterview({ id: id.value }))
        if (api.succeeded) {
            router.push(`/interviews/${id.value}/result`)
        } else {
            // Caller can handle alert
            throw new Error(api.error?.message || 'Failed to end interview')
        }
        processing.value = false
    }

    return {
        history,
        processing,
        processingAi,
        autoStarting,
        autoStartError,
        hasResult,
        skipVoicePlayback,
        loadInterview,
        startInterview,
        sendMessage,
        transcribeAudio,
        endInterview,
        playAiResponse
    }
}
