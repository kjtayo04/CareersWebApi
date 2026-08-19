import React, { useState, useEffect } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import Spinner from '../../../components/Spinner'
import ErrorBanner from '../../../components/ErrorBanner'
import SearchBar from '../components/SearchBar'
import PaginationControls from '../components/PaginationControls'
import PageSizeSelector from '../components/PageSizeSelector'
import JobList from '../components/JobList'
import useDebouncedValue from '../hooks/useDebouncedValue'
import useJobsQuery from '../hooks/useJobsQuery'
import type { JobSummary } from '../../../types/job'

export default function JobsPage(): JSX.Element{
  const [searchParams, setSearchParams] = useSearchParams()
  const [rawSearch, setRawSearch] = useState(searchParams.get('search') ?? '')
  const debouncedSearch = useDebouncedValue(rawSearch, 400)
  const [page, setPage] = useState(Number(searchParams.get('page') ?? 1))
  const [pageSize, setPageSize] = useState(Number(searchParams.get('pageSize') ?? 10))

  // keep URL in sync
  useEffect(()=>{
    const params:any = {}
    if (rawSearch) params.search = rawSearch
    if (page) params.page = String(page)
    if (pageSize) params.pageSize = String(pageSize)
    setSearchParams(params)
  },[rawSearch,page,pageSize,setSearchParams])

  // Reset page when debounced search or pageSize changes
  useEffect(()=>{
    setPage(1)
  },[debouncedSearch,pageSize])

  const { data, isLoading, isError, error, refetch } = useJobsQuery({search: debouncedSearch, page, pageSize})

  if (isLoading) return <Spinner />
  if (isError) return <ErrorBanner message={(error as any)?.title ?? 'Error loading jobs'} onRetry={()=>refetch()} />

  return (
    <div>
      <div style={{marginBottom:12}}>
        <SearchBar value={rawSearch} onChange={v=>setRawSearch(v)} />
        <PageSizeSelector value={pageSize} onChange={n=>{setPageSize(n); setPage(1)}} />
      </div>

      <JobList items={data?.items ?? []} />

      {data && (
        <PaginationControls
          page={data.page}
          totalPages={data.totalPages}
          hasPreviousPage={data.hasPreviousPage}
          hasNextPage={data.hasNextPage}
          onPageChange={p=>setPage(p)}
        />
      )}
    </div>
  )
}
