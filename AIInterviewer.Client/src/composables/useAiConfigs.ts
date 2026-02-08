import { ref } from 'vue';
import { useClient } from '@servicestack/vue';
import {
    ListAiConfigs,
    CreateAiConfig,
    UpdateAiConfig,
    DeleteAiConfig,
    AiConfigResponse
} from '@/lib/dtos';

export function useAiConfigs() {
    const client = useClient();
    const configs = ref<AiConfigResponse[]>([]);
    const loading = ref(true);
    const error = ref<string | null>(null);

    async function loadConfigs() {
        loading.value = true;
        error.value = null;
        try {
            const response = await client.api(new ListAiConfigs());
            if (response.succeeded && response.response) {
                configs.value = response.response.configs || [];
            } else {
                configs.value = [];
                error.value = response.errorMessage || 'Failed to load configurations';
            }
        } catch (e) {
            console.error('Failed to load configs', e);
            error.value = (e as any).message || 'An unexpected error occurred';
        } finally {
            loading.value = false;
        }
    }

    async function saveConfig(config: Partial<AiConfigResponse>) {
        error.value = null;
        try {
            const payload = {
                name: config.name || '',
                providerType: config.providerType || 'Gemini',
                apiKey: config.apiKey || '',
                modelId: config.modelId || '',
                fallbackModelId: config.fallbackModelId,
                voice: config.voice
            };

            if (config.id && config.id > 0) {
                await client.api(new UpdateAiConfig({
                    id: config.id,
                    ...payload
                }));
            } else {
                await client.api(new CreateAiConfig(payload));
            }
            await loadConfigs();
        } catch (e) {
            error.value = (e as any).message || 'Failed to save configuration';
            throw e;
        }
    }

    async function deleteConfig(configId: number) {
        error.value = null;
        try {
            await client.api(new DeleteAiConfig({ id: configId }));
            await loadConfigs();
        } catch (e) {
            error.value = (e as any).message || 'Failed to delete configuration';
            throw e;
        }
    }

    return {
        configs,
        loading,
        error,
        loadConfigs,
        saveConfig,
        deleteConfig
    };
}
