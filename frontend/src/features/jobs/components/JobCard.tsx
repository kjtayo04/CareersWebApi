import React from 'react'
import { Link } from 'react-router-dom'
import type { JobSummary } from '../../../types/job'

export default function JobCard({job}:{job:JobSummary}){
  return (
    <div className="card">
      <h3><Link to={`/jobs/${job.id}`}>{job.title}</Link></h3>
      <div style={{color:'#64748b'}}>{job.department} — {job.location}</div>
      <div style={{marginTop:8,fontSize:12,color:'#94a3b8'}}>{new Date(job.publishedAt).toLocaleDateString()}</div>
    </div>
  )
}
