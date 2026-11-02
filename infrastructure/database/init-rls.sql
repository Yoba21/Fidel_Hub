-- ==============================================================================
-- Fidel Hub: Enterprise Multi-Tenant PostgreSQL Database Initialization
-- Version: 1.1 (Sprint 1 Baseline - All 15 Schemas & Kernel RLS Policies)
-- Aligned with PRD-001, TAD-003, DDS-004, and URP-001
-- ==============================================================================

-- 1. Enable Required Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- 2. Create All 15 Domain Schemas
CREATE SCHEMA IF NOT EXISTS platform;
CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS school;
CREATE SCHEMA IF NOT EXISTS students;
CREATE SCHEMA IF NOT EXISTS admissions;
CREATE SCHEMA IF NOT EXISTS academic;
CREATE SCHEMA IF NOT EXISTS staff;
CREATE SCHEMA IF NOT EXISTS attendance;
CREATE SCHEMA IF NOT EXISTS examinations;
CREATE SCHEMA IF NOT EXISTS finance;
CREATE SCHEMA IF NOT EXISTS library;
CREATE SCHEMA IF NOT EXISTS facilities;
CREATE SCHEMA IF NOT EXISTS health;
CREATE SCHEMA IF NOT EXISTS audit;
CREATE SCHEMA IF NOT EXISTS integration;

-- 3. Create Dedicated Unprivileged Application Role (NOSUPERUSER, NOBYPASSRLS)
DO
$do$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_roles
      WHERE rolname = 'fidel_app_user') THEN
      CREATE ROLE fidel_app_user WITH LOGIN PASSWORD 'fidel_secure_pass' NOSUPERUSER NOBYPASSRLS;
   END IF;
END
$do$;

GRANT CONNECT ON DATABASE fidelhub TO fidel_app_user;

-- Grant USAGE and Table Permissions across all 15 Schemas
DO
$do$
DECLARE
    schema_name text;
    schemas text[] := ARRAY[
        'platform', 'identity', 'school', 'students', 'admissions',
        'academic', 'staff', 'attendance', 'examinations', 'finance',
        'library', 'facilities', 'health', 'audit', 'integration', 'public'
    ];
BEGIN
    FOREACH schema_name IN ARRAY schemas
    LOOP
        EXECUTE format('GRANT USAGE ON SCHEMA %I TO fidel_app_user;', schema_name);
        EXECUTE format('GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA %I TO fidel_app_user;', schema_name);
        EXECUTE format('GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA %I TO fidel_app_user;', schema_name);
        EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO fidel_app_user;', schema_name);
        EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT USAGE, SELECT ON SEQUENCES TO fidel_app_user;', schema_name);
    END LOOP;
END
$do$;

-- 4. Kernel RLS Resolution Helper Functions
CREATE OR REPLACE FUNCTION current_tenant_id() RETURNS uuid AS $$
    SELECT NULLIF(current_setting('app.current_tenant_id', true), '')::uuid;
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION current_branch_id() RETURNS uuid AS $$
    SELECT NULLIF(current_setting('app.current_branch_id', true), '')::uuid;
$$ LANGUAGE sql STABLE;

-- ------------------------------------------------------------------------------
-- 5. Platform Schema Tables
-- ------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS platform.tenants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) UNIQUE NOT NULL,
    legal_name VARCHAR(200) NULL,
    email VARCHAR(255) NULL,
    phone VARCHAR(30) NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'ACTIVE', -- PENDING, ACTIVE, SUSPENDED, CLOSED
    default_currency VARCHAR(3) NOT NULL DEFAULT 'ETB',
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS platform.tenant_quotas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID UNIQUE NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    subscription_plan VARCHAR(50) NOT NULL DEFAULT 'STANDARD', -- STARTER, STANDARD, ENTERPRISE
    max_students INT NOT NULL DEFAULT 1500,
    max_branches INT NOT NULL DEFAULT 3,
    storage_quota_gb INT NOT NULL DEFAULT 20,
    is_cbt_enabled BOOLEAN NOT NULL DEFAULT true,
    subscription_expires_at TIMESTAMPTZ NULL,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_tenant_quotas_tenant_id UNIQUE (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS platform.tenant_settings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    setting_key VARCHAR(100) NOT NULL,
    setting_value JSONB NOT NULL,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_tenant_settings UNIQUE (tenant_id, setting_key),
    CONSTRAINT uq_tenant_settings_tenant_id UNIQUE (tenant_id, id)
);

