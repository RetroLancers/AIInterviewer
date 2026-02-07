import { ref, onMounted } from 'vue'
import { client } from '@/lib/gateway'
import { ListAiConfigs, type AiConfigResponse } from '@/lib/dtos'

export function useAiConfigs() {
    const configs = ref<AiConfigResponse[]>([])
    const isLoading = ref(false)
    const error = ref<string | null>(null)

    const loadConfigs = async () => {
        isLoading.value = true
        error.value = null

        const response = await client.api(new ListAiConfigs())

        if (response.succeeded && response.response) {
            configs.value = response.response.configs || []
        } else {
            error.value = response.error?.message || 'Failed to load AI configurations'
            configs.value = []
        }

        isLoading.value = false
    }

    onMounted(() => {
        loadConfigs()
    })

    return {
        configs,
        isLoading,
        error,
        loadConfigs
    }
}
