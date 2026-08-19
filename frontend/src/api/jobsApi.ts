import { apiClient, parseApiError } from './client'
import type { PagedResult, JobSummary, JobDetail, ApiError } from '../types/job'

export async function getJobs(params: { search?: string; page?: number; pageSize?: number }): Promise<PagedResult<JobSummary>> {
  const requestParams: Record<string, unknown> = {}
  if (params.search) requestParams.search = params.search
  requestParams.page = params.page ?? 1
  requestParams.pageSize = params.pageSize ?? 10

  try {
    const resp = await apiClient.get<PagedResult<JobSummary>>('/api/v1/jobs', { params: requestParams })
    return resp.data
  } catch (e) {
    throw parseApiError(e) as ApiError
  }
}

export async function getJobById(id: number): Promise<JobDetail> {
  try {
    const resp = await apiClient.get<JobDetail>(`/api/v1/jobs/${id}`)
    return resp.data
  } catch (e) {
    throw parseApiError(e) as ApiError
  }
}
