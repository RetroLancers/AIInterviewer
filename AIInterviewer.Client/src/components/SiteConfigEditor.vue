<template>
  <div class="site-config-editor max-w-4xl mx-auto p-6">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
      <h2 class="text-2xl font-bold mb-6 text-gray-900 dark:text-gray-100">Site Configuration</h2>

      <!-- Loading State -->
      <div v-if="isLoading || aiConfigsLoading" class="flex items-center justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>

      <!-- Error Message -->
      <div v-if="error || aiConfigsError" class="mb-4 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-md">
        <p class="text-sm text-red-800 dark:text-red-200">{{ error || aiConfigsError }}</p>
      </div>

      <!-- Success Message -->
      <div v-if="saveSuccess" class="mb-4 p-4 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-md">
        <p class="text-sm text-green-800 dark:text-green-200">Configuration saved successfully!</p>
      </div>

      <!-- Form -->
      <form v-if="!isLoading && siteConfig" @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Active AI Service Configuration -->
        <div>
          <label for="active-ai-config" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Active AI Service
          </label>
          <select
            id="active-ai-config"
            v-model="formData.activeAiConfigId"
            :disabled="aiConfigsLoading || aiConfigs.length === 0"
            class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-gray-100 disabled:bg-gray-100 dark:disabled:bg-gray-800 disabled:cursor-not-allowed"
            required
          >
            <option value="">Select an AI service</option>
            <option v-for="config in aiConfigs" :key="config.id" :value="config.id">
              {{ config.name }} ({{ config.providerType }} - {{ config.modelId }})
            </option>
          </select>
          <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
            Select the AI service configuration to use for interviews. Manage AI services in the AI Config page.
          </p>
          <div v-if="aiConfigsLoading" class="mt-2 flex items-center text-sm text-gray-500 dark:text-gray-400">
            <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600 mr-2"></div>
            Loading AI configurations...
          </div>
          <div v-if="aiConfigsError" class="mt-2 p-2 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded text-xs text-yellow-800 dark:text-yellow-200">
            {{ aiConfigsError }}
          </div>
        </div>

        <!-- Other AI Configuration -->
        <div class="border-t border-gray-200 dark:border-gray-700 pt-6">
          <h3 class="text-lg font-semibold mb-4 text-gray-900 dark:text-gray-100">Additional AI Settings</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">

            <div>
              <label for="global-fallback-model" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Global Fallback Model
              </label>
              <select
                id="global-fallback-model"
                v-model="formData.globalFallbackModel"
                :disabled="!availableModels || availableModels.length === 0"
                class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-gray-100 disabled:bg-gray-100 dark:disabled:bg-gray-800 disabled:cursor-not-allowed"
              >
                <option value="">Select a fallback model (optional)</option>
                <option v-for="model in availableModels" :key="model" :value="model">
                  {{ model }}
                </option>
              </select>
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                Used when the primary model fails or is overloaded.
              </p>
            </div>

            <div>
              <label for="transcription-provider" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Transcription Provider
              </label>
              <select
                id="transcription-provider"
                v-model="formData.transcriptionProvider"
                class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-gray-100"
                required
              >
                <option value="Gemini">Gemini (Server-side)</option>
                <option value="Browser">Browser (Client-side)</option>
              </select>
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                Choose whether speech is transcribed on the server or directly in the browser.
              </p>
            </div>
          </div>
        </div>

        <!-- Voice Configuration -->
        <div class="border-t border-gray-200 dark:border-gray-700 pt-6">
          <h3 class="text-lg font-semibold mb-4 text-gray-900 dark:text-gray-100">Voice Configuration (Kokoro TTS)</h3>
          
          <div>
            <label for="kokoro-voice" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Default Interviewer Voice
            </label>
            <select
              id="kokoro-voice"
              v-model="formData.defaultVoice"
              class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-gray-100"
            >
              <option value="af_heart">af_heart (Default)</option>
              <option value="af_bella">af_bella</option>
              <option value="af_nicole">af_nicole</option>
              <option value="af_sarah">af_sarah</option>
              <option value="af_sky">af_sky</option>
              <option value="am_adam">am_adam</option>
              <option value="am_michael">am_michael</option>
              <option value="bf_emma">bf_emma</option>
              <option value="bf_isabella">bf_isabella</option>
              <option value="bm_george">bm_george</option>
              <option value="bm_lewis">bm_lewis</option>
            </select>
            <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
              Select the default voice for the AI interviewer.
            </p>
          </div>
        </div>

        <!-- Submit Button -->
        <div class="flex items-center justify-end space-x-3 pt-4 border-t border-gray-200 dark:border-gray-700">
          <button
            type="submit"
            :disabled="isSaving"
            class="px-6 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors disabled:cursor-not-allowed"
          >
            <span v-if="!isSaving">Save Configuration</span>
            <span v-else class="flex items-center">
              <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
              Saving...
            </span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useSiteConfig } from '@/composables/useSiteConfig'
import { useAiConfigs } from '@/composables/useAiConfigs'

const { siteConfig, isLoading, isSaving, error, saveSuccess, saveSiteConfig } = useSiteConfig()
const { aiConfigs, isLoading: aiConfigsLoading, error: aiConfigsError } = useAiConfigs()

// Form data
const formData = ref({
  activeAiConfigId: 0,
  globalFallbackModel: '',
  defaultVoice: 'af_heart',
  transcriptionProvider: 'Gemini'
})

// Available models for fallback (unique models from all configs)
const availableModels = computed(() => {
  const models = new Set<string>()
  aiConfigs.value.forEach(config => {
    if (config.modelId) {
      models.add(config.modelId)
    }
  })
  return Array.from(models).sort()
})

// Watch for siteConfig changes and update form
watch(siteConfig, (newConfig) => {
  if (newConfig) {
    formData.value.activeAiConfigId = newConfig.activeAiConfigId || 0
    formData.value.globalFallbackModel = newConfig.globalFallbackModel || ''
    formData.value.defaultVoice = newConfig.defaultVoice || 'af_heart'
    formData.value.transcriptionProvider = newConfig.transcriptionProvider || 'Gemini'
  }
}, { immediate: true })

const handleSubmit = async () => {
  await saveSiteConfig(
    formData.value.activeAiConfigId,
    formData.value.globalFallbackModel || undefined,
    formData.value.defaultVoice,
    formData.value.transcriptionProvider
  )
}
</script>
