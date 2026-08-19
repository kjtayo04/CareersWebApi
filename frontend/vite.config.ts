import { defineConfig, loadEnv } from 'vite'
import * as net from 'net'

async function canConnect(target: string, timeout = 1000): Promise<boolean> {
  try {
    const u = new URL(target)
    const host = u.hostname
    const port = Number(u.port) || (u.protocol === 'https:' ? 443 : 80)

    return await new Promise((resolve) => {
      const socket = net.connect({ host, port })
      let done = false
      const onSuccess = () => {
        if (done) return
        done = true
        socket.destroy()
        resolve(true)
      }
      const onFail = () => {
        if (done) return
        done = true
        try { socket.destroy() } catch {}
        resolve(false)
      }
      socket.once('connect', onSuccess)
      socket.once('error', onFail)
      socket.setTimeout(timeout, onFail)
    })
  } catch {
    return false
  }
}

export default defineConfig(async ({ mode }) => {
  // Load Vite env variables for the current mode so VITE_API_BASE_URL is available
  const env = loadEnv(mode, process.cwd(), '')
  // Default to your backend dev origin if env not set — this makes the proxy active even when .env is missing.
  let apiTarget = env.VITE_API_BASE_URL || 'https://localhost:7284'

  // If configured target is https but not reachable, try http fallback on same host:port
  if (apiTarget.startsWith('https://')) {
    // quick TCP probe
    const ok = await canConnect(apiTarget)
    if (!ok) {
      // try http fallback on same host/port
      try {
        const u = new URL(apiTarget)
        const fallback = `http://${u.hostname}${u.port ? ':' + u.port : ''}`
        const ok2 = await canConnect(fallback)
        if (ok2) {
          // eslint-disable-next-line no-console
          console.warn('[vite] HTTPS target not reachable, falling back to', fallback)
          apiTarget = fallback
        }
      } catch {}
    }
  }

  // dynamically import ESM-only plugin to avoid require() errors in some environments
  const reactPlugin = (await import('@vitejs/plugin-react')).default

  const server: any = { port: 5173 }
  if (apiTarget) {
    // Proxy /api requests to backend when VITE_API_BASE_URL is set — helpful in dev to avoid CORS or incorrect baseURL
    // Rewrite is used to normalize path casing (e.g. /api/v1/jobs -> /api/v1/Jobs) to avoid backend routing case issues
    server.proxy = {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
        rewrite: (path: string) => {
          // normalize common path casing: ensure controller segment 'Jobs' has capital J
          return path.replace(/^\/api\/v1\/jobs/i, '/api/v1/Jobs')
        },
      },
    }
    // Log proxy configuration at startup to help debugging
    // eslint-disable-next-line no-console
    console.log('[vite] proxy /api ->', apiTarget)
  }

  return {
    plugins: [reactPlugin()],
    server,
  }
})
