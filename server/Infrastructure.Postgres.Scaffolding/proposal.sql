-- ==============================================
-- CHANGED = different from the old schema, NEW = didn't exist before
-- ==============================================

-- CHANGED: same idea as the old userlink, just snake_case and with name
CREATE TABLE user_link (
                           user_id      text PRIMARY KEY,   -- id from keycloak
                           email        text NOT NULL,
                           display_name text
);

-- CHANGED: old table only had id, creator, details and date. now it has everything the spec asks for,
-- (severity, status, overdue) 
CREATE TABLE findings (
    -- chamged to uuid
                          finding_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),

                          title          text NOT NULL,
                          details        text NOT NULL,
                          affected_asset text NOT NULL,    -- what is vulnerable, like "login page"

    -- NULLABLE on purpose: an external reporter can't know the severity.
    -- analyst sets it during triage. NULL = not rated yet.
    -- CHECK = database rejects any value that is not in the list.
                          severity       text CHECK (severity IN ('Critical', 'High', 'Medium', 'Low')),

    -- CHANGED: text + CHECK instead of postgres enums. easier to change later ,
    -- and we can use the exact wording from the spec ("In Progress", no hyphen).
                          status         text NOT NULL DEFAULT 'New'
                              CHECK (status IN ('New', 'Triaged', 'In Progress',
                                                'Remediated', 'Accepted Risk')),

                          discovery_date date NOT NULL,
                          due_date       date,    -- NEW: the SLA deadline (empty until someone sets it). needed for "overdue"

    -- who reported it: a logged in user OR an outside person
                          creator_id     text REFERENCES user_link (user_id),  -- REFERENCES = must exist in user_link (if theres a valuee)
                          reporter_email text,                                 -- NEW: outside reporter's email (they have no account)

                          created_at     timestamptz NOT NULL DEFAULT now(),

    -- every finding must say where it came from: a user or an email
                          CHECK (creator_id IS NOT NULL OR reporter_email IS NOT NULL),
    -- NEW: due date can't be before the discovery date
                          CHECK (due_date IS NULL OR due_date >= discovery_date)
);

-- CHANGED: snake_case names, and the primary key starts with finding_id
CREATE TABLE user_findings (
                               finding_id uuid NOT NULL REFERENCES findings (finding_id),
                               user_id    text NOT NULL REFERENCES user_link (user_id),

    -- the same person can't be added twice to the same finding
                               PRIMARY KEY (finding_id, user_id)
);

-- audit_log: NEW (nil's part). one row per action on a finding. history only, never edited.
CREATE TABLE audit_log (
                           audit_id    bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                           occurred_at timestamptz NOT NULL DEFAULT now(),

    -- no cascade delete, history must stay. findings are never hard-deleted anyway
                           finding_id  uuid NOT NULL REFERENCES findings (finding_id),

                           actor_id    text,           -- who did it (keycloak id). empty for outside reporters, they have no account
                           actor_email text NOT NULL,  -- a COPY of their email at that moment, so the log is still right if they change email later


    -- what happened. limited list so typos are rejected (same idea as status)
                           action      text NOT NULL
                               CHECK (action IN ('created', 'updated', 'assigned', 'unassigned')),

    -- only for 'updated': which field changed, and the values before and after
                           field_name  text,    -- 'status', 'severity', 'due_date', 'title' ...
                           old_value   text,
                           new_value   text,
    -- for 'details' leave old_value and new_value EMPTY (only record that it changed),
    -- otherwise the log leaks what FIND-11 wants encrypted
    -- for 'assigned'/'unassigned' put the user id in new_value/old_value
                            -- an 'updated' row must say which field
                            CHECK (action <> 'updated' OR field_name IS NOT NULL)
);

-- nobody can edit or delete audit rows (inserts still work).
CREATE FUNCTION audit_log_immutable() RETURNS trigger AS $$
BEGIN
    RAISE EXCEPTION 'audit_log is append-only';
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER audit_log_no_change
    BEFORE UPDATE OR DELETE ON audit_log
FOR EACH ROW EXECUTE FUNCTION audit_log_immutable();

-- NEW: TRUNCATE (delete everything at once) skips row triggers, so it needs its own one
-- careful: this also blocks our own seeder/tests if they reset tables with TRUNCATE.
-- if that happens, drop the database/container and rebuild, or remove this trigger in dev
CREATE TRIGGER audit_log_no_truncate
    BEFORE TRUNCATE ON audit_log
FOR EACH STATEMENT EXECUTE FUNCTION audit_log_immutable();

-- dashboard: open findings by severity and status
CREATE INDEX ix_findings_status_severity ON findings (status, severity);

-- dashboard: overdue. partial index = only open findings.
-- if we change what "open" means, change this list too
CREATE INDEX ix_findings_due_open ON findings (due_date)
    WHERE status IN ('New', 'Triaged', 'In Progress');

-- NEW: export by asset and by time period (FR-RPT-02), also filter by asset (FIND-05)
CREATE INDEX ix_findings_asset ON findings (affected_asset);
CREATE INDEX ix_findings_discovery ON findings (discovery_date);

-- "which findings is this user assigned to". the primary key already covers
-- "who is assigned to this finding" (it starts with finding_id), but not this direction
CREATE INDEX ix_user_findings_user ON user_findings (user_id);

-- history of one finding in time order
CREATE INDEX ix_audit_finding_time ON audit_log (finding_id, occurred_at);

-- DEFINITIONS (agree as a team, write in docs/):
--   Open    = New, Triaged, In Progress
--   Closed  = Remediated, Accepted Risk   (what means Accepted Risk for us?)
--   Overdue = open AND due_date < today