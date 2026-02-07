<template>
    <div class="px-4 py-6 sm:px-0">
        <div class="flex justify-between items-center mb-6">
            <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">AI Configurations</h1>
            <button @click="openAddModal" class="rounded-md bg-indigo-600 px-3.5 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">
                Add New Configuration
            </button>
        </div>

        <!-- Configurations List -->
        <div v-if="loading" class="text-center py-4">
            <p class="text-gray-500">Loading configurations...</p>
        </div>
        <div v-else-if="configs.length === 0" class="text-center py-12 bg-white dark:bg-gray-800 rounded-lg shadow">
            <p class="text-gray-500 dark:text-gray-400">No AI configurations found.</p>
            <p class="mt-2 text-sm text-gray-400">Click "Add New Configuration" to get started.</p>
        </div>
        <div v-else class="overflow-x-auto bg-white dark:bg-gray-800 shadow rounded-lg">
            <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-900">
                    <tr>
                        <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Name</th>
                        <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Provider</th>
                        <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Model ID</th>
                        <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Actions</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
                    <tr v-for="config in configs" :key="config.id" class="hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors duration-150">
                        <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">{{ config.name }}</td>
                        <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                            <span class="inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset" 
                                  :class="config.providerType === 'Gemini' ? 'bg-blue-50 text-blue-700 ring-blue-700/10' : 'bg-green-50 text-green-700 ring-green-600/20'">
                                {{ config.providerType }}
                            </span>
                        </td>
                        <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400 font-mono">{{ config.modelId }}</td>
                        <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                            <button @click="editConfig(config)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300 mr-4">Edit</button>
                            <button @click="confirmDelete(config)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">Delete</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Add/Edit Modal -->
        <div v-if="showModal" class="relative z-10" aria-labelledby="modal-title" role="dialog" aria-modal="true">
            <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" @click="closeModal"></div>

            <div class="fixed inset-0 z-10 w-screen overflow-y-auto">
                <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
                    <div class="relative transform overflow-hidden rounded-lg bg-white dark:bg-gray-800 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg" @click.stop>
                        <form @submit.prevent="saveConfig">
                            <div class="bg-white dark:bg-gray-800 px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
                                <div class="sm:flex sm:items-start">
                                    <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left w-full">
                                        <h3 class="text-base font-semibold leading-6 text-gray-900 dark:text-gray-100" id="modal-title">
                                            {{ isEditing ? 'Edit Configuration' : 'Add New Configuration' }}
                                        </h3>
                                        <div class="mt-4 space-y-4">
                                            <div>
                                                <label for="name" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Name</label>
                                                <div class="mt-1">
                                                    <input type="text" v-model="form.name" id="name" required class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600" placeholder="My Gemini Config">
                                                </div>
                                            </div>

                                            <div>
                                                <label for="provider" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Provider</label>
                                                <div class="mt-1">
                                                    <select v-model="form.providerType" id="provider" required class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                                                        <option value="Gemini">Gemini</option>
                                                        <option value="OpenAI">OpenAI</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div>
                                                <label for="apiKey" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">API Key</label>
                                                <div class="mt-1">
                                                    <input type="password" v-model="form.apiKey" id="apiKey" required class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                                                </div>
                                            </div>

                                            <div>
                                                <label for="modelId" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Model ID</label>
                                                <div class="mt-1">
                                                    <input type="text" v-model="form.modelId" id="modelId" required class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600" placeholder="gemini-1.5-flash">
                                                </div>
                                                <p class="mt-1 text-xs text-gray-500">e.g. gemini-1.5-flash or gpt-4o</p>
                                            </div>
                                            
                                            <div>
                                                <label for="baseUrl" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Base URL (Optional)</label>
                                                <div class="mt-1">
                                                    <input type="text" v-model="form.baseUrl" id="baseUrl" class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="bg-gray-50 dark:bg-gray-700 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
                                <button type="submit" class="inline-flex w-full justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 sm:ml-3 sm:w-auto">Save</button>
                                <button type="button" @click="closeModal" class="mt-3 inline-flex w-full justify-center rounded-md bg-white dark:bg-gray-600 px-3 py-2 text-sm font-semibold text-gray-900 dark:text-white shadow-sm ring-1 ring-inset ring-gray-300 dark:ring-gray-500 hover:bg-gray-50 dark:hover:bg-gray-500 sm:mt-0 sm:w-auto">Cancel</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useClient } from '@servicestack/vue';
import { 
    ListAiConfigs, 
    CreateAiConfig, 
    UpdateAiConfig, 
    DeleteAiConfig,
    AiConfigResponse
} from '@/lib/dtos';

const client = useClient();
const configs = ref<AiConfigResponse[]>([]);
const loading = ref(true);
const showModal = ref(false);
const isEditing = ref(false);

const form = ref({
    id: 0,
    name: '',
    providerType: 'Gemini',
    apiKey: '',
    modelId: '',
    baseUrl: ''
});

async function loadConfigs() {
    loading.value = true;
    try {
        const response = await client.api(new ListAiConfigs());
        configs.value = response.response || [];
    } catch (e) {
        console.error('Failed to load configs', e);
    } finally {
        loading.value = false;
    }
}

function openAddModal() {
    isEditing.value = false;
    form.value = {
        id: 0,
        name: '',
        providerType: 'Gemini',
        apiKey: '',
        modelId: '',
        baseUrl: ''
    };
    showModal.value = true;
}

function editConfig(config: AiConfigResponse) {
    isEditing.value = true;
    form.value = { ...config, baseUrl: config.baseUrl || '' };
    showModal.value = true;
}

function closeModal() {
    showModal.value = false;
}

async function saveConfig() {
    try {
        if (isEditing.value) {
            await client.api(new UpdateAiConfig({
                id: form.value.id,
                name: form.value.name,
                providerType: form.value.providerType,
                apiKey: form.value.apiKey,
                modelId: form.value.modelId,
                baseUrl: form.value.baseUrl || undefined
            }));
        } else {
            await client.api(new CreateAiConfig({
                name: form.value.name,
                providerType: form.value.providerType,
                apiKey: form.value.apiKey,
                modelId: form.value.modelId,
                baseUrl: form.value.baseUrl || undefined
            }));
        }
        await loadConfigs();
        closeModal();
    } catch (e) {
        alert('Failed to save configuration: ' + (e as any).message);
    }
}

async function confirmDelete(config: AiConfigResponse) {
    if (confirm(`Are you sure you want to delete configuration "${config.name}"?`)) {
        try {
            await client.api(new DeleteAiConfig({ id: config.id }));
            await loadConfigs();
        } catch (e) {
             alert('Failed to delete configuration: ' + (e as any).message);
        }
    }
}

onMounted(() => {
    loadConfigs();
});
</script>
