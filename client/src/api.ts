import { AuthClient, UsersClient } from './generated-client'

export const apiUrl = 'http://localhost:5001'

const cookieAuth = {
  fetch: (url: RequestInfo, options?: RequestInit) =>
    window.fetch(url, { ...options, credentials: 'include' }),
}

export const authClient = new AuthClient(apiUrl, cookieAuth)
export const usersClient = new UsersClient(apiUrl, cookieAuth)
