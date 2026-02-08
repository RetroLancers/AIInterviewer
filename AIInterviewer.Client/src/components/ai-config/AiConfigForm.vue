<template>
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
                <select v-model="form.providerType" id="provider" required @change="$emit('providerChange')" class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                    <option value="Gemini">Gemini</option>
                    <option value="OpenAI">OpenAI</option>
                </select>
            </div>
        </div>

        <div>
            <label for="apiKey" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">API Key</label>
            <div class="mt-1 flex gap-2">
                <input type="password" v-model="form.apiKey" id="apiKey" required class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                <button type="button" @click="$emit('fetchModels')" :disabled="loadingModels || !form.apiKey" class="inline-flex items-center rounded-md bg-white dark:bg-gray-700 px-3 py-2 text-sm font-semibold text-gray-900 dark:text-white shadow-sm ring-1 ring-inset ring-gray-300 dark:ring-gray-600 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50">
                    <span v-if="loadingModels">...</span>
                    <span v-else>Fetch</span>
                </button>
            </div>
        </div>

        <div>
            <label for="modelId" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Model ID</label>
            <div class="mt-1">
                <select v-if="availableModels.length > 0" v-model="form.modelId" id="modelId" required class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                    <option v-for="model in availableModels" :key="model" :value="model">{{ model }}</option>
                </select>
                <input v-else type="text" v-model="form.modelId" id="modelId" required class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600" placeholder="gemini-1.5-flash">
            </div>
            <p v-if="availableModels.length === 0" class="mt-1 text-xs text-gray-500">Enter API key and click "Fetch" to see available models.</p>
        </div>

        <div>
            <label for="fallbackModelId" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Fallback Model ID (Optional)</label>
            <div class="mt-1">
                <select v-if="availableModels.length > 0" v-model="form.fallbackModelId" id="fallbackModelId" class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                    <option :value="undefined">None</option>
                    <option v-for="model in availableModels" :key="'fallback-'+model" :value="model">{{ model }}</option>
                </select>
                <input v-else type="text" v-model="form.fallbackModelId" id="fallbackModelId" class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600" placeholder="gemini-1.5-pro">
            </div>
        </div>

        <div>
            <label for="voice" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Voice (Optional)</label>
            <div class="mt-1">
                <select v-model="form.voice" id="voice" class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                    <option :value="undefined">Use Site Default</option>
                    <option v-for="voice in availableVoices" :key="voice" :value="voice">{{ voice }}</option>
                </select>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { AiConfigResponse } from '@/lib/dtos';

const form = defineModel<Partial<AiConfigResponse>>({ required: true });

defineProps<{
    availableModels: string[];
    loadingModels: boolean;
    availableVoices: string[];
}>();

defineEmits<{
    (e: 'fetchModels'): void;
    (e: 'providerChange'): void;
}>();
</script>
