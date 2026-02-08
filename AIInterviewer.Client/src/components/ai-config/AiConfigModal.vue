<template>
    <div v-if="show" class="relative z-10" aria-labelledby="modal-title" role="dialog" aria-modal="true">
        <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" @click="$emit('close')"></div>

        <div class="fixed inset-0 z-10 w-screen overflow-y-auto">
            <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
                <div class="relative transform overflow-hidden rounded-lg bg-white dark:bg-gray-800 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg" @click.stop>
                    <form @submit.prevent="$emit('save')">
                        <div class="bg-white dark:bg-gray-800 px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
                            <div class="sm:flex sm:items-start">
                                <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left w-full">
                                    <h3 class="text-base font-semibold leading-6 text-gray-900 dark:text-gray-100" id="modal-title">
                                        {{ isEditing ? 'Edit Configuration' : 'Add New Configuration' }}
                                    </h3>

                                    <AiConfigForm
                                        v-model="config"
                                        :available-models="availableModels"
                                        :loading-models="loadingModels"
                                        :available-voices="availableVoices"
                                        @fetch-models="$emit('fetchModels')"
                                        @provider-change="$emit('providerChange')"
                                    />

                                </div>
                            </div>
                        </div>
                        <div class="bg-gray-50 dark:bg-gray-700 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
                            <button type="submit" class="inline-flex w-full justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 sm:ml-3 sm:w-auto">Save</button>
                            <button type="button" @click="$emit('close')" class="mt-3 inline-flex w-full justify-center rounded-md bg-white dark:bg-gray-600 px-3 py-2 text-sm font-semibold text-gray-900 dark:text-white shadow-sm ring-1 ring-inset ring-gray-300 dark:ring-gray-500 hover:bg-gray-50 dark:hover:bg-gray-500 sm:mt-0 sm:w-auto">Cancel</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { AiConfigResponse } from '@/lib/dtos';
import AiConfigForm from './AiConfigForm.vue';

const config = defineModel<Partial<AiConfigResponse>>({ required: true });

defineProps<{
    show: boolean;
    isEditing: boolean;
    availableModels: string[];
    loadingModels: boolean;
    availableVoices: string[];
}>();

defineEmits<{
    (e: 'close'): void;
    (e: 'save'): void;
    (e: 'fetchModels'): void;
    (e: 'providerChange'): void;
}>();
</script>
