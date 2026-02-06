<template>
  <div class="mx-auto max-w-5xl px-4 py-8 sm:px-6 lg:px-8">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-3xl font-bold text-gray-900 dark:text-gray-100">Interview #{{ id }}</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          {{ interview ? formatDate(interview.createdDate) : '' }}
        </p>
      </div>
      <div class="flex flex-wrap gap-2">
        <RouterLink
          to="/interviews/history"
          class="inline-flex items-center rounded-md border border-gray-300 dark:border-gray-600 px-3 py-1.5 text-sm font-medium text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-700"
        >
          Back to history
        </RouterLink>
        <RouterLink
          :to="`/interviews/${id}`"
          class="inline-flex items-center rounded-md border border-gray-300 dark:border-gray-600 px-3 py-1.5 text-sm font-medium text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-700"
        >
          Resume interview
        </RouterLink>
        <button
          type="button"
          class="inline-flex items-center rounded-md bg-blue-600 px-3 py-1.5 text-sm font-medium text-white shadow-sm hover:bg-blue-700"
          @click="startFromPrompt"
        >
          Start from prompt
        </button>
      </div>
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
          @click="fetchInterview"
        >
          Retry
        </button>
      </div>
    </div>

    <div v-else-if="interview" class="space-y-6">
      <div class="rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 p-5 shadow-sm">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-gray-100">System prompt</h2>
        <p class="mt-3 whitespace-pre-wrap text-sm text-gray-700 dark:text-gray-200">{{ interview.prompt }}</p>
      </div>

      <div class="rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 p-5 shadow-sm">
        <div class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-gray-100">Transcript</h2>
          <RouterLink
            v-if="result"
            :to="`/interviews/${id}/result`"
            class="text-sm font-semibold text-blue-600 hover:underline"
          >
            View feedback report
          </RouterLink>
        </div>
        <div class="mt-4 space-y-4">
          <div
            v-for="msg in history"
            :key="msg.id"
            class="flex"
            :class="msg.role === 'User' ? 'justify-end' : 'justify-start'"
          >
            <div
              :class="[
                'max-w-[80%] p-4 rounded-2xl shadow-sm',
                msg.role === 'User'
                  ? 'bg-blue-600 text-white rounded-tr-none'
                  : 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200 rounded-tl-none'
              ]"
            >
              <div class="text-xs opacity-75 mb-1 font-bold">{{ msg.role }}</div>
              <div class="whitespace-pre-wrap">{{ msg.content }}</div>
            </div>
          </div>
          <div v-if="history.length === 0" class="text-sm text-gray-500 dark:text-gray-400">
            No transcript entries recorded yet.
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { client } from '@/lib/gateway'
import { GetInterview, type InterviewDto, type InterviewChatHistoryDto, type InterviewResultDto } from '@/lib/dtos'

const route = useRoute('/interviews/history/[id]')
const router = useRouter()
const id = computed(() => Number(route.params.id))
const interview = ref<InterviewDto | null>(null)
const history = ref<InterviewChatHistoryDto[]>([])
const result = ref<InterviewResultDto | null>(null)
const loading = ref(true)
const error = ref('')

const fetchInterview = async () => {
  loading.value = true
  error.value = ''
  const api = await client.api(new GetInterview({ id: id.value }))
  if (api.succeeded && api.response) {
    interview.value = api.response.interview
    history.value = api.response.history ?? []
    result.value = api.response.result ?? null
  } else {
    error.value = api.error?.message || 'Failed to load interview.'
  }
  loading.value = false
}

const formatDate = (value: string | Date) => {
  return new Date(value).toLocaleString()
}

const startFromPrompt = () => {
  if (!interview.value) return
  router.push({ path: '/interviews/new', query: { prompt: interview.value.prompt } })
}

onMounted(() => {
  fetchInterview()
})
</script>
