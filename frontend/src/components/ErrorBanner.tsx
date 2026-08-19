import React from 'react'

export default function ErrorBanner({message, onRetry}:{message:string; onRetry?:()=>void}){
  return (
    <div role="alert" style={{border:'1px solid #fca5a5',padding:12,borderRadius:6,background:'#fff1f2'}}>
      <div style={{color:'#991b1b'}}>{message}</div>
      {onRetry && <button className="button" onClick={onRetry} style={{marginTop:8}}>Retry</button>}
    </div>
  )
}
