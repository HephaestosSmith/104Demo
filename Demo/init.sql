-- Demo SQL 初始化腳本：建立 DemoDb 與 Users 表
-- 建議：實務上請儲存密碼雜湊值而非明文密碼

IF DB_ID(N'DemoDb') IS NULL
BEGIN
    CREATE DATABASE [DemoDb];
END
GO

USE [DemoDb];
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, -- 唯一識別
        Account NVARCHAR(100) NOT NULL,            -- 帳號（建議唯一）
        PasswordHash VARBINARY(256) NOT NULL,      -- 密碼雜湊值（非明文）
        FullName NVARCHAR(200) NULL,               -- 姓名
        Phone NVARCHAR(256) NULL,                   -- 電話號碼
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );

    -- 加上帳號唯一索引
    CREATE UNIQUE INDEX IX_Users_Account ON dbo.Users(Account);
END
GO