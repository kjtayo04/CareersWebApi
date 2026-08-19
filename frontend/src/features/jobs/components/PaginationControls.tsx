import React from 'react'

export default function PaginationControls({
  page,
  totalPages,
  hasPreviousPage,
  hasNextPage,
  onPageChange
}:{
  page:number
  totalPages:number
  hasPreviousPage:boolean
  hasNextPage:boolean
  onPageChange:(p:number)=>void
}){
  return (
    <div className="pagination" style={{marginTop:12}}>
      <button aria-label="previous-page" className="button" onClick={()=>onPageChange(Math.max(1,page-1))} disabled={!hasPreviousPage}>Previous</button>
      <div>Page {page} of {totalPages}</div>
      <button aria-label="next-page" className="button" onClick={()=>onPageChange(Math.min(totalPages,page+1))} disabled={!hasNextPage}>Next</button>
    </div>
  )
}
