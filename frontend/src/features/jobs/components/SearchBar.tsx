import React from 'react'

export default function SearchBar({value,onChange}:{value:string;onChange:(v:string)=>void}){
  return (
    <input
      aria-label="search"
      className="input"
      value={value}
      onChange={e=>onChange(e.target.value)}
      placeholder="Search jobs"
    />
  )
}
