
import { useEffect, useState } from 'react'
import { dashboardClient, apiUrl } from './api'
import type { DashboardSummary, DashboardCount } from './generated-client'
import './DashboardPage.css'

type ReportFinding = {
    findingId: string
    title: string
    details: string
    affectedAsset: string
    severity: string | null
    status: string
    discoveryDate: string
    dueDate: string | null
    createdAt: string
}

function DashboardPage() {
    const [summary, setSummary] = useState<DashboardSummary | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(false)
    const [reloadKey, setReloadKey] = useState(0)

    const [fromDate, setFromDate] = useState('')
    const [toDate, setToDate] = useState('')
    const [affectedAsset, setAffectedAsset] = useState('')

    const [exportLoading, setExportLoading] = useState(false)
    const [exportError, setExportError] = useState('')
    const [reportRows, setReportRows] = useState<ReportFinding[] | null>(null)
    const [printRequested, setPrintRequested] = useState(false)

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

    useEffect(() => {
        if (printRequested && reportRows !== null) {
            window.print()
            setPrintRequested(false)
        }
    }, [printRequested, reportRows])

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

    async function fetchReport(): Promise<ReportFinding[]> {
        const params = new URLSearchParams()

        if (fromDate) params.set('fromDate', fromDate)
        if (toDate) params.set('toDate', toDate)
        if (affectedAsset.trim()) {
            params.set('affectedAsset', affectedAsset.trim())
        }

        const query = params.toString()
        const url = `${apiUrl}/api/dashboard/Report${query ? `?${query}` : ''}`

        const response = await window.fetch(url, {
            method: 'GET',
            credentials: 'include',
            headers: {
                Accept: 'application/json'
            }
        })

        if (!response.ok) {
            throw new Error(
                response.status === 400
                    ? 'Please check the selected date range.'
                    : 'Could not generate the report.'
            )
        }

        return await response.json() as ReportFinding[]
    }

    async function exportJson() {
        setExportLoading(true)
        setExportError('')

        try {
            const findings = await fetchReport()

            const report = {
                generatedAt: new Date().toISOString(),
                filters: {
                    fromDate: fromDate || null,
                    toDate: toDate || null,
                    affectedAsset: affectedAsset.trim() || null
                },
                totalFindings: findings.length,
                findings
            }

            const blob = new Blob(
                [JSON.stringify(report, null, 2)],
                { type: 'application/json' }
            )

            const downloadUrl = URL.createObjectURL(blob)
            const link = document.createElement('a')

            link.href = downloadUrl
            link.download = 'findings-report.json'
            document.body.appendChild(link)
            link.click()
            link.remove()

            URL.revokeObjectURL(downloadUrl)
        } catch (error) {
            setExportError(
                error instanceof Error
                    ? error.message
                    : 'Could not export JSON report.'
            )
        } finally {
            setExportLoading(false)
        }
    }

    async function exportPdf() {
        setExportLoading(true)
        setExportError('')
        setReportRows(null)

        try {
            const findings = await fetchReport()
            setReportRows(findings)
            setPrintRequested(true)
        } catch (error) {
            setExportError(
                error instanceof Error
                    ? error.message
                    : 'Could not generate PDF report.'
            )
        } finally {
            setExportLoading(false)
        }
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

                    <section className="dashboard-panel dashboard-export">
                        <h2>Export Findings Report</h2>
                        <p>
                            Choose optional filters and export the matching findings.
                        </p>

                        <div className="report-filters">
                            <label>
                                Discovery date from
                                <input
                                    type="date"
                                    value={fromDate}
                                    onChange={event => setFromDate(event.target.value)}
                                />
                            </label>

                            <label>
                                Discovery date to
                                <input
                                    type="date"
                                    value={toDate}
                                    onChange={event => setToDate(event.target.value)}
                                />
                            </label>

                            <label>
                                Affected asset
                                <input
                                    type="text"
                                    placeholder="e.g. Production server"
                                    value={affectedAsset}
                                    onChange={event => setAffectedAsset(event.target.value)}
                                />
                            </label>
                        </div>

                        <div className="report-export-actions">
                            <button
                                onClick={exportJson}
                                disabled={exportLoading}
                            >
                                Export JSON
                            </button>

                            <button
                                onClick={exportPdf}
                                disabled={exportLoading}
                            >
                                {exportLoading ? 'Generating...' : 'Export PDF'}
                            </button>

                            <button
                                type="button"
                                onClick={() => {
                                    setFromDate('')
                                    setToDate('')
                                    setAffectedAsset('')
                                    setExportError('')
                                }}
                                disabled={exportLoading}
                            >
                                Clear filters
                            </button>
                        </div>

                        {fromDate && toDate && fromDate > toDate && (
                            <p className="report-error">
                                The start date cannot be later than the end date.
                            </p>
                        )}

                        {exportError && (
                            <p className="report-error">{exportError}</p>
                        )}
                    </section>
                </>
            )}

            {reportRows !== null && (
                <section className="report-print-area">
                    <h1>Findings Report</h1>
                    <p>Generated: {new Date().toLocaleString()}</p>

                    <p>
                        Discovery date from: {fromDate || 'Any'}
                        {' | '}
                        Discovery date to: {toDate || 'Any'}
                    </p>
                    <p>
                        Affected asset: {affectedAsset.trim() || 'Any'}
                    </p>
                    <p>Total findings: {reportRows.length}</p>

                    {reportRows.length === 0 ? (
                        <p>No findings match the selected filters.</p>
                    ) : (
                        <table>
                            <thead>
                            <tr>
                                <th>Title</th>
                                <th>Details</th>
                                <th>Affected Asset</th>
                                <th>Severity</th>
                                <th>Status</th>
                                <th>Discovery Date</th>
                                <th>Due Date</th>
                            </tr>
                            </thead>
                            <tbody>
                            {reportRows.map(finding => (
                                <tr key={finding.findingId}>
                                    <td>{finding.title}</td>
                                    <td>{finding.details}</td>
                                    <td>{finding.affectedAsset}</td>
                                    <td>{finding.severity || 'Unrated'}</td>
                                    <td>{finding.status}</td>
                                    <td>{finding.discoveryDate}</td>
                                    <td>{finding.dueDate || 'Not set'}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    )}
                </section>
            )}
        </main>
    )
}

export default DashboardPage
