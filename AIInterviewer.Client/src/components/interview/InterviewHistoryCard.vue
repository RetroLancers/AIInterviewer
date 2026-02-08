<template>
  <div class="rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 p-5 shadow-sm">
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
          @click="$emit('startFromPrompt', interview.prompt)"
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
</template>

<script setup lang="ts">
import { type InterviewDto } from '@/lib/dtos'
import { formatDate, promptPreview } from '@/composables/useInterviewHistory'

defineProps<{
  interview: InterviewDto
}>()

defineEmits(['startFromPrompt'])
</script>
