import { ref, onMounted } from 'vue'
import { client } from '@/lib/gateway'
import { ListAiConfigs, type AiConfigResponse } from '@/lib/dtos'

export function useAiConfigs() {
    const aiConfigs = ref<AiConfigResponse[]>([])
    const isLoading = ref(false)
    const error = ref<string | null>(null)

    const loadAiConfigs = async () => {
        isLoading.value = true
        error.value = null

        const response = await client.api(new ListAiConfigs())

        if (response.succeeded && response.response) {
            aiConfigs.value = response.response.configs || []
        } else {
            error.value = response.error?.message || 'Failed to load AI configurations'
        }

        isLoading.value = false
    }

    onMounted(() => {
        loadAiConfigs()
    })

    return {
        aiConfigs,
        isLoading,
        error,
        loadAiConfigs
    }
}
