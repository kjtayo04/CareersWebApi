import React from 'react'
import { render, screen } from '@testing-library/react'
import JobList from '../features/jobs/components/JobList'

const items = [{ id:1, title:'Title', location:'Remote', department:'Eng', publishedAt:new Date().toISOString(), absoluteUrl:'https://example.com' }]

describe('JobList', ()=>{
  it('renders items', ()=>{
    render(<JobList items={items} />)
    expect(screen.getByText('Title')).toBeInTheDocument()
  })
  it('renders empty state when no items', ()=>{
    render(<JobList items={[]} />)
    expect(screen.getByText('No jobs found')).toBeInTheDocument()
  })
})
