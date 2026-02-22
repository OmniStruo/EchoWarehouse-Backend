-- Create users table for EchoWarehouse authentication
-- Migration: 001_CreateUsersTable.sql

-- Drop table if exists (for fresh setup only, remove in production)
-- DROP TABLE IF EXISTS users CASCADE;

CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(200) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    refresh_token VARCHAR(512),
    refresh_token_expiry_time TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    role VARCHAR(50) DEFAULT 'User',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);

-- Create indexes for faster queries
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_refresh_token ON users(refresh_token);
CREATE INDEX IF NOT EXISTS idx_users_is_active ON users(is_active);

-- Add comment to table
COMMENT ON TABLE users IS 'Users table for authentication and authorization';
COMMENT ON COLUMN users.id IS 'Unique user identifier';
COMMENT ON COLUMN users.username IS 'Unique username for login';
COMMENT ON COLUMN users.email IS 'Unique email address';
COMMENT ON COLUMN users.password_hash IS 'SHA256 hashed password';
COMMENT ON COLUMN users.refresh_token IS 'JWT refresh token for token rotation';
COMMENT ON COLUMN users.refresh_token_expiry_time IS 'When the refresh token expires (UTC)';
COMMENT ON COLUMN users.is_active IS 'Whether the user account is active';
COMMENT ON COLUMN users.role IS 'User role for authorization (e.g., Admin, User)';
COMMENT ON COLUMN users.created_at IS 'When the user was created (UTC)';
COMMENT ON COLUMN users.updated_at IS 'When the user was last updated (UTC)';

