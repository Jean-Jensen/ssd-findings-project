import { useEffect, useState } from 'react'
import { AuthClient } from './generated-client'
import type { UserModel } from './generated-client'
import './App.css'

const apiUrl = 'http://localhost:5001'

const authClient = new AuthClient(apiUrl, {
  fetch: (url, options) =>
    window.fetch(url, { ...options, credentials: 'include' }),
})

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

  return (
    <div className="card">
      {user.isAuthenticated ? (
        <>
          <h1>Welcome, {user.name || user.emailAddress || 'user'}</h1>
          <p>You are logged in.</p>
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