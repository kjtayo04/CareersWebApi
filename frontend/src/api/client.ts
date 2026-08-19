import axios from 'axios'

const envBase = (import.meta.env.VITE_API_BASE_URL ?? '').toString()

// In dev prefer Vite proxy (so requests go to Vite which proxies to backend) to avoid CORS/HTTPS cert issues.
const useProxyInDev = import.meta.env.DEV === true
const resolvedBase = useProxyInDev ? '' : envBase

if (!envBase) {
  // Helpful developer message when env var is missing
  // In dev this often means requests go to the Vite server origin which may not proxy to the API.
  // Ensure you set VITE_API_BASE_URL in .env (e.g. VITE_API_BASE_URL=http://localhost:5000)
  // See README.md and .env.example.
  // eslint-disable-next-line no-console
  console.warn('[api] VITE_API_BASE_URL is not set — requests will use the current origin')
}
const baseURL = resolvedBase

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 10000
})
// Log requests/responses to help diagnose 404s in dev
apiClient.interceptors.request.use((config) => {
  // eslint-disable-next-line no-console
  console.debug('[api] request', { baseURL: apiClient.defaults.baseURL, url: config.url, params: config.params })
  return config
})

apiClient.interceptors.response.use(
  (resp) => {
    // eslint-disable-next-line no-console
    console.debug('[api] response', { status: resp.status, url: resp.config.url })
    return resp
  },
  (err) => {
    // eslint-disable-next-line no-console
    console.error('[api] response error', {
      message: err?.message,
      url: err?.config?.url,
      status: err?.response?.status,
      data: err?.response?.data,
    })
    return Promise.reject(err)
  }
)

export function parseApiError(e: unknown) {
  if (axios.isAxiosError(e)) {
    return {
      title: e.response?.data?.title ?? e.message ?? 'Axios error',
      status: e.response?.status ?? 0,
      detail: e.response?.data?.detail ?? (typeof e.response?.data === 'string' ? e.response?.data : undefined),
      url: e.config?.url,
    } as any
  }
  return { title: 'Unknown error', status: 0 }
}

export default apiClient
