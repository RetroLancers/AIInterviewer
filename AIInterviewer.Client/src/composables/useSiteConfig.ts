import { ref, onMounted } from 'vue'
import { client } from '@/lib/gateway'
import { GetSiteConfigRequest, UpdateSiteConfigRequest, type SiteConfigResponse } from '@/lib/dtos'

export function useSiteConfig() {
    const siteConfig = ref<SiteConfigResponse | null>(null)
    const isLoading = ref(false)
    const isSaving = ref(false)
    const error = ref<string | null>(null)
    const saveSuccess = ref(false)

    const loadSiteConfig = async () => {
        isLoading.value = true
        error.value = null

        // Site config typically has ID of 1 (singleton pattern)
        const response = await client.api(new GetSiteConfigRequest({ id: 1 }))

        if (response.succeeded && response.response) {
            siteConfig.value = response.response
        } else {
            error.value = response.error?.message || 'Failed to load site configuration'
        }

        isLoading.value = false
    }

    const saveSiteConfig = async (
        activeAiConfigId?: number,
        globalFallbackModel?: string,
        kokoroVoice?: string,
        transcriptionProvider?: string
    ) => {
        if (!siteConfig.value) {
            error.value = 'No configuration loaded'
            return false
        }

        isSaving.value = true
        error.value = null
        saveSuccess.value = false

        const response = await client.api(new UpdateSiteConfigRequest({
            id: siteConfig.value.id,
            activeAiConfigId,
            globalFallbackModel: globalFallbackModel || undefined,
            kokoroVoice: kokoroVoice || undefined,
            transcriptionProvider: transcriptionProvider || siteConfig.value.transcriptionProvider || 'Gemini'
        }))

        if (response.succeeded) {
            saveSuccess.value = true
            // Reload the config to get the latest state
            await loadSiteConfig()
            setTimeout(() => {
                saveSuccess.value = false
            }, 3000)
            isSaving.value = false
            return true
        }

        error.value = response.error?.message || 'Failed to update site configuration'
        isSaving.value = false
        return false
    }

    onMounted(() => {
        loadSiteConfig()
    })

    return {
        siteConfig,
        isLoading,
        isSaving,
        error,
        saveSuccess,
        loadSiteConfig,
        saveSiteConfig
    }
}
