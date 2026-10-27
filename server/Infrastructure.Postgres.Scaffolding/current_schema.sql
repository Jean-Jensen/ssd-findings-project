-- This schema is generated based on the current DBContext. Please check the class Seeder to see.
CREATE TYPE severity AS ENUM ('Critical', 'High', 'Medium', 'Low');
CREATE TYPE status AS ENUM ('New', 'Triaged', 'In-Progress', 'Remediated', 'Accepted Risk');


CREATE TABLE findings (
    findingid text NOT NULL,
    creatorid text,
    details text NOT NULL,
    discoverydate date NOT NULL,
    CONSTRAINT findings_pkey PRIMARY KEY (findingid)
);


CREATE TABLE userinfo (
    isauthenticated boolean,
    name text,
    email text,
    additionalclaims jsonb
);


CREATE TABLE userlink (
    userid text NOT NULL,
    email text,
    CONSTRAINT userlink_pkey PRIMARY KEY (userid)
);


CREATE TABLE userfindings (
    assignedid text NOT NULL,
    findingid text NOT NULL,
    CONSTRAINT userfindings_pkey PRIMARY KEY (assignedid, findingid),
    CONSTRAINT findings_userfindings_fk FOREIGN KEY (findingid) REFERENCES findings (findingid),
    CONSTRAINT user_userfindings_fk FOREIGN KEY (assignedid) REFERENCES userlink (userid)
);


CREATE INDEX "IX_userfindings_findingid" ON userfindings (findingid);


