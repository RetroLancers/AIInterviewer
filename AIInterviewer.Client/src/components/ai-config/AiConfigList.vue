<template>
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
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400 font-mono">
                        {{ config.modelId }}
                        <span v-if="config.fallbackModelId" class="text-xs text-gray-400 ml-1">(Fallback: {{ config.fallbackModelId }})</span>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <button @click="$emit('edit', config)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300 mr-4">Edit</button>
                        <button @click="$emit('delete', config)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">Delete</button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script setup lang="ts">
import { AiConfigResponse } from '@/lib/dtos';

defineProps<{
    configs: AiConfigResponse[];
    loading: boolean;
}>();

defineEmits<{
    (e: 'edit', config: AiConfigResponse): void;
    (e: 'delete', config: AiConfigResponse): void;
}>();
</script>
