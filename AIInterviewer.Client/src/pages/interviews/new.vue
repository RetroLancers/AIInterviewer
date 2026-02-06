<template>
  <div class="mx-auto mt-8 max-w-3xl px-4 sm:px-6 lg:px-8">
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg overflow-hidden">
      <div class="px-4 py-5 sm:p-6">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6 flex items-center">
            <Iconify icon="mdi:robot-excited-outline" class="mr-3 text-green-600 w-8 h-8"/>
            Create New Interview
        </h1>

        <div class="space-y-6">
          <!-- Error State -->
          <div v-if="error" class="p-4 bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 rounded-md">
            <div class="flex">
              <Iconify icon="mdi:alert-circle" class="h-5 w-5 text-red-400" />
              <div class="ml-3">
                <h3 class="text-sm font-medium text-red-800 dark:text-red-300">Error</h3>
                <div class="mt-1 text-sm text-red-700 dark:text-red-400">
                  {{ error }}
                </div>
              </div>
            </div>
          </div>

          <!-- Step 1: Prompt Source -->
          <div class="space-y-3">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Prompt Source
            </label>
            <div class="flex flex-col gap-3 sm:flex-row">
              <label class="flex items-center gap-2 rounded-md border border-gray-200 dark:border-gray-700 px-3 py-2 text-sm text-gray-700 dark:text-gray-300">
                <input
                  v-model="promptSource"
                  type="radio"
                  value="generate"
                  class="text-blue-600 focus:ring-blue-500"
                />
                Generate a prompt with AI
              </label>
              <label class="flex items-center gap-2 rounded-md border border-gray-200 dark:border-gray-700 px-3 py-2 text-sm text-gray-700 dark:text-gray-300">
                <input
                  v-model="promptSource"
                  type="radio"
                  value="custom"
                  class="text-blue-600 focus:ring-blue-500"
                />
                Paste your own prompt
              </label>
            </div>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              Choose whether to generate a prompt or supply your own system prompt directly.
            </p>
          </div>

          <!-- Step 2: Inputs -->
          <div v-if="promptSource === 'generate'" class="grid grid-cols-1 gap-6">
            <div>
              <label for="targetRole" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                Target Role
              </label>
              <div class="mt-1">
                <input
                  type="text"
                  id="targetRole"
                  v-model="targetRole"
                  placeholder="e.g. Senior Fullstack Developer"
                  class="block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 dark:text-white shadow-sm focus:border-green-500 focus:ring-green-500 sm:text-sm p-3 border"
                  @keyup.enter="generatePrompt"
                />
              </div>
            </div>

            <div>
              <label for="context" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                Additional Context (Optional)
              </label>
              <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                Mention specific technologies, company culture, or scenario details.
              </p>
              <div class="mt-1">
                <textarea
                  id="context"
                  rows="3"
                  v-model="context"
                  placeholder="e.g. Focus on Vue 3, TypeScript, and AWS architecture. The company is a mid-stage fintech startup."
                  class="block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 dark:text-white shadow-sm focus:border-green-500 focus:ring-green-500 sm:text-sm p-3 border"
                ></textarea>
              </div>
            </div>
          </div>

          <!-- Generate Button -->
          <div v-if="promptSource === 'generate'" class="flex justify-end">
            <button
              type="button"
              @click="generatePrompt"
              :disabled="loading || !targetRole"
              class="inline-flex items-center rounded-md border border-transparent bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              <Iconify v-if="loading" icon="line-md:loading-twotone-loop" class="mr-2 h-5 w-5" />
              <Iconify v-else icon="mdi:magic-staff" class="mr-2 h-5 w-5" />
              Generate Prompt
            </button>
          </div>

          <!-- Step 2: Generated Prompt Review -->
          <div v-if="promptSource === 'custom' || systemPrompt || loading" class="relative pt-6 border-t border-gray-200 dark:border-gray-700">
            <div v-if="loading" class="absolute inset-0 bg-white/50 dark:bg-gray-800/50 flex flex-col items-center justify-center z-10 rounded-md">
                 <Iconify icon="line-md:loading-twotone-loop" class="h-12 w-12 text-blue-500 mb-2" />
                 <span class="text-sm font-medium text-gray-700 dark:text-gray-300">AI is crafting your interview...</span>
            </div>
            
            <label for="prompt" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Interview System Prompt
            </label>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              {{ promptSource === 'custom' ? 'Paste your system prompt below or write your own from scratch.' : 'Review and edit the system prompt that will guide the AI interviewer.' }}
            </p>
            <div class="mt-2">
              <textarea
                id="prompt"
                rows="10"
                v-model="systemPrompt"
                class="block w-full rounded-md border-gray-300 dark:border-gray-600 dark:bg-gray-700 dark:text-white shadow-sm focus:border-green-500 focus:ring-green-500 sm:text-sm p-3 font-mono text-xs border"
              ></textarea>
            </div>
          </div>

          <!-- Start Interview Action -->
          <div v-if="systemPrompt" class="border-t border-gray-200 dark:border-gray-700 pt-6 flex justify-end">
             <button
              type="button"
              @click="startInterview"
              :disabled="starting"
              class="inline-flex items-center rounded-md border border-transparent bg-green-600 px-6 py-3 text-base font-medium text-white shadow-sm hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 disabled:opacity-50 transition-colors"
            >
              <Iconify v-if="starting" icon="line-md:loading-twotone-loop" class="mr-2 h-5 w-5" />
              <span>Start Interview</span>
              <Iconify v-if="!starting" icon="mdi:arrow-right" class="ml-2 h-5 w-5" />
            </button>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Quick Tips / Info -->
    <div class="mt-8 grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div class="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
            <h3 class="font-semibold text-blue-800 dark:text-blue-300 mb-1 flex items-center"><Iconify icon="mdi:lightbulb-on-outline" class="mr-1"/> Be Specific</h3>
            <p class="text-sm text-blue-700 dark:text-blue-400">The more details you provide about the role and company, the more realistic the interview will be.</p>
        </div>
        <div class="p-4 bg-purple-50 dark:bg-purple-900/20 rounded-lg">
             <h3 class="font-semibold text-purple-800 dark:text-purple-300 mb-1 flex items-center"><Iconify icon="mdi:file-edit-outline" class="mr-1"/> Edit Freely</h3>
            <p class="text-sm text-purple-700 dark:text-purple-400">You can manually tweak the generated prompt to add specific questions or constraints.</p>
        </div>
         <div class="p-4 bg-orange-50 dark:bg-orange-900/20 rounded-lg">
             <h3 class="font-semibold text-orange-800 dark:text-orange-300 mb-1 flex items-center"><Iconify icon="mdi:microphone" class="mr-1"/> Voice Ready</h3>
            <p class="text-sm text-orange-700 dark:text-orange-400">The interview supports voice interaction. Ensure your microphone is ready!</p>
        </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { client } from '@/lib/gateway'
import { GenerateInterviewPrompt, CreateInterview } from '@/lib/dtos'

const router = useRouter()

const targetRole = ref('')
const context = ref('')
const promptSource = ref<'generate' | 'custom'>('generate')
const systemPrompt = ref('')
const loading = ref(false)
const starting = ref(false)
const error = ref<string | null>(null)

async function generatePrompt() {
    if (!targetRole.value) return
    
    loading.value = true
    error.value = null
    
    const api = await client.api(new GenerateInterviewPrompt({ 
        targetRole: targetRole.value,
        context: context.value 
    }))

    if (api.succeeded && api.response) {
        systemPrompt.value = api.response.systemPrompt || ''
    } else {
        error.value = api.error?.message || 'Failed to generate prompt. Please try again.'
    }
    
    loading.value = false
}

async function startInterview() {
    if (!systemPrompt.value) return
    
    starting.value = true
    error.value = null

    const api = await client.api(new CreateInterview({
        systemPrompt: systemPrompt.value
    }))

    if (api.succeeded && api.response) {
        router.push(`/interviews/${api.response.id}`)
    } else {
        error.value = api.error?.message || 'Failed to create interview. Please try again.'
    }

    starting.value = false
}
</script>
