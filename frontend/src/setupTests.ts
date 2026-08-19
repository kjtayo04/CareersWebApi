import '@testing-library/jest-dom'

// msw server import for tests
if (import.meta.vitest) {
  const { server } = await import('./mocks/server')
  server.listen({ onUnhandledRequest: 'warn' })
  // teardown
  import.meta.vitest.afterAll(() => server.close())
  import.meta.vitest.afterEach(() => server.resetHandlers())
}
