import { signinRedirect } from '../services/userService'

export function LoginPage() {
  return (
    <div>
      <button type="button" className="btn btn-primary m-3" onClick={() => signinRedirect()}>
        Login
      </button>
    </div>
  )
}
