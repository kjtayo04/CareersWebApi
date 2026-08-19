import React from 'react'

export default function EmptyState({title, children}:{title:string; children?:React.ReactNode}){
  return (
    <div style={{padding:24,textAlign:'center'}}>
      <h3>{title}</h3>
      {children}
    </div>
  )
}
