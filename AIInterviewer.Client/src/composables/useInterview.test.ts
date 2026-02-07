import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useInterview } from './useInterview'
import { client } from '@/lib/gateway'
import { GetInterview, StartInterview } from '@/lib/dtos'

// Mock client
vi.mock('@/lib/gateway', () => ({
    client: {
        api: vi.fn()
    }
}))

// Mock vue-router
const pushMock = vi.fn()
vi.mock('vue-router', () => ({
    useRouter: () => ({
        push: pushMock
    })
}))

describe('useInterview', () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it('initializes with empty history', () => {
        const { history } = useInterview(123)
        expect(history.value).toEqual([])
    })

    it('loadInterview fetches interview and updates history', async () => {
        const mockResponse = {
            history: [{ id: 1, role: 'User', content: 'Hello' }],
            result: null
        }

        ;(client.api as any).mockResolvedValue({
            succeeded: true,
            response: mockResponse
        })

        const { history, loadInterview } = useInterview(123)
        await loadInterview()

        expect(client.api).toHaveBeenCalledWith(expect.any(GetInterview))
        expect(history.value).toEqual(mockResponse.history)
    })

    it('startInterview calls API and updates history', async () => {
        const mockResponse = {
            history: [{ id: 1, role: 'Interviewer', content: 'Hi' }],
        }

        ;(client.api as any).mockResolvedValue({
            succeeded: true,
            response: mockResponse
        })

        const { history, startInterview, processingAi, skipVoicePlayback } = useInterview(123)
        skipVoicePlayback.value = true

        // startInterview(shouldAutoStart = true)
        const promise = startInterview(true)

        expect(processingAi.value).toBe(true)

        await promise

        expect(client.api).toHaveBeenCalledWith(expect.any(StartInterview))
        expect(history.value).toEqual(mockResponse.history)
        expect(processingAi.value).toBe(false)
    })
})
