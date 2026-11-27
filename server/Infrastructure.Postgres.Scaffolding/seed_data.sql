
BEGIN;

-- 1. USERS FROM KEYCLOAK
INSERT INTO user_link (user_id, email, display_name)
VALUES
    ('12bbad92-c9bf-5efc-90ec-f113f3b5fcb6', 'analyst@example.com', 'Analyst'),
    ('f1400ed8-86a3-5e57-b05c-bbb507a67ee1', 'auditor@example.com', 'Auditor'),
    ('479c556b-cecd-59a5-8b3f-bef70ae45cb7', 'developer@example.com', 'Developer'),
    ('f24af656-68f3-5d60-899e-c1cc36757d3d', 'project_manager@example.com', 'Project Manager');


-- 2. FINDINGS
INSERT INTO findings
(finding_id, title, details, affected_asset, severity, status,
 discovery_date, due_date, creator_id, reporter_email)
VALUES
    ('aaaaaaaa-0000-0000-0000-000000000001',
     'SQL injection in login form',
     'User input is not properly validated in the login form.',
     'Authentication service', 'Critical', 'New',
     CURRENT_DATE - 12, CURRENT_DATE - 2,
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000002',
     'Missing security headers',
     'Several recommended HTTP security headers are missing.',
     'Web application', 'High', 'Triaged',
     CURRENT_DATE - 8, CURRENT_DATE + 2,
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000003',
     'Outdated JavaScript library',
     'An outdated dependency contains known vulnerabilities.',
     'Frontend application', 'Medium', 'In Progress',
     CURRENT_DATE - 15, CURRENT_DATE - 1,
     '479c556b-cecd-59a5-8b3f-bef70ae45cb7', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000004',
     'Weak password policy',
     'The current password policy does not enforce sufficient complexity.',
     'User management', 'High', 'New',
     CURRENT_DATE - 3, CURRENT_DATE + 10,
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000005',
     'Exposed server information',
     'Server version information is visible in HTTP responses.',
     'Production server', 'Low', 'Remediated',
     CURRENT_DATE - 20, CURRENT_DATE - 10,
     '479c556b-cecd-59a5-8b3f-bef70ae45cb7', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000006',
     'Insufficient access restrictions',
     'Some application resources may be accessible without the correct permissions.',
     'Admin dashboard', 'Critical', 'In Progress',
     CURRENT_DATE - 6, CURRENT_DATE + 1,
     'f24af656-68f3-5d60-899e-c1cc36757d3d', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000007',
     'Unencrypted internal communication',
     'Internal communication between services is not encrypted.',
     'Internal API', 'Medium', 'Accepted Risk',
     CURRENT_DATE - 30, CURRENT_DATE - 15,
     'f24af656-68f3-5d60-899e-c1cc36757d3d', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000008',
     'Missing input validation',
     'A form accepts unexpected input without sufficient validation.',
     'Contact form', 'Low', 'Triaged',
     CURRENT_DATE - 4, NULL,
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6', NULL),

    ('aaaaaaaa-0000-0000-0000-000000000009',
     'Potential information disclosure',
     'An error message may expose internal application details.',
     'Error handling service', 'Medium', 'New',
     CURRENT_DATE - 2, CURRENT_DATE + 7,
     NULL, 'external@example.com'),

    ('aaaaaaaa-0000-0000-0000-000000000010',
     'Unnecessary open network port',
     'A network port is exposed although the application does not require it.',
     'Application server', 'Low', 'Remediated',
     CURRENT_DATE - 18, CURRENT_DATE - 5,
     '479c556b-cecd-59a5-8b3f-bef70ae45cb7', NULL);


-- 3. USER ASSIGNMENTS
INSERT INTO user_findings (finding_id, user_id)
VALUES
    ('aaaaaaaa-0000-0000-0000-000000000001', '479c556b-cecd-59a5-8b3f-bef70ae45cb7'),
    ('aaaaaaaa-0000-0000-0000-000000000001', '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6'),
    ('aaaaaaaa-0000-0000-0000-000000000002', '479c556b-cecd-59a5-8b3f-bef70ae45cb7'),
    ('aaaaaaaa-0000-0000-0000-000000000003', '479c556b-cecd-59a5-8b3f-bef70ae45cb7'),
    ('aaaaaaaa-0000-0000-0000-000000000003', 'f24af656-68f3-5d60-899e-c1cc36757d3d'),
    ('aaaaaaaa-0000-0000-0000-000000000004', '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6'),
    ('aaaaaaaa-0000-0000-0000-000000000005', '479c556b-cecd-59a5-8b3f-bef70ae45cb7'),
    ('aaaaaaaa-0000-0000-0000-000000000006', 'f24af656-68f3-5d60-899e-c1cc36757d3d'),
    ('aaaaaaaa-0000-0000-0000-000000000006', '479c556b-cecd-59a5-8b3f-bef70ae45cb7'),
    ('aaaaaaaa-0000-0000-0000-000000000007', 'f24af656-68f3-5d60-899e-c1cc36757d3d'),
    ('aaaaaaaa-0000-0000-0000-000000000008', '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6'),
    ('aaaaaaaa-0000-0000-0000-000000000008', '479c556b-cecd-59a5-8b3f-bef70ae45cb7'),
    ('aaaaaaaa-0000-0000-0000-000000000009', 'f1400ed8-86a3-5e57-b05c-bbb507a67ee1'),
    ('aaaaaaaa-0000-0000-0000-000000000010', '479c556b-cecd-59a5-8b3f-bef70ae45cb7');


-- 4. AUDIT HISTORY
INSERT INTO audit_log
(finding_id, actor_id, actor_email, action, field_name,
 old_value, new_value, occurred_at)
VALUES
    ('aaaaaaaa-0000-0000-0000-000000000001',
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6',
     'analyst@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '12 days'),

    ('aaaaaaaa-0000-0000-0000-000000000001',
     'f24af656-68f3-5d60-899e-c1cc36757d3d',
     'project_manager@example.com', 'updated', 'severity', 'High', 'Critical', NOW() - INTERVAL '12 hours'),

    ('aaaaaaaa-0000-0000-0000-000000000002',
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6',
     'analyst@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '8 days'),

    ('aaaaaaaa-0000-0000-0000-000000000002',
     'f24af656-68f3-5d60-899e-c1cc36757d3d',
     'project_manager@example.com', 'updated', 'status', 'New', 'Triaged', NOW() - INTERVAL '8 hours'),

    ('aaaaaaaa-0000-0000-0000-000000000003',
     '479c556b-cecd-59a5-8b3f-bef70ae45cb7',
     'developer@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '15 days'),

    ('aaaaaaaa-0000-0000-0000-000000000003',
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6',
     'analyst@example.com', 'assigned', NULL, NULL, NULL, NOW() - INTERVAL '5 hours'),

    ('aaaaaaaa-0000-0000-0000-000000000004',
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6',
     'analyst@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '3 days'),

    ('aaaaaaaa-0000-0000-0000-000000000005',
     '479c556b-cecd-59a5-8b3f-bef70ae45cb7',
     'developer@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '20 days'),

    ('aaaaaaaa-0000-0000-0000-000000000006',
     'f24af656-68f3-5d60-899e-c1cc36757d3d',
     'project_manager@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '6 days'),

    ('aaaaaaaa-0000-0000-0000-000000000007',
     'f24af656-68f3-5d60-899e-c1cc36757d3d',
     'project_manager@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '30 days'),

    ('aaaaaaaa-0000-0000-0000-000000000008',
     '12bbad92-c9bf-5efc-90ec-f113f3b5fcb6',
     'analyst@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '4 days'),

    ('aaaaaaaa-0000-0000-0000-000000000009',
     NULL,
     'external@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '2 days'),

    ('aaaaaaaa-0000-0000-0000-000000000010',
     '479c556b-cecd-59a5-8b3f-bef70ae45cb7',
     'developer@example.com', 'created', NULL, NULL, NULL, NOW() - INTERVAL '18 days');

COMMIT;
