import React from 'react'
import { Routes, Route, Link } from 'react-router-dom'
import JobsPage from './features/jobs/pages/JobsPage'
import JobDetailPage from './features/jobs/pages/JobDetailPage'

export default function App(): JSX.Element {
  return (
    <div className="container">
      <header>
        <h1><Link to="/">Careers</Link></h1>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<JobsPage />} />
          <Route path="/jobs/:id" element={<JobDetailPage />} />
        </Routes>
      </main>
    </div>
  )
}
