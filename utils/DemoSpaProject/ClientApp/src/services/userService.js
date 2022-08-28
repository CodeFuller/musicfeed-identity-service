import { UserManager } from 'oidc-client';
import { storeUserError, storeUser } from '../actions/authActions'

const config = {
  authority: "https://localhost:5001",
  client_id: "musicfeed-api",
  redirect_uri: "https://localhost:44456/signin-oidc",
  response_type: "code",
  scope: "openid profile offline_access musicfeed-api",
  post_logout_redirect_uri: "https://localhost:44456/signout-oidc",
};

const userManager = new UserManager(config)

export async function loadUserFromStorage(store) {
  try {
    var user = await userManager.getUser()
    if (!user) {
      return store.dispatch(storeUserError())
    }

    store.dispatch(storeUser(user))
  } catch (e) {
    console.error(`User not found: ${e}`)
    store.dispatch(storeUserError())
  }
}

export function signinRedirect() {
  return userManager.signinRedirect()
}

export function signinRedirectCallback() {
  return userManager.signinRedirectCallback()
}

export function signoutRedirect() {
  userManager.clearStaleState()
  userManager.removeUser()
  return userManager.signoutRedirect()
}

export function signoutRedirectCallback() {
  userManager.clearStaleState()
  userManager.removeUser()
  return userManager.signoutRedirectCallback()
}

export async function refreshToken() {
  console.debug("Calling signinSilent() ...")
  await userManager.signinSilent()
  console.debug("signinSilent() completed")
}

export default userManager
