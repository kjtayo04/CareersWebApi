import React from 'react'

export default function PageSizeSelector({value,onChange}:{value:number;onChange:(n:number)=>void}){
  return (
    <select aria-label="page-size" value={value} onChange={e=>onChange(Number(e.target.value))} style={{marginLeft:8}}>
      {[5,6,7,8,9,10].map(n=> <option key={n} value={n}>{n} per page</option>)}
    </select>
  )
}
