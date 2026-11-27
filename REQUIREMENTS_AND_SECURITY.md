# Project Report

## 1. Requirements

The team implemented all requirements classified as MUST and SHOULD. No changes were made to the original requirements, and no additional requirements were added.

| Requirement area | Responsible team member | Status |
|---|---|---|
| AUTH – Authentication and authorization | Nelson| Implemented and tested|
| FIND – Findings functionality | Jean | Implemented and tested|
| FR-RPT – Reporting functionality | Nilofar | Implemented and tested |
| AUDIT – Audit functionality | Nilofar | Implemented and tested |

## 2. Cryptography

[To be completed]


## 3. Security Practices Throughout the SDLC

[To be completed]


## 4. Threat Model

### AUDIT and FR-RPT

The functionality covered by this section includes audit event recording, audit log viewing, dashboard summaries and report generation and export.

**Main assets:**
- Audit records and their history.
- User identifiers and email addresses.
- Finding information displayed in dashboards and reports.
- Authentication and authorization information.

**Main entry points and trust boundaries:**
- Frontend-to-backend API communication.
- Backend-to-database communication.
- Backend communication with Keycloak.

**Identified threats and mitigations:**

| Threat | Mitigation |
|---|---|
| Unauthorized access to audit records | Backend authorization checks |
| Unauthorized access to dashboard and report data | Backend authorization policies |
| Modification or deletion of audit records | Append-only database trigger preventing UPDATE, DELETE and TRUNCATE |
| Unauthorized access to sensitive finding information | Access restrictions and server-side authorization |

**Verification:** The AUDIT and FR-RPT functionality was implemented and tested. The authorization checks and append-only database protection provide security controls for these features.


## 5. LLM and Agent Disclosure

**Which parts were co-authored with AI?**

**How was AI used?**

**How was quality and security ensured?**

AI-generated suggestions were not accepted blindly. Code that was provided by AI was reviewed and adapted before use. The application was run and the implemented functionality was tested to verify that it behaved as expected. Implementation decisions and final testing remained the developer's responsibility.
