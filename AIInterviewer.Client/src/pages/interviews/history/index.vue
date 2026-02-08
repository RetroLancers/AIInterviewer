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

    <InterviewHistoryList
      :interviews="interviews"
      :loading="loading"
      :error="error"
      @retry="fetchHistory"
      @start-from-prompt="startFromPrompt"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useInterviewHistory } from '@/composables/useInterviewHistory'
import InterviewHistoryList from '@/components/interview/InterviewHistoryList.vue'

const { interviews, loading, error, fetchHistory, startFromPrompt } = useInterviewHistory()

onMounted(() => {
  fetchHistory()
})
</script>
