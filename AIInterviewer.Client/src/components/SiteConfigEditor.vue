<template>
  <div class="site-config-editor max-w-4xl mx-auto p-6">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
      <h2 class="text-2xl font-bold mb-6 text-gray-900 dark:text-gray-100">Site Configuration</h2>

      <!-- Loading State -->
      <div v-if="isLoading" class="flex items-center justify-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>

      <!-- Error Message -->
      <div v-if="error" class="mb-4 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-md">
        <p class="text-sm text-red-800 dark:text-red-200">{{ error }}</p>
      </div>

      <!-- Success Message -->
      <div v-if="saveSuccess" class="mb-4 p-4 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-md">
        <p class="text-sm text-green-800 dark:text-green-200">Configuration saved successfully!</p>
      </div>

      <!-- Form -->
      <form v-if="!isLoading && siteConfig" @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Gemini API Key Input -->
        <div>
          <label for="gemini-api-key" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Gemini API Key
          </label>
          <div class="relative">
            <input
              :type="showApiKey ? 'text' : 'password'"
              id="gemini-api-key"
              v-model="formData.geminiApiKey"
              class="w-full px-4 py-2 pr-12 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-gray-100"
              placeholder="Enter your Gemini API key"
              required
            />
            <button
              type="button"
              @click="showApiKey = !showApiKey"
              class="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
            >
              <svg v-if="!showApiKey" class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
              <svg v-else class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Loading/Error states for models -->
        <div v-if="modelsLoading" class="flex items-center text-sm text-gray-500 dark:text-gray-400 mb-2">
          <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600 mr-2"></div>
          Loading available models...
        </div>

        <div v-if="modelsError" class="mb-2 p-2 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded text-xs text-yellow-800 dark:text-yellow-200">
          {{ modelsError }}
        </div>

        <!-- Interview Model Configuration -->
        <div class="border-t border-gray-200 dark:border-gray-700 pt-6">
          <h3 class="text-lg font-semibold mb-4 text-gray-900 dark:text-gray-100">AI Interview Configuration</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="interview-model" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Interview Model
              </label>
              <select
                id="interview-model"
                v-model="formData.interviewModel"
                :disabled="!availableModels || availableModels.length === 0"
                class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-gray-100 disabled:bg-gray-100 dark:disabled:bg-gray-800 disabled:cursor-not-allowed"
                required
              >
                <option value="">Select a model</option>
                <option v-for="model in availableModels" :key="model" :value="model">
                  {{ model }}
                </option>
              </select>
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                The primary model used for conducting interviews.
              </p>
            </div>

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
              v-model="formData.kokoroVoice"
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
import { ref, watch } from 'vue'
import { useSiteConfig } from '@/composables/useSiteConfig'
import { useGeminiModels } from '@/composables/useGeminiModels'

const { siteConfig, isLoading, isSaving, error, saveSuccess, saveSiteConfig } = useSiteConfig()

// Form data
const formData = ref({
  geminiApiKey: '',
  interviewModel: '',
  globalFallbackModel: '',
  kokoroVoice: 'af_heart',
  transcriptionProvider: 'Gemini'
})

// Show/hide API key toggle
const showApiKey = ref(false)

// Watch for siteConfig changes and update form
watch(siteConfig, (newConfig) => {
  if (newConfig) {
    formData.value.geminiApiKey = newConfig.geminiApiKey || ''
    formData.value.interviewModel = newConfig.interviewModel || ''
    formData.value.globalFallbackModel = newConfig.globalFallbackModel || ''
    formData.value.kokoroVoice = newConfig.kokoroVoice || 'af_heart'
    formData.value.transcriptionProvider = newConfig.transcriptionProvider || 'Gemini'
  }
}, { immediate: true })

// Fetch Gemini models based on the API key
const { models: availableModels, isLoading: modelsLoading, error: modelsError } = useGeminiModels(
  () => formData.value.geminiApiKey
)

const handleSubmit = async () => {
  await saveSiteConfig(
    formData.value.geminiApiKey,
    formData.value.interviewModel,
    formData.value.globalFallbackModel || undefined,
    formData.value.kokoroVoice,
    formData.value.transcriptionProvider
  )
}
</script>
