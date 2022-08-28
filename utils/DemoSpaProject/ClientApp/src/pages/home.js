import React, { useState } from 'react'
import { useSelector } from 'react-redux'
import { signinRedirect } from '../services/userService'
import * as apiService from '../services/apiService'

function Home() {
  const user = useSelector(state => state.auth.user)
  const [identityData, setIdentityData] = useState(null)

  async function getIdentityData() {
    const identity = await apiService.getIdentityData()
    setIdentityData(identity)
  }

  if (user) {
    const loadCaption = identityData ? 'Reload data' : 'Load data'
    return (
      <div className="m-3">
        <button type="button" className="btn btn-primary" onClick={() => getIdentityData()}>
          {loadCaption}
        </button>

        {
          identityData &&
            <div className="mt-3">
              <h4>Identity data:</h4>
              <pre>
                <code>
                  {identityData ? JSON.stringify(identityData, null, 2) : 'No data yet :('}
                </code>
              </pre>
            </div>
        }

      </div>
    );
  }

  return (
    <div>
      <button type="button" className="btn btn-primary m-3" onClick={() => signinRedirect()}>
        Login
      </button>
    </div>
  )
}

export default Home
