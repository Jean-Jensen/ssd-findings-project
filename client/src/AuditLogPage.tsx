
import { useEffect, useState } from 'react'
import { ApiException, type AuditLogEntryDto } from './generated-client'
import { auditClient } from './api'
import './AuditLogPage.css'

const pageSize = 25

function formatDate(value?: Date): string {
    if (!value) return '—'

    const date = new Date(value)

    if (Number.isNaN(date.getTime())) return '—'

    return date.toLocaleString()
}

function showValue(value?: string): string {
    return value ?? '—'
}

function isValidGuid(value: string): boolean {
    const guidPattern =
        /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i

    return guidPattern.test(value)
}

export default function AuditLogPage() {
    const [entries, setEntries] = useState<AuditLogEntryDto[]>([])
    const [total, setTotal] = useState(0)

    const [findingIdInput, setFindingIdInput] = useState('')
    const [appliedFindingId, setAppliedFindingId] = useState<string | null>(null)

    const [offset, setOffset] = useState(0)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    // Changing this value forces the audit log to reload,
    // even when the applied filter is already null.
    const [reloadKey, setReloadKey] = useState(0)

    useEffect(() => {
        let active = true

        async function loadAuditLog() {
            setLoading(true)
            setError(null)

            try {
                const result = await auditClient.getAuditLog(
                    appliedFindingId,
                    offset,
                    pageSize
                )

                if (!active) return

                setEntries(result.items ?? [])
                setTotal(result.total ?? 0)
            } catch (err: unknown) {
                if (!active) return

                if (err instanceof ApiException) {
                    if (err.status === 401) {
                        setError('You must be logged in to view the audit log.')
                    } else if (err.status === 403) {
                        setError('You do not have permission to view the audit log.')
                    } else if (err.status === 404) {
                        setError('Finding not found. No finding exists with this ID.')
                    } else if (err.status === 400) {
                        setError('Invalid request. Please check the finding ID and try again.')
                    } else {
                        setError(`Could not load the audit log. Server error: ${err.status}`)
                    }
                } else {
                    setError('Could not connect to the server. Please try again.')
                }

                setEntries([])
                setTotal(0)
            } finally {
                if (active) {
                    setLoading(false)
                }
            }
        }

        loadAuditLog()

        return () => {
            active = false
        }
    }, [appliedFindingId, offset, reloadKey])

    function handleFilterSubmit(
        event: Parameters<NonNullable<React.ComponentProps<'form'>['onSubmit']>>[0]
    ) {
        event.preventDefault()

        const newFindingId = findingIdInput.trim()

        if (newFindingId === '') {
            clearFilter()
            return
        }

        if (!isValidGuid(newFindingId)) {
            setError('Please enter a valid finding ID. The ID must be in GUID format.')
            return
        }

        setError(null)
        setOffset(0)
        setAppliedFindingId(newFindingId)
        setReloadKey((previous) => previous + 1)
    }

    function clearFilter() {
        setFindingIdInput('')
        setError(null)
        setOffset(0)
        setAppliedFindingId(null)
        setReloadKey((previous) => previous + 1)
    }

    const firstEntry = total === 0 ? 0 : offset + 1
    const lastEntry = Math.min(offset + pageSize, total)

    return (
        <main className="audit-log-page">
            <nav className="audit-log-top-navigation">
                <a href="/">← Back to home</a>
            </nav>

            <header className="audit-log-header">
                <div>
                    <h1>Audit log</h1>
                    <p>
                        Review recorded changes and actions performed in the system.
                    </p>
                </div>

                <div className="audit-log-total">
                    <span className="audit-log-total-label">Total records</span>
                    <strong>{loading ? '—' : total}</strong>
                </div>
            </header>

            <section className="audit-log-filter-panel">
                <div className="audit-log-filter-heading">
                    <h2>Filter records</h2>
                    <p>Search for audit entries associated with a specific finding.</p>
                </div>

                <form className="audit-log-filter" onSubmit={handleFilterSubmit}>
                    <div className="audit-log-input-group">
                        <label htmlFor="findingId">Finding ID</label>

                        <input
                            id="findingId"
                            type="text"
                            value={findingIdInput}
                            onChange={(event) => setFindingIdInput(event.target.value)}
                            placeholder="Enter finding GUID"
                        />
                    </div>

                    <div className="audit-log-filter-actions">
                        <button type="submit">Apply filter</button>

                        <button
                            type="button"
                            className="audit-log-clear-button"
                            onClick={clearFilter}
                        >
                            Clear filter
                        </button>
                    </div>
                </form>
            </section>

            {error && (
                <p className="audit-log-error" role="alert">
                    {error}
                </p>
            )}

            <section className="audit-log-results">
                <div className="audit-log-results-heading">
                    <div>
                        <h2>Recorded activity</h2>
                        <p>
                            {appliedFindingId
                                ? 'Showing records for the selected finding.'
                                : 'Showing all recorded activity.'}
                        </p>
                    </div>

                    {appliedFindingId && (
                        <span className="audit-log-filter-indicator">
                            Filter active
                        </span>
                    )}
                </div>

                {loading ? (
                    <p className="audit-log-message">Loading audit records...</p>
                ) : error ? null : entries.length === 0 ? (
                    <p className="audit-log-message">
                        {appliedFindingId
                            ? 'No audit entries found for this finding.'
                            : 'There are currently no audit entries.'}
                    </p>
                ) : (
                    <>
                        <div className="audit-log-table">
                            <table>
                                <thead>
                                <tr>
                                    <th>Date and time</th>
                                    <th>Actor</th>
                                    <th>Action</th>
                                    <th>Finding ID</th>
                                    <th>Field</th>
                                    <th>Old value</th>
                                    <th>New value</th>
                                </tr>
                                </thead>

                                <tbody>
                                {entries.map((entry) => (
                                    <tr key={entry.auditId}>
                                        <td className="audit-log-date">
                                            {formatDate(entry.occurredAt)}
                                        </td>
                                        <td>{showValue(entry.actorEmail)}</td>
                                        <td>
                                                <span className="audit-log-action">
                                                    {showValue(entry.action)}
                                                </span>
                                        </td>
                                        <td className="audit-log-id">
                                            {showValue(entry.findingId)}
                                        </td>
                                        <td>{showValue(entry.fieldName)}</td>
                                        <td>{showValue(entry.oldValue)}</td>
                                        <td>{showValue(entry.newValue)}</td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </div>

                        <div className="audit-log-footer">
                            <p className="audit-log-count">
                                Showing {firstEntry}–{lastEntry} of {total} records
                            </p>

                            <nav className="audit-log-navigation">
                                <button
                                    type="button"
                                    disabled={offset === 0 || loading}
                                    onClick={() => setOffset(Math.max(0, offset - pageSize))}
                                >
                                    Previous
                                </button>

                                <button
                                    type="button"
                                    disabled={offset + pageSize >= total || loading}
                                    onClick={() => setOffset(offset + pageSize)}
                                >
                                    Next
                                </button>
                            </nav>
                        </div>
                    </>
                )}
            </section>
        </main>
    )
}
