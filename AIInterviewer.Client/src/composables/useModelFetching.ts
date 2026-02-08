import { ref } from 'vue';
import { useClient } from '@servicestack/vue';
import { GetAiModels } from '@/lib/dtos';

export function useModelFetching() {
    const client = useClient();
    const availableModels = ref<string[]>([]);
    const loadingModels = ref(false);

    async function fetchModels(providerType: string, apiKey: string) {
        if (!apiKey || !providerType) return;

        loadingModels.value = true;
        try {
            const response = await client.api(new GetAiModels({
                providerType,
                apiKey
            }));

            if (response.succeeded && response.response) {
                availableModels.value = response.response.models || [];
            } else {
                throw new Error('Failed to fetch models');
            }
        } catch (e) {
            console.error('Error fetching models', e);
            throw e;
        } finally {
            loadingModels.value = false;
        }
    }

    return {
        availableModels,
        loadingModels,
        fetchModels
    };
}
