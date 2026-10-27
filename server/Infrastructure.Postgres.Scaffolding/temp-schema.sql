
/*
CREATE TABLE "user"
(
    userid   text NOT NULL,
    username text NOT NULL,
    email    text NOT NULL,
    hash     text NOT NULL,
    salt     text NOT NULL,
    roleid   text NOT NULL,
    CONSTRAINT user_pk PRIMARY KEY (id),
    CONSTRAINT user_role_fk FOREIGN KEY (roleid) REFERENCES "role" (roleid)
);

CREATE TABLE role
(
    roleid                 text,
    name                   text NOT NULL,

    canCreateFinding       bool NOT NULL DEFAULT FALSE,
    canEditFinding         bool NOT NULL DEFAULT FALSE,
    canViewFinding         bool NOT NULL DEFAULT FALSE,
    canDeleteFinding       bool NOT NULL DEFAULT FALSE,

    canCreateUser          bool NOT NULL DEFAULT FALSE,
    canEditUser            bool NOT NULL DEFAULT FALSE,
    canViewUser            bool NOT NULL DEFAULT FALSE,
    canDeleteUser          bool NOT NULL DEFAULT FALSE,

    canAssignUserToFinding bool NOT NULL DEFAULT FALSE,

    CONSTRAINT role_pk PRIMARY KEY (roleid)
);
*/

CREATE TYPE severity AS ENUM('Critical', 'High', 'Medium', 'Low');

CREATE TYPE status AS ENUM('New', 'Triaged', 'In-Progress', 'Remediated', 'Accepted Risk');

CREATE TABLE findings
(
    findingid     text     NOT NULL,
    creatorid     text,
    details       text     NOT NULL,
    severity      severity NOT NULL,
    discoverydate DATE NOT NULL,
    status        status   NOT NULL,
    
    CONSTRAINT findings_pk PRIMARY KEY (findingid),
);

/* to allow multiple users to be assigned to a finding */
CREATE TABLE userFindings(
    assignedid    text,
    findingid     text,

    CONSTRAINT findings_userfindings_fk FOREIGN KEY (findingid) REFERENCES "findings" (findingid),
    CONSTRAINT user_userfindings_fk FOREIGN KEY (assignedid) REFERENCES "userLink" (userid),

)

/* to link to keycloak users */
CREATE TABLE userLink(
    userid text,
    email text,

)

/* I think this is the table we need for the user information (nelson) */
CREATE TABLE userInfo(
    isauthenticated boolean,
    name text,
    email text,
    additionalclaims jsonb

)



