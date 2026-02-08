<template>
  <div v-if="loading" class="flex justify-center py-10">
    <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-gray-900 dark:border-gray-200"></div>
  </div>

  <div v-else-if="error" class="rounded-md border border-red-200 bg-red-50 p-4 text-red-700 dark:border-red-900/40 dark:bg-red-900/20 dark:text-red-300">
    <div class="flex items-center justify-between gap-4">
      <span>{{ error }}</span>
      <button
        type="button"
        class="text-sm font-semibold underline"
        @click="$emit('retry')"
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
    <InterviewHistoryCard
      v-for="interview in interviews"
      :key="interview.id"
      :interview="interview"
      @start-from-prompt="(prompt) => $emit('startFromPrompt', prompt)"
    />
  </div>
</template>

<script setup lang="ts">
import { type InterviewDto } from '@/lib/dtos'
import InterviewHistoryCard from './InterviewHistoryCard.vue'

defineProps<{
  interviews: InterviewDto[]
  loading: boolean
  error: string
}>()

defineEmits(['retry', 'startFromPrompt'])
</script>
