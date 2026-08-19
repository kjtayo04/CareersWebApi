import React from 'react'
import { render, screen, fireEvent } from '@testing-library/react'
import PaginationControls from '../features/jobs/components/PaginationControls'

describe('PaginationControls', ()=>{
  it('renders and disables when at boundaries', ()=>{
    const onPageChange = vi.fn()
    render(<PaginationControls page={1} totalPages={3} hasPreviousPage={false} hasNextPage={true} onPageChange={onPageChange} />)
    expect(screen.getByLabelText('previous-page')).toBeDisabled()
    expect(screen.getByLabelText('next-page')).not.toBeDisabled()
    fireEvent.click(screen.getByLabelText('next-page'))
    expect(onPageChange).toHaveBeenCalled()
  })
})
