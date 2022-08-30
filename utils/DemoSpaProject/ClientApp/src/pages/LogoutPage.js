import { signoutRedirect } from '../services/userService'

export function LogoutPage() {
  return (
    <div>
      Do you really want to logout?
      <button type="button" className="btn btn-primary m-3" onClick={() => signoutRedirect()}>
        Logout
      </button>
    </div>
  )
}
