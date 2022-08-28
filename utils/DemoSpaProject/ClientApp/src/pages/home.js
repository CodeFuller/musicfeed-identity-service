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
              <h4>Claims:</h4>
              <table className="table table-striped table-bordered">
                <thead>
                  <tr>
                    <th>Type</th>
                    <th>Value</th>
                  </tr>
                </thead>
                <tbody>
                  {identityData.claims.map(claim => 
                    (
                      <tr>
                        <td>{claim.type}</td>
                        <td>{claim.value}</td>
                      </tr>
                    ))
                  }
                </tbody>
              </table>
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
