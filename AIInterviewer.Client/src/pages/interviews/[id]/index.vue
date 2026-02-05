<template>
    <div class="container mx-auto p-4">
        <h1 class="text-2xl font-bold mb-4">Interview #{{ id }}</h1>
        
        <div class="bg-gray-100 dark:bg-gray-900 p-4 rounded h-96 overflow-y-auto mb-4">
            <div v-for="msg in history" :key="msg.id" class="mb-2">
                <strong :class="msg.role === 'User' ? 'text-blue-600' : 'text-green-600'">{{ msg.role }}:</strong> {{ msg.content }}
            </div>
            <div v-if="history.length === 0" class="text-gray-500 text-center mt-10">No messages yet. Start talking!</div>
        </div>

        <div class="flex gap-4">
           <input type="text" placeholder="Type a message..." class="border p-2 flex-grow rounded" disabled title="Chat UI handled by other task"/>
           
           <button @click="endInterview" :disabled="processing" class="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700 transition">
               {{ processing ? 'Ending...' : 'End Interview' }}
           </button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { client } from '@/lib/gateway'
import { GetInterview, FinishInterview, type InterviewChatHistoryDto } from '@/lib/dtos'

const route = useRoute()
const router = useRouter()
const id = parseInt(route.params.id as string)
const history = ref<InterviewChatHistoryDto[]>([])
const processing = ref(false)

const loadInterview = async () => {
    const api = await client.api(new GetInterview({ id }))
    if (api.succeeded) {
        history.value = api.response.history
        if (api.response.result) {
            router.push(`/interviews/${id}/result`)
        }
    }
}

const endInterview = async () => {
    if (!confirm('Are you sure you want to end the interview? This will generate your feedback report.')) return;
    
    processing.value = true
    try {
        const api = await client.api(new FinishInterview({ id }))
        if (api.succeeded) {
            router.push(`/interviews/${id}/result`)
        } else {
            alert('Failed to end interview: ' + api.error?.message)
        }
    } catch (e: any) {
        alert('Error: ' + e.message)
    } finally {
        processing.value = false
    }
}

onMounted(() => {
    loadInterview()
})
</script>
