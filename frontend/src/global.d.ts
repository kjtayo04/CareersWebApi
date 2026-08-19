/* Auto-generated shims to satisfy TypeScript checks during solution build */
declare module 'vite' {
  export function defineConfig(config: any): any
  export function loadEnv(mode: string, cwd: string, prefix?: string): Record<string, string>
}

declare module 'vitest/config' {
  export function defineConfig(config: any): any
}

declare module '@vitejs/plugin-react' {
  const plugin: any
  export default plugin
}

declare module 'msw' {
  export const rest: any
}

declare module '*.css'

declare global {
  var process: any
  interface ImportMetaEnv {
    VITE_API_BASE_URL?: string
    DEV?: boolean
  }
  interface ImportMeta {
    readonly env: ImportMetaEnv
    readonly vitest?: any
  }
}

export {}
