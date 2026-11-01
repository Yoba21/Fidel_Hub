-- ==============================================================================
-- Fidel Hub: Multi-Tenant Row Level Security (RLS) Initialization
-- Aligned with PRD: Student SIS, Fees & Invoicing, and Offline CBT Exams
-- ==============================================================================

-- 1. Create dedicated application role (non-superuser to enforce RLS)
DO
$do$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_roles
      WHERE rolname = 'fidel_app_user') THEN
      CREATE ROLE fidel_app_user WITH LOGIN PASSWORD 'fidel_secure_pass';
   END IF;
END
$do$;

GRANT CONNECT ON DATABASE fidelhub TO fidel_app_user;
GRANT USAGE ON SCHEMA public TO fidel_app_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO fidel_app_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO fidel_app_user;

-- 2. Utility functions for Tenant Resolution
CREATE OR REPLACE FUNCTION current_tenant_id() RETURNS uuid AS $$
    SELECT NULLIF(current_setting('app.current_tenant_id', true), '')::uuid;
$$ LANGUAGE sql STABLE;

-- 3. Core Tables Schema
CREATE TABLE IF NOT EXISTS tenants (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    subdomain VARCHAR(100) UNIQUE NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS students (
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    id UUID NOT NULL,
    branch_id UUID NOT NULL,
    admission_number VARCHAR(50) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_students PRIMARY KEY (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS invoices (
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    id UUID NOT NULL,
    branch_id UUID NOT NULL,
    student_id UUID NOT NULL,
    invoice_number VARCHAR(50) NOT NULL,
    total_amount NUMERIC(14, 2) NOT NULL,
    paid_amount NUMERIC(14, 2) NOT NULL DEFAULT 0.00,
    status VARCHAR(20) NOT NULL DEFAULT 'DRAFT',
    due_date DATE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_invoices PRIMARY KEY (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS payments (
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    id UUID NOT NULL,
    branch_id UUID NOT NULL,
    invoice_id UUID NOT NULL,
    amount NUMERIC(14, 2) NOT NULL,
    payment_method VARCHAR(30) NOT NULL,
    reference_number VARCHAR(100) NULL,
    idempotency_key UUID UNIQUE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_payments PRIMARY KEY (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS cbt_exam_sessions (
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    id UUID NOT NULL,
    exam_id UUID NOT NULL,
    student_id UUID NOT NULL,
    client_sync_id UUID UNIQUE NOT NULL,
    started_at TIMESTAMPTZ NOT NULL,
    submitted_at TIMESTAMPTZ NOT NULL,
    answers_json JSONB NOT NULL,
    score NUMERIC(6, 2) NULL,
    sync_status VARCHAR(20) NOT NULL DEFAULT 'SYNCED',
    CONSTRAINT pk_cbt_exam_sessions PRIMARY KEY (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS outbox_messages (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    event_type VARCHAR(255) NOT NULL,
    payload JSONB NOT NULL,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    processed_at TIMESTAMPTZ NULL,
    retry_count INT NOT NULL DEFAULT 0
);

-- 4. Enable Row-Level Security (RLS) on Tenant Tables
ALTER TABLE students ENABLE ROW LEVEL SECURITY;
ALTER TABLE invoices ENABLE ROW LEVEL SECURITY;
ALTER TABLE payments ENABLE ROW LEVEL SECURITY;
ALTER TABLE cbt_exam_sessions ENABLE ROW LEVEL SECURITY;

-- 5. Define Restrictive RLS Policies
DROP POLICY IF EXISTS tenant_isolation_students ON students;
CREATE POLICY tenant_isolation_students ON students
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS tenant_isolation_invoices ON invoices;
CREATE POLICY tenant_isolation_invoices ON invoices
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS tenant_isolation_payments ON payments;
CREATE POLICY tenant_isolation_payments ON payments
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS tenant_isolation_cbt_sessions ON cbt_exam_sessions;
CREATE POLICY tenant_isolation_cbt_sessions ON cbt_exam_sessions
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

-- Grant privileges to application user
GRANT ALL ON TABLE tenants TO fidel_app_user;
GRANT ALL ON TABLE students TO fidel_app_user;
GRANT ALL ON TABLE invoices TO fidel_app_user;
GRANT ALL ON TABLE payments TO fidel_app_user;
GRANT ALL ON TABLE cbt_exam_sessions TO fidel_app_user;
GRANT ALL ON TABLE outbox_messages TO fidel_app_user;
