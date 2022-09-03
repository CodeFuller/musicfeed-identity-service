import React, { useState } from 'react'
import ApiService from '../services/apiService'

export function IdentityPage() {
  const [identityData, setIdentityData] = useState(null)

  async function getIdentityData() {
    const identity = await ApiService.getIdentityData()
    setIdentityData(identity)
  }

  function getDateTimeValue(claimType, claimValue) {
    return claimType === 'nbf' || claimType === 'iat' || claimType === 'exp' || claimType === 'auth_time' ?
      getDateTime(claimValue) : '';
  }

  function getDateTime(unixTime) {
    return new Date(1000 * unixTime).toLocaleString()
  }
  
  const loadCaption = identityData ? 'Reload data' : 'Load data'
  return (
    <div className="m-3">
      <button type="button" className="btn btn-primary" onClick={() => getIdentityData()}>
        {loadCaption}
      </button>
      {
        identityData &&
        <div className="mt-3">
          <h4>Token:</h4>
          <code>{identityData.token}</code>
          <h4 className='mt-3'>Claims:</h4>
          <table className="table table-striped table-bordered">
            <thead>
              <tr>
                <th>Type</th>
                <th>Value</th>
                <th>Date &amp; Time</th>
              </tr>
            </thead>
            <tbody>
              {identityData.claims.map(claim =>
              (
                <tr>
                  <td>{claim.type}</td>
                  <td>{claim.value}</td>
                  <td>{getDateTimeValue(claim.type, claim.value)}</td>
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
