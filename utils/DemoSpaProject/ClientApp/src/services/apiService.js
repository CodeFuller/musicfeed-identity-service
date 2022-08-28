import axios from 'axios'

async function getIdentityData() {
  const response = await axios.get('https://localhost:6001/identity');
  return response.data;
}

export {
  getIdentityData
}
