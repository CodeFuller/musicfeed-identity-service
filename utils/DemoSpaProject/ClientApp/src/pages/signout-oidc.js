import React, { useEffect } from 'react'
import { signoutRedirectCallback } from '../services/userService'
import { useHistory } from 'react-router-dom'

export function SignoutOidc() {
  const history = useHistory()
  useEffect(() => {
    async function signoutAsync() {
      await signoutRedirectCallback()
      history.push('/')
    }
    signoutAsync()
  }, [history])

  return (
    <div>
      Redirecting...
    </div>
  )
}
