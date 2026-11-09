import { useEffect, useState } from 'react'
import { AuthClient } from './generated-client'
import type { UserModel } from './generated-client'
import './App.css'
import {useAtom} from "jotai";
import {userAtom} from "./atoms.ts";
import Findingform from "./FindingForm.tsx";

const apiUrl = 'http://localhost:5001'

const authClient = new AuthClient(apiUrl, {
  fetch: (url, options) =>
    window.fetch(url, { ...options, credentials: 'include' }),
})

function App() {
  const [user, setUser] = useState<UserModel | null>(null)
  const [, setUserA] = useAtom(userAtom)
  const [error, setError] = useState(false)

  useEffect(() => {
    let active = true

    authClient.getUserInfo()
      .then(result => {
        if (active) {
          setUser(result);
          setUserA(result);
          }
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
          <Findingform></Findingform>
        </>
      ) : (
          <div>
            <button
                onClick={() => {
                  window.location.href = `${apiUrl}/api/auth/Login`
                }}
            >
              Login
            </button>
            <div className={"external-login-div"}>
              <p>not a member of this organization? submit form here</p>
              <button onClick={() => {
                window.location.href = window.location.href + `/FindingForm`
              }}>To external form</button>
            </div>
            
          </div>
      )}
    </div>
  )
}

export default App