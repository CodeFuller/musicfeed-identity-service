import axios from 'axios'
import jwt_decode from 'jwt-decode'
import { refreshToken } from './userService'

const ApiService = {
  axios: axios.create(),
  jwtExpiration: null,

  setJwt(jwt) {
    if (jwt) {
      const tokenData = jwt_decode(jwt)
      this.jwtExpiration = new Date(1000 * tokenData.exp)
      console.debug(`Token will expire at: ${this.jwtExpiration.toLocaleString()}`)
    } else {
      console.debug('Cleared JWT')
    }

    this.axios.defaults.headers.common['Authorization'] = `Bearer ${jwt}`
  },

  async getIdentityData() {
    await this.refreshTokenIfNecessary()

    const response = await this.axios.get('https://localhost:6001/identity');
    return response.data;
  },

  async refreshTokenIfNecessary() {
    if (Date.now() >= this.jwtExpiration) {
      console.info('JWT has expired, refreshing ...')
      await refreshToken()
    }
  }
}

export default ApiService
