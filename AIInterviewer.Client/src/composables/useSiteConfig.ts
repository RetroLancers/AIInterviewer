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

        try {
            // Site config typically has ID of 1 (singleton pattern)
            const response = await client.api(new GetSiteConfigRequest({ id: 1 }))

            if (response.succeeded && response.response) {
                siteConfig.value = response.response
            } else {
                error.value = response.error?.message || 'Failed to load site configuration'
            }
        } catch (e: any) {
            error.value = e.message || 'An error occurred while loading site configuration'
        } finally {
            isLoading.value = false
        }
    }

    const saveSiteConfig = async (
        geminiApiKey: string,
        interviewModel: string,
        globalFallbackModel?: string,
        kokoroVoice?: string
    ) => {
        if (!siteConfig.value) {
            error.value = 'No configuration loaded'
            return false
        }

        isSaving.value = true
        error.value = null
        saveSuccess.value = false

        try {
            const response = await client.api(new UpdateSiteConfigRequest({
                id: siteConfig.value.id,
                geminiApiKey,
                interviewModel,
                globalFallbackModel: globalFallbackModel || undefined,
                kokoroVoice: kokoroVoice || undefined
            }))

            if (response.succeeded) {
                saveSuccess.value = true
                // Reload the config to get the latest state
                await loadSiteConfig()
                setTimeout(() => {
                    saveSuccess.value = false
                }, 3000)
                return true
            } else {
                error.value = response.error?.message || 'Failed to update site configuration'
                return false
            }
        } catch (e: any) {
            error.value = e.message || 'An error occurred while saving site configuration'
            return false
        } finally {
            isSaving.value = false
        }
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
