-- This schema is generated based on the current DBContext. Please check the class Seeder to see.
CREATE TABLE user_link (
    user_id uuid NOT NULL,
    email text NOT NULL,
    display_name text,
    CONSTRAINT user_link_pkey PRIMARY KEY (user_id)
);


CREATE TABLE findings (
    finding_id uuid NOT NULL DEFAULT (gen_random_uuid()),
    title text NOT NULL,
    details text NOT NULL,
    affected_asset text NOT NULL,
    severity text,
    status text NOT NULL DEFAULT ('New'::text),
    discovery_date date NOT NULL,
    due_date date,
    creator_id uuid,
    reporter_email text,
    created_at timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT findings_pkey PRIMARY KEY (finding_id),
    CONSTRAINT findings_creator_id_fkey FOREIGN KEY (creator_id) REFERENCES user_link (user_id)
);


CREATE TABLE audit_log (
    audit_id bigint GENERATED ALWAYS AS IDENTITY,
    occurred_at timestamp with time zone NOT NULL DEFAULT (now()),
    finding_id uuid NOT NULL,
    actor_id uuid,
    actor_email text NOT NULL,
    action text NOT NULL,
    field_name text,
    old_value text,
    new_value text,
    CONSTRAINT audit_log_pkey PRIMARY KEY (audit_id),
    CONSTRAINT audit_log_finding_id_fkey FOREIGN KEY (finding_id) REFERENCES findings (finding_id)
);


CREATE TABLE user_findings (
    finding_id uuid NOT NULL,
    user_id uuid NOT NULL,
    CONSTRAINT user_findings_pkey PRIMARY KEY (finding_id, user_id),
    CONSTRAINT user_findings_finding_id_fkey FOREIGN KEY (finding_id) REFERENCES findings (finding_id),
    CONSTRAINT user_findings_user_id_fkey FOREIGN KEY (user_id) REFERENCES user_link (user_id)
);


CREATE INDEX ix_audit_finding_time ON audit_log (finding_id, occurred_at);


CREATE INDEX ix_findings_asset ON findings (affected_asset);


CREATE INDEX "IX_findings_creator_id" ON findings (creator_id);


CREATE INDEX ix_findings_discovery ON findings (discovery_date);


CREATE INDEX ix_findings_due_open ON findings (due_date) WHERE (status = ANY (ARRAY['New'::text, 'Triaged'::text, 'In Progress'::text]));


CREATE INDEX ix_findings_status_severity ON findings (status, severity);


CREATE INDEX ix_user_findings_user ON user_findings (user_id);


