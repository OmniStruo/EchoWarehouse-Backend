-- Seed initial users for development/testing
-- Migration: 002_SeedInitialUsers.sql

INSERT INTO users (username, email, password_hash, is_active, role, created_at)
VALUES 
    -- Password: admin123 (SHA256 hashed)
    ('admin', 'admin@echowarehouse.com', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', TRUE, 'Admin', CURRENT_TIMESTAMP),
    -- Password: user123 (SHA256 hashed)
    ('user', 'user@echowarehouse.com', '602f48ac77fc0bf84d37f0ef62aacc88c17829cebc2d5e0b8ef4b76e0a78be4d', TRUE, 'User', CURRENT_TIMESTAMP)
ON CONFLICT (username) DO NOTHING;

