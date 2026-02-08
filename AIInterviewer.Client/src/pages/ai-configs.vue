<template>
    <div class="px-4 py-6 sm:px-0">
        <div class="flex justify-between items-center mb-6">
            <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">AI Configurations</h1>
            <button @click="openAddModal" class="rounded-md bg-indigo-600 px-3.5 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">
                Add New Configuration
            </button>
        </div>

        <AiConfigList
            :configs="configs"
            :loading="loading"
            @edit="openEditModal"
            @delete="confirmDelete"
        />

        <AiConfigModal
            :show="showModal"
            :is-editing="isEditing"
            v-model="form"
            :available-models="availableModels"
            :loading-models="loadingModels"
            :available-voices="availableVoices"
            @close="closeModal"
            @save="saveConfigHandler"
            @fetch-models="fetchModelsHandler"
            @provider-change="onProviderChange"
        />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { AiConfigResponse } from '@/lib/dtos';
import { useAiConfigs } from '@/composables/useAiConfigs';
import { useModelFetching } from '@/composables/useModelFetching';
import AiConfigList from '@/components/ai-config/AiConfigList.vue';
import AiConfigModal from '@/components/ai-config/AiConfigModal.vue';

const { configs, loading, loadConfigs, saveConfig, deleteConfig } = useAiConfigs();
const { availableModels, loadingModels, fetchModels } = useModelFetching();

const showModal = ref(false);
const isEditing = ref(false);

const availableVoices = [
    'af_heart', 'af_bella', 'af_nicole', 'af_sarah', 'af_sky',
    'am_adam', 'am_michael',
    'bf_emma', 'bf_isabella',
    'bm_george', 'bm_lewis'
];

const form = ref<Partial<AiConfigResponse>>({
    id: 0,
    name: '',
    providerType: 'Gemini',
    apiKey: '',
    modelId: '',
    fallbackModelId: undefined,
    voice: 'af_heart'
});

async function fetchModelsHandler() {
    if (!form.value.apiKey || !form.value.providerType) return;
    
    try {
        await fetchModels(form.value.providerType, form.value.apiKey);
        if (availableModels.value.length > 0 && !form.value.modelId) {
            form.value.modelId = availableModels.value[0];
        }
    } catch (e) {
        alert('Failed to fetch models: ' + (e as any).message);
    }
}

function onProviderChange() {
    availableModels.value = [];
    form.value.modelId = '';
    form.value.fallbackModelId = undefined;
}

function openAddModal() {
    isEditing.value = false;
    availableModels.value = [];
    form.value = {
        id: 0,
        name: '',
        providerType: 'Gemini',
        apiKey: '',
        modelId: '',
        fallbackModelId: undefined,
        voice: 'af_heart'
    };
    showModal.value = true;
}

function openEditModal(config: AiConfigResponse) {
    isEditing.value = true;
    availableModels.value = [];
    form.value = { 
        ...config, 
        fallbackModelId: config.fallbackModelId || undefined,
        voice: config.voice || 'af_heart' 
    };
    showModal.value = true;
    // Attempt to fetch models if API key is present
    if (form.value.apiKey) {
        fetchModelsHandler();
    }
}

function closeModal() {
    showModal.value = false;
}

async function saveConfigHandler() {
    try {
        await saveConfig(form.value);
        closeModal();
    } catch (e) {
        alert('Failed to save configuration: ' + (e as any).message);
    }
}

async function confirmDelete(config: AiConfigResponse) {
    if (confirm(`Are you sure you want to delete configuration "${config.name}"?`)) {
        try {
            await deleteConfig(config.id);
        } catch (e) {
             alert('Failed to delete configuration: ' + (e as any).message);
        }
    }
}

onMounted(() => {
    loadConfigs();
});
</script>
