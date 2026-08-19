import React from 'react'
import { render } from '@testing-library/react'
import useDebouncedValue from '../features/jobs/hooks/useDebouncedValue'

describe('useDebouncedValue', ()=>{
  it('debounces value changes', async ()=>{
    vi.useFakeTimers()

    function Test({value}:{value:string}){
      const v = useDebouncedValue(value, 200)
      return <div data-testid="out">{v}</div>
    }

    const { getByTestId, rerender } = render(<Test value="" />)
    expect(getByTestId('out').textContent).toBe('')

    rerender(<Test value="a" />)
    // not updated immediately
    expect(getByTestId('out').textContent).toBe('')

    vi.advanceTimersByTime(200)
    // after timers flush
    expect(getByTestId('out').textContent).toBe('a')

    vi.useRealTimers()
  })
})
