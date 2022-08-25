import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { claims: [], loading: true };
  }

  componentDidMount() {
    this.populateClaimsData();
  }

  static renderClaimsTable(claims) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Claim Type</th>
            <th>Claim Value</th>
          </tr>
        </thead>
        <tbody>
          {claims.map(claim =>
            <tr key={claim.type}>
              <td>{claim.type}</td>
              <td>{claim.value}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderClaimsTable(this.state.claims);

    return (
      <div>
        <h1 id="tabelLabel">Claims</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async populateClaimsData() {
    const response = await fetch('https://localhost:6001/identity');
    const data = await response.json();
    this.setState({ claims: data.claims, loading: false });
  }
}
