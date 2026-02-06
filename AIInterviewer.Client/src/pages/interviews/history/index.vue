<template>
  <div class="mx-auto max-w-5xl px-4 py-8 sm:px-6 lg:px-8">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-3xl font-bold text-gray-900 dark:text-gray-100">Interview History</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Review your past interview sessions and reuse their prompts.
        </p>
      </div>
      <RouterLink
        to="/interviews/new"
        class="inline-flex items-center justify-center rounded-md bg-green-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-green-700 transition-colors"
      >
        Start New Interview
      </RouterLink>
    </div>

    <div v-if="loading" class="flex justify-center py-10">
      <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-gray-900 dark:border-gray-200"></div>
    </div>

    <div v-else-if="error" class="rounded-md border border-red-200 bg-red-50 p-4 text-red-700 dark:border-red-900/40 dark:bg-red-900/20 dark:text-red-300">
      <div class="flex items-center justify-between gap-4">
        <span>{{ error }}</span>
        <button
          type="button"
          class="text-sm font-semibold underline"
          @click="fetchHistory"
        >
          Retry
        </button>
      </div>
    </div>

    <div v-else-if="interviews.length === 0" class="rounded-lg border border-dashed border-gray-300 dark:border-gray-700 p-8 text-center">
      <p class="text-gray-600 dark:text-gray-300">No interview history yet.</p>
      <RouterLink
        to="/interviews/new"
        class="mt-4 inline-flex items-center justify-center rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 transition-colors"
      >
        Create your first interview
      </RouterLink>
    </div>

    <div v-else class="space-y-4">
      <div
        v-for="interview in interviews"
        :key="interview.id"
        class="rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 p-5 shadow-sm"
      >
        <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h2 class="text-lg font-semibold text-gray-900 dark:text-gray-100">
              Interview #{{ interview.id }}
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              {{ formatDate(interview.createdDate) }}
            </p>
          </div>
          <div class="flex flex-wrap gap-2">
            <RouterLink
              :to="`/interviews/history/${interview.id}`"
              class="inline-flex items-center rounded-md border border-gray-300 dark:border-gray-600 px-3 py-1.5 text-sm font-medium text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-700"
            >
              View Transcript
            </RouterLink>
            <RouterLink
              :to="`/interviews/${interview.id}`"
              class="inline-flex items-center rounded-md border border-gray-300 dark:border-gray-600 px-3 py-1.5 text-sm font-medium text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-700"
            >
              Resume
            </RouterLink>
            <button
              type="button"
              class="inline-flex items-center rounded-md bg-blue-600 px-3 py-1.5 text-sm font-medium text-white shadow-sm hover:bg-blue-700"
              @click="startFromPrompt(interview.prompt)"
            >
              Start from prompt
            </button>
          </div>
        </div>
        <div class="mt-4 rounded-md bg-gray-50 dark:bg-gray-900/40 p-4 text-sm text-gray-700 dark:text-gray-200">
          <p class="font-medium text-gray-600 dark:text-gray-300">Prompt preview</p>
          <p class="mt-1 whitespace-pre-wrap">{{ promptPreview(interview.prompt) }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { client } from '@/lib/gateway'
import { GetInterviewHistory, type InterviewDto } from '@/lib/dtos'

const router = useRouter()
const interviews = ref<InterviewDto[]>([])
const loading = ref(true)
const error = ref('')

const fetchHistory = async () => {
  loading.value = true
  error.value = ''
  const api = await client.api(new GetInterviewHistory())
  if (api.succeeded && api.response) {
    interviews.value = api.response.interviews ?? []
  } else {
    error.value = api.error?.message || 'Failed to load interview history.'
  }
  loading.value = false
}

const formatDate = (value: string | Date) => {
  return new Date(value).toLocaleString()
}

const promptPreview = (prompt: string) => {
  const trimmed = prompt.trim()
  if (trimmed.length <= 240) return trimmed
  return `${trimmed.slice(0, 240)}...`
}

const startFromPrompt = (prompt: string) => {
  router.push({ path: '/interviews/new', query: { prompt } })
}

onMounted(() => {
  fetchHistory()
})
</script>
