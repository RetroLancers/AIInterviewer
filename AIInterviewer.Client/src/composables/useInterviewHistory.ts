import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { client } from '@/lib/gateway'
import { GetInterviewHistory, type InterviewDto } from '@/lib/dtos'

export const formatDate = (value: string | Date) => {
  return new Date(value).toLocaleString()
}

export const promptPreview = (prompt: string) => {
  const trimmed = prompt.trim()
  if (trimmed.length <= 240) return trimmed
  return `${trimmed.slice(0, 240)}...`
}

export function useInterviewHistory() {
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

  const startFromPrompt = (prompt: string) => {
    router.push({ path: '/interviews/new', query: { prompt } })
  }

  return {
    interviews,
    loading,
    error,
    fetchHistory,
    formatDate,
    promptPreview,
    startFromPrompt
  }
}
