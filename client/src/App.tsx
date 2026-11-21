import { useEffect, useState } from 'react'
import { apiUrl, authClient } from './api'
import type { UserModel } from './generated-client'
import UserRolesPage from './UserRolesPage'
import './App.css'
import {useAtom} from "jotai";
import {userAtom} from "./atoms.ts";
import Findingform from "./FindingForm.tsx";
import { useNavigate } from 'react-router-dom';

const userRolesPath = '/user-roles'

function App() {
  const [user, setUser] = useState<UserModel | null>(null)
  const [, setUserA] = useAtom(userAtom)
  const [error, setError] = useState(false)
  const navigate = useNavigate();
  

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
          <Findingform></Findingform>
          {canManageRoles && <a className="user-roles-link" href={userRolesPath}>Manage user roles</a>}
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
              <button onClick={() => {navigate('/FindingForm')}}>To external form</button>
            </div>
            
          </div>
      )}
    </div>
  )
}

export default App
