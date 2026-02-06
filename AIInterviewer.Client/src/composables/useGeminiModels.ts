import { ref, watch } from 'vue'
import { client } from '@/lib/gateway'
import { GetGeminiModels } from '@/lib/dtos'

export function useGeminiModels(geminiApiKey: () => string | null | undefined) {
    const models = ref<string[]>([])
    const isLoading = ref(false)
    const error = ref<string | null>(null)

    const fetchModels = async () => {
        const apiKey = geminiApiKey()

        // Only fetch if API key is not empty or null
        if (!apiKey || apiKey.trim() === '') {
            models.value = []
            error.value = null
            return
        }

        isLoading.value = true
        error.value = null

        try {
            const response = await client.api(new GetGeminiModels({ apiKey }))

            if (response.succeeded && response.response) {
                models.value = response.response.models || []
            } else {
                error.value = response.error?.message || 'Failed to fetch Gemini models'
                models.value = []
            }
        } catch (e: any) {
            error.value = e.message || 'An error occurred while fetching models'
            models.value = []
        } finally {
            isLoading.value = false
        }
    }

    // Watch for changes in the API key
    watch(geminiApiKey, () => {
        fetchModels()
    }, { immediate: true })

    return {
        models,
        isLoading,
        error,
        fetchModels
    }
}
