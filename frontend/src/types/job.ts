export type JobSummary = {
  id: number
  title: string
  location: string
  department: string
  publishedAt: string // ISO
  absoluteUrl: string
}

export type JobDetail = JobSummary & {
  content: string
}

export type PagedResult<T> = {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export type ApiError = {
  title: string
  status: number
  detail?: string
}
