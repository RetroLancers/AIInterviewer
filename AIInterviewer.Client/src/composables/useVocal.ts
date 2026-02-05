import { ref, onUnmounted } from 'vue'

export function useVocal() {
    const isRecording = ref(false)
    const mediaRecorder = ref<MediaRecorder | null>(null)
    const audioChunks = ref<Blob[]>([])
    const audioUrl = ref<string | null>(null)
    const stream = ref<MediaStream | null>(null)
    const mimeType = ref('audio/webm')

    const startRecording = async () => {
        try {
            stream.value = await navigator.mediaDevices.getUserMedia({ audio: true })
            
            // Try supported mime types
            const types = ['audio/webm', 'audio/ogg', 'audio/mp4']
            for (const type of types) {
                if (MediaRecorder.isTypeSupported(type)) {
                    mimeType.value = type
                    break
                }
            }

            mediaRecorder.value = new MediaRecorder(stream.value, { mimeType: mimeType.value })
            audioChunks.value = []

            mediaRecorder.value.ondataavailable = (event) => {
                if (event.data.size > 0) {
                    audioChunks.value.push(event.data)
                }
            }

            mediaRecorder.value.onstop = () => {
                const audioBlob = new Blob(audioChunks.value, { type: mimeType.value })
                audioUrl.value = URL.createObjectURL(audioBlob)
            }

            mediaRecorder.value.start()
            isRecording.value = true
        } catch (error) {
            console.error('Error starting recording:', error)
            throw error
        }
    }

    const stopRecording = (): Promise<{ blob: Blob, mimeType: string }> => {
        return new Promise((resolve) => {
            if (mediaRecorder.value && isRecording.value) {
                mediaRecorder.value.onstop = () => {
                    const audioBlob = new Blob(audioChunks.value, { type: mimeType.value })
                    audioUrl.value = URL.createObjectURL(audioBlob)
                    isRecording.value = false
                    
                    // Stop all tracks in the stream
                    if (stream.value) {
                        stream.value.getTracks().forEach(track => track.stop())
                    }
                    
                    resolve({ blob: audioBlob, mimeType: mimeType.value })
                }
                mediaRecorder.value.stop()
            } else {
                resolve({ blob: new Blob([]), mimeType: '' })
            }
        })
    }

    const blobToBase64 = (blob: Blob): Promise<string> => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader()
            reader.onloadend = () => {
                const base64String = (reader.result as string).split(',')[1]
                resolve(base64String)
            }
            reader.onerror = reject
            reader.readAsDataURL(blob)
        })
    }

    onUnmounted(() => {
        if (stream.value) {
            stream.value.getTracks().forEach(track => track.stop())
        }
    })

    return {
        isRecording,
        audioUrl,
        startRecording,
        stopRecording,
        blobToBase64
    }
}
