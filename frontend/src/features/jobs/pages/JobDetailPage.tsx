import React from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getJobById } from '../../../api/jobsApi'
import Spinner from '../../../components/Spinner'
import ErrorBanner from '../../../components/ErrorBanner'
import DOMPurify from 'dompurify'

export default function JobDetailPage(): JSX.Element{
  const { id } = useParams()
  const navigate = useNavigate()
  const location = useLocation()

  const numId = Number(id)
  const { data, isLoading, isError, error, refetch } = useQuery(['job', numId], ()=>getJobById(numId), {enabled: Boolean(numId)})

  if (isLoading) return <Spinner />
  if (isError) return <ErrorBanner message={(error as any)?.title ?? 'Error loading job'} onRetry={()=>refetch()} />

  if (!data) return <div>Not found</div>

  return (
    <div>
      <button className="button" onClick={()=>navigate(-1)}>Back</button>
      <h2>{data.title}</h2>
      <div style={{color:'#64748b'}}>{data.department} — {data.location}</div>
      <div style={{marginTop:12}} dangerouslySetInnerHTML={{__html: DOMPurify.sanitize(data.content)}} />
      <div style={{marginTop:12}}><a href={data.absoluteUrl} target="_blank" rel="noreferrer">Apply on external site</a></div>
    </div>
  )
}
