import { useEffect, useState } from 'react'
import { apiUrl, authClient } from './api'
import type { UserModel } from './generated-client'
import UserRolesPage from './UserRolesPage'
import './App.css'

const userRolesPath = '/user-roles'

function App() {
  const [user, setUser] = useState<UserModel | null>(null)
  const [error, setError] = useState(false)

  useEffect(() => {
    let active = true

    authClient.getUserInfo()
      .then(result => {
        if (active) setUser(result)
      })
      .catch(() => {
        if (active) setError(true)
      })

    return () => {
      active = false
    }
  }, [])

  if (error) return <p>Could not check login status.</p>
  if (!user) return <p>Checking login…</p>

  const canManageRoles = user.isAuthenticated && user.additionalClaims?.app_roles === 'PM'

  if (window.location.pathname === userRolesPath) {
    return canManageRoles ? <UserRolesPage /> : (
      <main>
        <p>Only project managers can manage user roles.</p>
        <a href="/">Back to home</a>
      </main>
    )
  }

  return (
    <div className="card">
      {user.isAuthenticated ? (
        <>
          <h1>Welcome, {user.name || user.emailAddress || 'user'}</h1>
          <p>You are logged in.</p>
          {canManageRoles && <a className="user-roles-link" href={userRolesPath}>Manage user roles</a>}
        </>
      ) : (
        <button
          onClick={() => {
            window.location.href = `${apiUrl}/api/auth/Login`
          }}
        >
          Login
        </button>
      )}
    </div>
  )
}

export default App
