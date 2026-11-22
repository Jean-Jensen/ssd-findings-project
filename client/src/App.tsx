
import { useEffect, useState } from 'react'
import { apiUrl, authClient } from './api'
import type { UserModel } from './generated-client'
import UserRolesPage from './UserRolesPage'
import AuditLogPage from './AuditLogPage'
import './App.css'
import {useAtom} from "jotai";
import {userAtom} from "./atoms.ts";
import Findingform from "./FindingForm.tsx";
import { useNavigate } from 'react-router-dom';
import FindingTablePage from "./FindingTablePage.tsx";
import DashboardPage from './DashboardPage'

const userRolesPath = '/user-roles'
const auditLogPath = '/audit-log'
const dashboardPath = '/dashboard'

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

    if (error) {
        return <p>Could not check login status.</p>
    }

    if (!user) {
        return <p>Checking login...</p>
    }

    const canManageRoles =
        user.isAuthenticated &&
        user.additionalClaims?.app_roles === 'PM'

    const canViewAuditLog =
        user.isAuthenticated &&
        (user.additionalClaims?.app_roles === 'PM' ||
            user.additionalClaims?.app_roles === 'Auditor')

    const canViewDashboard =
        user.isAuthenticated &&
        ['Analyst', 'Developer', 'PM', 'Auditor'].includes(
            user.additionalClaims?.app_roles ?? ''
        )

    if (window.location.pathname === userRolesPath) {
        return canManageRoles
            ? <UserRolesPage />
            : (
                <main>
                    <p>Only project managers can manage user roles.</p>
                    <a href="/">Back to home</a>
                </main>
            )
    }

    if (window.location.pathname === auditLogPath) {
        return canViewAuditLog
            ? <AuditLogPage />
            : (
                <main>
                    <p>You do not have permission to view the audit log.</p>
                    <a href="/">Back to home</a>
                </main>
            )
    }
    if (window.location.pathname === dashboardPath) {
        return canViewDashboard
            ? <DashboardPage />
            : (
                <main>
                    <p>You do not have permission to view the dashboard.</p>
                    <a href="/">Back to home</a>
                </main>
            )
    }

    return (
        <main className="app-home">
            {user.isAuthenticated ? (
                <>
                    <header className="app-home-header">
                        <h1>
                            Welcome, {user.name || user.emailAddress || 'user'}
                        </h1>
                        <p>
                            You are logged in.
                        </p>
                    </header>

                    <section className="app-home-section">
                        <h2>Application</h2>

                        <nav className="app-navigation">

                            {canViewDashboard && (
                                <a
                                    href={dashboardPath}
                                    className="app-navigation-card"
                                >
                                    <div>
                                        <h3>Dashboard</h3>
                                        <p>
                                            View finding statistics, severity distribution
                                            and current status.
                                        </p>
                                    </div>

                                    <span className="app-card-link">
            View dashboard →
        </span>
                                </a>
                            )}
                            {canViewAuditLog && (
                                <a
                                    href={auditLogPath}
                                    className="app-navigation-card"
                                >
                                    <div>
                                        <h3>Audit Log</h3>
                                        <p>
                                            Review recorded system activities,
                                            changes and user actions.
                                        </p>
                                    </div>

                                    <span className="app-card-link">
                                        View audit log →
                                    </span>
                                </a>
                            )}

                            {canManageRoles && (
                                <a
                                    href={userRolesPath}
                                    className="app-navigation-card"
                                >
                                    <div>
                                        <h3>Manage User Roles</h3>
                                        <p>
                                            View users and manage their
                                            application roles and permissions.
                                        </p>
                                    </div>

                                    <span className="app-card-link">
                                        Manage users →
                                    </span>
                                </a>
                            )}

                        </nav>
                    </section>


                    <footer className="app-home-footer">
                        Application dashboard
                    </footer>
                </>
            ) : (
                <div className="card">
                    <h1>Welcome</h1>
                    <p>Please log in to continue.</p>
                    <button
                        onClick={() => {
                            window.location.href = `${apiUrl}/api/auth/Login`
                        }}
                    >
                        Login
                    </button>
                </div>
            )}
        </main>
    )
}

export default App
