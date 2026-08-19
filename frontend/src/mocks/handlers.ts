import { rest } from 'msw'
import type { PagedResult, JobSummary } from '../types/job'

const makeJobs = (count:number): JobSummary[] => Array.from({length:count}).map((_,i)=>({
  id: i+1,
  title: `Job ${i+1}`,
  location: 'Remote',
  department: 'Engineering',
  publishedAt: new Date().toISOString(),
  absoluteUrl: `https://example.com/jobs/${i+1}`
}))

export const handlers = [
  // match any origin for tests/dev
  rest.get(/.*\/api\/v1\/jobs$/, (req, res, ctx) => {
    const search = req.url.searchParams.get('search') ?? ''
    const page = Number(req.url.searchParams.get('page') ?? '1')
    const pageSize = Number(req.url.searchParams.get('pageSize') ?? '10')

    const all = makeJobs(42).filter(j=> j.title.toLowerCase().includes(search.toLowerCase()))
    const totalCount = all.length
    const totalPages = Math.max(1, Math.ceil(totalCount / pageSize))
    const items = all.slice((page-1)*pageSize, page*pageSize)

    const body: PagedResult<JobSummary> = {
      items,
      page,
      pageSize,
      totalCount,
      totalPages,
      hasPreviousPage: page > 1,
      hasNextPage: page < totalPages
    }

    return res(ctx.status(200), ctx.json(body))
  }),

  rest.get(/.*\/api\/v1\/jobs\/[0-9]+$/, (req,res,ctx)=>{
    const id = Number(req.params.id)
    return res(ctx.status(200), ctx.json({
      id,
      title: `Job ${id}`,
      location: 'Remote',
      department: 'Engineering',
      publishedAt: new Date().toISOString(),
      absoluteUrl: `https://example.com/jobs/${id}`,
      content: `<p>Description for job ${id}</p>`
    }))
  })
]
