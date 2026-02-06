<template>
  <div class="max-w-4xl mx-auto p-6">
    <div v-if="loading" class="flex justify-center p-8">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
    </div>
    <div v-else-if="error" class="bg-red-50 text-red-600 p-4 rounded">
      {{ error }}
      <button @click="fetchResult" class="ml-4 underline">Retry</button>
    </div>
    <div v-else-if="result" class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="mb-8 text-center">
        <h1 class="text-3xl font-bold mb-2">Interview Result</h1>
        <p class="text-gray-500 dark:text-gray-400">Completed on {{ new Date(result.createdDate).toLocaleDateString() }}</p>
      </div>

      <div class="flex flex-col md:flex-row gap-8 mb-8">
        <div class="flex-shrink-0 mx-auto md:mx-0">
          <div class="w-32 h-32 rounded-full border-4 flex items-center justify-center text-4xl font-bold"
               :class="getScoreClass(result.score)">
            {{ result.score }}
          </div>
          <p class="text-center mt-2 font-medium">Score</p>
        </div>
        
        <div class="flex-grow">
          <h2 class="text-xl font-semibold mb-4">Feedback Report</h2>
          <div class="prose dark:prose-invert max-w-none" v-html="renderedReport"></div>
        </div>
      </div>

      <div class="border-t pt-6 flex justify-between items-center">
          <router-link :to="`/interviews/${id}`" class="text-blue-600 hover:underline">View Transcript</router-link>
          
          <router-link to="/" class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors">Start New Interview</router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { client } from '@/lib/gateway'
import { GetInterview, type InterviewResultDto } from '@/lib/dtos'
import MarkdownIt from 'markdown-it'

const route = useRoute()
const id = parseInt(route.params.id as string)

const result = ref<InterviewResultDto | null>(null)
const loading = ref(true)
const error = ref('')
const markdown = new MarkdownIt({ linkify: true, breaks: true })

const renderedReport = computed(() => {
  if (!result.value?.reportText) {
    return '<p>No report available.</p>'
  }
  return markdown.render(result.value.reportText)
})

const fetchResult = async () => {
  loading.value = true
  error.value = ''
  const api = await client.api(new GetInterview({ id }))
  if (api.succeeded) {
    if (api.response?.result) {
      result.value = api.response.result
    } else {
      error.value = 'No result found for this interview.'
    }
  } else {
    error.value = api.error?.message || 'Failed to load interview.'
  }
  loading.value = false
}

function getScoreClass(score: number) {
  if (score >= 80) return 'border-green-500 text-green-600'
  if (score >= 60) return 'border-yellow-500 text-yellow-600'
  return 'border-red-500 text-red-600'
}

onMounted(() => {
  fetchResult()
})
</script>
