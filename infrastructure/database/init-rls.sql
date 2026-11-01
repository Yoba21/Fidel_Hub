-- ==============================================================================
-- Fidel Hub: Multi-Tenant Row Level Security (RLS) Initialization
-- ==============================================================================

-- 1. Create a dedicated application role that is NOT a superuser
-- Superusers bypass RLS policies; the application MUST connect using this role.
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

-- 3. Core Multi-Tenant Tables Schema Example
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
    admission_number VARCHAR(50) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_students PRIMARY KEY (tenant_id, id)
);

CREATE TABLE IF NOT EXISTS journal_entries (
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    id UUID NOT NULL,
    voucher_number VARCHAR(100) NOT NULL,
    transaction_date DATE NOT NULL,
    account_code VARCHAR(50) NOT NULL,
    debit_amount NUMERIC(18, 4) NOT NULL DEFAULT 0.0000,
    credit_amount NUMERIC(18, 4) NOT NULL DEFAULT 0.0000,
    memo TEXT,
    idempotency_key UUID UNIQUE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_journal_entries PRIMARY KEY (tenant_id, id)
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
ALTER TABLE journal_entries ENABLE ROW LEVEL SECURITY;

-- 5. Define Restrictive RLS Policies
DROP POLICY IF EXISTS tenant_isolation_students ON students;
CREATE POLICY tenant_isolation_students ON students
    FOR ALL
    TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

DROP POLICY IF EXISTS tenant_isolation_journal_entries ON journal_entries;
CREATE POLICY tenant_isolation_journal_entries ON journal_entries
    FOR ALL
    TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

-- Grant privileges to application user
GRANT ALL ON TABLE tenants TO fidel_app_user;
GRANT ALL ON TABLE students TO fidel_app_user;
GRANT ALL ON TABLE journal_entries TO fidel_app_user;
GRANT ALL ON TABLE outbox_messages TO fidel_app_user;
