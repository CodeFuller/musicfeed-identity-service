import React from 'react'
import { useSelector } from 'react-redux'
import { signinRedirect } from '../services/userService'

function Home() {
  const user = useSelector(state => state.auth.user)

  if (user) {
    return (
      <div>
        <h1>You are logged in :)</h1>
      </div>
    );
  }

  return (
    <div>
      <h1>You are not logged in :(</h1>
      <button onClick={() => signinRedirect()}>
        Login
      </button>
    </div>
  )
}

export default Home