-- ------------------------------------------------------------------------------
-- 6. School Schema Tables (Branches created before user_branch_access foreign key)
-- ------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS school.branches (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    name VARCHAR(150) NOT NULL,
    code VARCHAR(50) NOT NULL,
    city VARCHAR(100) NOT NULL DEFAULT 'Addis Ababa',
    subcity VARCHAR(100) NULL,
    woreda VARCHAR(50) NULL,
    phone VARCHAR(30) NULL,
    email VARCHAR(255) NULL,
    is_main_branch BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_branch_code UNIQUE (tenant_id, code),
    CONSTRAINT uq_branches_tenant_id UNIQUE (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS school.academic_years (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    code VARCHAR(50) NOT NULL, -- e.g. "2026-2027"
    ethiopian_year INT NOT NULL, -- e.g. 2019
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    is_current BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_academic_year_code UNIQUE (tenant_id, code),
    CONSTRAINT uq_academic_years_tenant_id UNIQUE (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS school.academic_terms (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    academic_year_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL, -- e.g. "Term 1" / "Semester 1"
    sequence INT NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    is_locked BOOLEAN NOT NULL DEFAULT false,
    CONSTRAINT uq_academic_term_seq UNIQUE (tenant_id, academic_year_id, sequence),
    CONSTRAINT uq_academic_terms_tenant_id UNIQUE (tenant_id, id),
    CONSTRAINT fk_academic_terms_year FOREIGN KEY (tenant_id, academic_year_id) REFERENCES school.academic_years (tenant_id, id) ON DELETE CASCADE
);

-- ------------------------------------------------------------------------------
-- 7. Identity Schema Tables
-- ------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS identity.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(200) NOT NULL,
    phone_number VARCHAR(30) NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    email_verified BOOLEAN NOT NULL DEFAULT false,
    two_factor_enabled BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_tenant_user_email UNIQUE (tenant_id, email),
    CONSTRAINT uq_users_tenant_id UNIQUE (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS identity.roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT NULL
);

CREATE TABLE IF NOT EXISTS identity.user_roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    user_id UUID NOT NULL,
    role_id UUID NOT NULL REFERENCES identity.roles(id) ON DELETE CASCADE,
    assigned_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_user_role UNIQUE (tenant_id, user_id, role_id),
    CONSTRAINT uq_user_roles_tenant_id UNIQUE (tenant_id, id),
    CONSTRAINT fk_user_roles_user FOREIGN KEY (tenant_id, user_id) REFERENCES identity.users (tenant_id, id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS identity.user_branch_access (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    user_id UUID NOT NULL,
    branch_id UUID NOT NULL,
    is_default BOOLEAN NOT NULL DEFAULT false,
    granted_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_user_branch UNIQUE (tenant_id, user_id, branch_id),
    CONSTRAINT uq_user_branch_access_tenant_id UNIQUE (tenant_id, id),
    CONSTRAINT fk_user_branch_access_user FOREIGN KEY (tenant_id, user_id) REFERENCES identity.users (tenant_id, id) ON DELETE CASCADE,
    CONSTRAINT fk_user_branch_access_branch FOREIGN KEY (tenant_id, branch_id) REFERENCES school.branches (tenant_id, id) ON DELETE CASCADE
);

-- ------------------------------------------------------------------------------
-- 8. Integration Schema Tables (Outbox Pattern)
-- ------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS integration.outbox_messages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES platform.tenants(id) ON DELETE CASCADE,
    event_type VARCHAR(150) NOT NULL,
    payload JSONB NOT NULL,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    processed_at TIMESTAMPTZ NULL,
    retry_count INT NOT NULL DEFAULT 0,
    error TEXT NULL,
    CONSTRAINT uq_outbox_messages_tenant_id UNIQUE (tenant_id, id)
);

CREATE INDEX IF NOT EXISTS ix_outbox_unprocessed 
ON integration.outbox_messages (occurred_at) WHERE processed_at IS NULL;

-- ------------------------------------------------------------------------------
-- 9. Enable Kernel Row-Level Security (RLS) on Tenant Tables
-- ------------------------------------------------------------------------------
ALTER TABLE platform.tenant_quotas ENABLE ROW LEVEL SECURITY;
ALTER TABLE platform.tenant_quotas FORCE ROW LEVEL SECURITY;

ALTER TABLE platform.tenant_settings ENABLE ROW LEVEL SECURITY;
ALTER TABLE platform.tenant_settings FORCE ROW LEVEL SECURITY;

ALTER TABLE identity.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE identity.users FORCE ROW LEVEL SECURITY;

ALTER TABLE identity.user_roles ENABLE ROW LEVEL SECURITY;
ALTER TABLE identity.user_roles FORCE ROW LEVEL SECURITY;

ALTER TABLE school.branches ENABLE ROW LEVEL SECURITY;
ALTER TABLE school.branches FORCE ROW LEVEL SECURITY;

ALTER TABLE identity.user_branch_access ENABLE ROW LEVEL SECURITY;
ALTER TABLE identity.user_branch_access FORCE ROW LEVEL SECURITY;

ALTER TABLE school.academic_years ENABLE ROW LEVEL SECURITY;
ALTER TABLE school.academic_years FORCE ROW LEVEL SECURITY;

ALTER TABLE school.academic_terms ENABLE ROW LEVEL SECURITY;
ALTER TABLE school.academic_terms FORCE ROW LEVEL SECURITY;

ALTER TABLE integration.outbox_messages ENABLE ROW LEVEL SECURITY;
ALTER TABLE integration.outbox_messages FORCE ROW LEVEL SECURITY;

-- ------------------------------------------------------------------------------
-- 10. Standard RLS Policies (USING & WITH CHECK)
-- Permissive policies (PostgreSQL default) guarantee tenant isolation for fidel_app_user
-- ------------------------------------------------------------------------------
DROP POLICY IF EXISTS rls_tenant_quotas ON platform.tenant_quotas;
CREATE POLICY rls_tenant_quotas ON platform.tenant_quotas FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_tenant_settings ON platform.tenant_settings;
CREATE POLICY rls_tenant_settings ON platform.tenant_settings FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_users ON identity.users;
CREATE POLICY rls_users ON identity.users FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_user_roles ON identity.user_roles;
CREATE POLICY rls_user_roles ON identity.user_roles FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_branches ON school.branches;
CREATE POLICY rls_branches ON school.branches FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_user_branch_access ON identity.user_branch_access;
CREATE POLICY rls_user_branch_access ON identity.user_branch_access FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_academic_years ON school.academic_years;
CREATE POLICY rls_academic_years ON school.academic_years FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_academic_terms ON school.academic_terms;
CREATE POLICY rls_academic_terms ON school.academic_terms FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS rls_outbox_messages ON integration.outbox_messages;
CREATE POLICY rls_outbox_messages ON integration.outbox_messages FOR ALL TO fidel_app_user
USING (tenant_id = current_tenant_id()) WITH CHECK (tenant_id = current_tenant_id());

-- ------------------------------------------------------------------------------
-- 11. Seed Baseline Data: Standard Roles, Demo Tenant & Default Branch
-- ------------------------------------------------------------------------------
-- 11.1 Seed Institutional Roles
INSERT INTO identity.roles (id, code, name, description) VALUES
    ('00000000-0000-0000-0000-000000000001', 'PLATFORM_SUPERADMIN', 'Platform Super Administrator', 'Global SaaS platform and tenant operations.'),
    ('00000000-0000-0000-0000-000000000002', 'TENANT_PRINCIPAL', 'School Principal', 'Tenant-wide academic and institutional leadership.'),
    ('00000000-0000-0000-0000-000000000003', 'BRANCH_ADMIN', 'Branch Campus Administrator', 'Day-to-day operations for assigned campus.'),
    ('00000000-0000-0000-0000-000000000004', 'HEAD_OF_DEPARTMENT', 'Head of Department', 'Curriculum quality, syllabus tracking, and lesson plan approval.'),
    ('00000000-0000-0000-0000-000000000005', 'TEACHER', 'Classroom Teacher', 'Daily attendance, gradebook mark entry, and exam authoring.'),
    ('00000000-0000-0000-0000-000000000006', 'EXAM_COORDINATOR', 'Examination Coordinator', 'Question bank management, paper assembly, and venue server unlocking.'),
    ('00000000-0000-0000-0000-000000000007', 'CASHIER', 'School Cashier', 'Fee structures, batch invoices, and payment receipting.'),
    ('00000000-0000-0000-0000-000000000008', 'REGISTRAR', 'Registrar Officer', 'Admissions pipeline, student biodata, and guardian records.'),
    ('00000000-0000-0000-0000-000000000009', 'LIBRARIAN', 'School Librarian', 'Book cataloging, lending desk, and overdue fines.'),
    ('00000000-0000-0000-0000-000000000010', 'LAB_TECHNICIAN', 'Laboratory Technician', 'Lab apparatus, chemical stocks, and room bookings.'),
    ('00000000-0000-0000-0000-000000000011', 'SCHOOL_NURSE', 'School Health Officer', 'Student clinic visits, vitals, and emergency triage alerts.'),
    ('00000000-0000-0000-0000-000000000012', 'PARENT', 'Parent / Guardian', 'Attendance notifications, report cards, fee payments.'),
    ('00000000-0000-0000-0000-000000000013', 'STUDENT', 'Enrolled Student', 'CBT assessments, timetables, and academic grades.')
ON CONFLICT (code) DO NOTHING;

-- 11.2 Seed Demo Tenant: "Fidel Hub Academy"
INSERT INTO platform.tenants (id, name, code, legal_name, email, phone, status, default_currency) VALUES
    ('11111111-1111-1111-1111-111111111111', 'Fidel Hub Academy', 'fidelhub', 'Fidel Hub Educational Services PLC', 'info@fidelhub.com', '+251911000000', 'ACTIVE', 'ETB')
ON CONFLICT (code) DO NOTHING;

INSERT INTO platform.tenant_quotas (id, tenant_id, subscription_plan, max_students, max_branches, storage_quota_gb, is_cbt_enabled) VALUES
    ('11111111-1111-1111-1111-111111111112', '11111111-1111-1111-1111-111111111111', 'ENTERPRISE', 5000, 10, 100, true)
ON CONFLICT (tenant_id) DO NOTHING;

-- 11.3 Seed Default Campus Branch
INSERT INTO school.branches (id, tenant_id, name, code, city, subcity, woreda, phone, email, is_main_branch, is_active) VALUES
    ('22222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111111', 'Main Bole Campus', 'BOLE-MAIN', 'Addis Ababa', 'Bole', '03', '+251911111111', 'bole@fidelhub.com', true, true)
ON CONFLICT (tenant_id, code) DO NOTHING;

-- 11.4 Seed Default Academic Year (2026/2027 GC / 2019 EC)
INSERT INTO school.academic_years (id, tenant_id, code, ethiopian_year, start_date, end_date, is_current) VALUES
    ('33333333-3333-3333-3333-333333333333', '11111111-1111-1111-1111-111111111111', '2026-2027', 2019, '2026-09-01', '2027-06-30', true)
ON CONFLICT (tenant_id, code) DO NOTHING;

INSERT INTO school.academic_terms (id, tenant_id, academic_year_id, name, sequence, start_date, end_date, is_locked) VALUES
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', '33333333-3333-3333-3333-333333333333', 'Semester 1', 1, '2026-09-01', '2027-01-31', false),
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', '33333333-3333-3333-3333-333333333333', 'Semester 2', 2, '2027-02-01', '2027-06-30', false)
ON CONFLICT DO NOTHING;

-- 11.5 Seed Default Tenant User (Solomon Bekele Hailu)
INSERT INTO identity.users (id, tenant_id, email, password_hash, full_name, phone_number, is_active, email_verified) VALUES
    ('b2f6c0a8-4e12-4d56-9812-7a541289de01', '11111111-1111-1111-1111-111111111111', 'solomon@fidelhub.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4D.xZvy9ma', 'Solomon Bekele Hailu', '+251911223344', true, true)
ON CONFLICT (tenant_id, email) DO NOTHING;

-- Seed User Role (TEACHER & EXAM_COORDINATOR)
INSERT INTO identity.user_roles (id, tenant_id, user_id, role_id) VALUES
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', 'b2f6c0a8-4e12-4d56-9812-7a541289de01', '00000000-0000-0000-0000-000000000005'),
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', 'b2f6c0a8-4e12-4d56-9812-7a541289de01', '00000000-0000-0000-0000-000000000006')
ON CONFLICT DO NOTHING;

-- Seed User Branch Access (Main Bole Campus)
INSERT INTO identity.user_branch_access (id, tenant_id, user_id, branch_id, is_default) VALUES
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', 'b2f6c0a8-4e12-4d56-9812-7a541289de01', '22222222-2222-2222-2222-222222222222', true)
ON CONFLICT DO NOTHING;

