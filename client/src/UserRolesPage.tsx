import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { usersClient } from './api'
import { ApiException } from './generated-client'
import type { ManagedUsersPageDto } from './generated-client'
import './UserRolesPage.css'

const roles = ['Analyst', 'Developer', 'PM', 'Auditor']
const pageSize = 25

function UserRolesPage() {
  const [offset, setOffset] = useState(0)

  return (
    <main className="user-roles-page">
      <a href="/">Back to home</a>
      <h1>Manage user roles</h1>
      <UserRolesList key={offset} offset={offset} onPageChange={setOffset} />
    </main>
  )
}

function UserRolesList({ offset, onPageChange }: {
  offset: number
  onPageChange: (offset: number) => void
}) {
  const [page, setPage] = useState<ManagedUsersPageDto>()
  const [loadError, setLoadError] = useState('')
  const [saving, setSaving] = useState(false)
  const [notice, setNotice] = useState<{ text: string; error: boolean }>()

  useEffect(() => {
    let active = true

    usersClient.listManagedUsers(offset, pageSize)
      .then(result => {
        if (active) setPage(result)
      })
      .catch(error => {
        if (active) setLoadError(failureMessage(error, 'Could not load users. Refresh to try again.'))
      })

    return () => {
      active = false
    }
  }, [offset])

  async function saveRole(event: FormEvent<HTMLFormElement>, userId: string, username: string) {
    event.preventDefault()
    if (saving) return

    const form = event.currentTarget
    const role = new FormData(form).get('role')
    if (typeof role !== 'string' || !roles.includes(role)) return

    setSaving(true)
    setNotice(undefined)
    try {
      await usersClient.changeUserRole(userId, { role })
      setPage(current => ({
        ...current,
        users: current?.users?.map(user => user.id === userId ? { ...user, roles: [role] } : user),
      }))
      form.reset()
      setNotice({ text: `Updated ${username}’s role to ${role}.`, error: false })
    } catch (error) {
      setNotice({ text: failureMessage(error, `Could not update ${username}’s role.`), error: true })
    } finally {
      setSaving(false)
    }
  }

  const users = page?.users ?? []

  return (
    <>
      {loadError && <p role="alert">{loadError}</p>}
      {!page && !loadError && <p role="status">Loading users…</p>}
      {page && users.length === 0 && <p>No users to manage on this page.</p>}
      {users.length > 0 && (
        <div className="user-roles-table">
          <table>
            <thead>
              <tr>
                <th scope="col">User</th>
                <th scope="col">Current roles</th>
                <th scope="col">Change role</th>
              </tr>
            </thead>
            <tbody>
              {users.map(({ id, username, roles: currentRoles }) => {
                if (!id) return null
                const name = username || id

                return (
                  <tr key={id}>
                    <th scope="row">{name}</th>
                    <td>{currentRoles?.join(', ') || 'None'}</td>
                    <td>
                      <form onSubmit={event => void saveRole(event, id, name)}>
                        <select name="role" aria-label={`New role for ${name}`} defaultValue="" required disabled={saving}>
                          <option value="" disabled>Select role</option>
                          {roles.map(role => <option key={role}>{role}</option>)}
                        </select>
                        <button type="submit" aria-label={`Save role for ${name}`} disabled={saving}>Save</button>
                      </form>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}
      {saving && <p role="status">Saving role…</p>}
      {notice && <p role={notice.error ? 'alert' : 'status'}>{notice.text}</p>}
      {(offset > 0 || page?.hasMore) && (
        <nav aria-label="User pages">
          <button disabled={saving || offset === 0} onClick={() => onPageChange(offset - pageSize)}>Previous</button>
          <button disabled={saving || !page?.hasMore} onClick={() => onPageChange(offset + pageSize)}>Next</button>
        </nav>
      )}
    </>
  )
}

function failureMessage(error: unknown, fallback: string) {
  if (error instanceof ApiException && (error.status === 401 || error.status === 403)) {
    return 'Access denied. A project manager session is required.'
  }
  return fallback
}

export default UserRolesPage
