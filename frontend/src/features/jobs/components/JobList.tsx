import React from 'react'
import type { JobSummary } from '../../../types/job'
import JobCard from './JobCard'
import EmptyState from '../../../components/EmptyState'

export default function JobList({items}:{items:JobSummary[]}){
  if (!items || items.length === 0) return <EmptyState title="No jobs found" />
  return <div>{items.map(j=> <JobCard key={j.id} job={j} />)}</div>
}
