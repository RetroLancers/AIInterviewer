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

    async function loadConfigs() {
        loading.value = true;
        try {
            const response = await client.api(new ListAiConfigs());
            if (response.succeeded && response.response) {
                configs.value = response.response.configs || [];
            } else {
                configs.value = [];
            }
        } catch (e) {
            console.error('Failed to load configs', e);
        } finally {
            loading.value = false;
        }
    }

    async function saveConfig(config: Partial<AiConfigResponse>) {
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
            throw e;
        }
    }

    async function deleteConfig(configId: number) {
        try {
            await client.api(new DeleteAiConfig({ id: configId }));
            await loadConfigs();
        } catch (e) {
            throw e;
        }
    }

    return {
        configs,
        loading,
        loadConfigs,
        saveConfig,
        deleteConfig
    };
}
