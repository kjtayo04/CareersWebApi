import { useQuery } from '@tanstack/react-query'
import { getJobs } from '../../../api/jobsApi'
import type { PagedResult, JobSummary } from '../../../types/job'

type Params = { search?: string; page?: number; pageSize?: number }

export default function useJobsQuery(params: Params) {
  const key = ['jobs', params]
  return useQuery<PagedResult<JobSummary>, unknown>(key, () => getJobs(params), { keepPreviousData: true })
}
