
import { useEffect, useState } from 'react'
import { dashboardClient } from './api'
import type { DashboardSummary, DashboardCount } from './generated-client'
import './DashboardPage.css'

function DashboardPage() {
    const [summary, setSummary] = useState<DashboardSummary | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(false)
    const [reloadKey, setReloadKey] = useState(0)

    useEffect(() => {
        let active = true

        setLoading(true)
        setError(false)

        dashboardClient.getSummary()
            .then(result => {
                if (active) {
                    setSummary(result)
                }
            })
            .catch(() => {
                if (active) {
                    setError(true)
                }
            })
            .finally(() => {
                if (active) {
                    setLoading(false)
                }
            })

        return () => {
            active = false
        }
    }, [reloadKey])

    function renderBreakdown(title: string, items: DashboardCount[] | undefined) {
        const counts = items ?? []
        const maxCount = Math.max(
            ...counts.map(item => item.count ?? 0),
            0
        )

        return (
            <section className="dashboard-panel">
                <h2>{title}</h2>

                {counts.length === 0 ? (
                    <p className="dashboard-empty">No data available.</p>
                ) : (
                    <div className="dashboard-breakdown">
                        {counts.map((item, index) => {
                            const count = item.count ?? 0
                            const percentage = maxCount > 0
                                ? (count / maxCount) * 100
                                : 0

                            return (
                                <div
                                    className="dashboard-breakdown-item"
                                    key={`${item.label}-${index}`}
                                >
                                    <div className="dashboard-breakdown-heading">
                                        <span>{item.label || 'Unknown'}</span>
                                        <strong>{count}</strong>
                                    </div>

                                    <div className="dashboard-bar-background">
                                        <div
                                            className="dashboard-bar"
                                            style={{ width: `${percentage}%` }}
                                        />
                                    </div>
                                </div>
                            )
                        })}
                    </div>
                )}
            </section>
        )
    }

    return (
        <main className="dashboard-page">
            <header className="dashboard-header">
                <div>
                    <a href="/" className="dashboard-back-link">
                        ← Back to home
                    </a>
                    <h1>Dashboard</h1>
                    <p>Overview of findings and their current status.</p>
                </div>

                <button
                    className="dashboard-refresh-button"
                    onClick={() => setReloadKey(value => value + 1)}
                    disabled={loading}
                >
                    {loading ? 'Loading...' : 'Refresh'}
                </button>
            </header>

            {error ? (
                <div className="dashboard-error">
                    <p>Could not load dashboard data.</p>
                    <button onClick={() => setReloadKey(value => value + 1)}>
                        Try again
                    </button>
                </div>
            ) : loading ? (
                <p className="dashboard-loading">Loading dashboard...</p>
            ) : (
                <>
                    <section className="dashboard-summary">
                        <article className="dashboard-summary-card">
        <span className="dashboard-card-label">
            Total Open Findings
        </span>
                            <strong>{summary?.totalOpen ?? 0}</strong>
                        </article>

                        <article className="dashboard-summary-card">
        <span className="dashboard-card-label">
            Overdue Findings
        </span>
                            <strong>{summary?.overdue ?? 0}</strong>
                        </article>

                        <article className="dashboard-summary-card">
        <span className="dashboard-card-label">
            Within SLA
        </span>
                            <strong>
                                {Math.max(
                                    0,
                                    (summary?.totalOpen ?? 0) - (summary?.overdue ?? 0)
                                )}
                            </strong>
                        </article>
                    </section>

                    <section className="dashboard-breakdowns">
                        {renderBreakdown('Findings by Severity', summary?.bySeverity)}
                        {renderBreakdown('Findings by Status', summary?.byStatus)}
                    </section>
                </>
            )}
        </main>
    )
}

export default DashboardPage
