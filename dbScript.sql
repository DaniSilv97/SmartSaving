USE [dbSmartSaving]
GO
/****** Object:  User [userSmartSaving]    Script Date: 27/06/2025 17:47:27 ******/
CREATE USER [userSmartSaving] FOR LOGIN [userSmartSaving] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [userSmartSaving]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [userSmartSaving]
GO
ALTER ROLE [db_datareader] ADD MEMBER [userSmartSaving]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [userSmartSaving]
GO
/****** Object:  Table [dbo].[tRole]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tRole](
	[idRole] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_tRoles] PRIMARY KEY CLUSTERED 
(
	[idRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tTracker]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tTracker](
	[guidTracker] [uniqueidentifier] NOT NULL,
	[date] [date] NOT NULL,
	[guidUser] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tTrackers] PRIMARY KEY CLUSTERED 
(
	[guidTracker] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tTransaction]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tTransaction](
	[guidTransaction] [uniqueidentifier] NOT NULL,
	[value] [decimal](18, 3) NOT NULL,
	[idTransactionType] [tinyint] NOT NULL,
	[guidTracker] [uniqueidentifier] NOT NULL,
	[date] [datetime] NOT NULL,
	[description] [varchar](255) NOT NULL,
 CONSTRAINT [PK_tTransactions] PRIMARY KEY CLUSTERED 
(
	[guidTransaction] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tTransactionType]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tTransactionType](
	[idTransactionType] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](10) NOT NULL,
 CONSTRAINT [PK_tTransactionType] PRIMARY KEY CLUSTERED 
(
	[idTransactionType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tUser]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tUser](
	[guidUser] [uniqueidentifier] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[email] [varchar](100) NOT NULL,
	[password] [varchar](100) NOT NULL,
	[idRole] [tinyint] NOT NULL,
 CONSTRAINT [PK_tUser] PRIMARY KEY CLUSTERED 
(
	[guidUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tTracker] ADD  CONSTRAINT [DF_tTrackers_guidTracker]  DEFAULT (newid()) FOR [guidTracker]
GO
ALTER TABLE [dbo].[tTracker] ADD  CONSTRAINT [DF_tTrackers_date]  DEFAULT (getdate()) FOR [date]
GO
ALTER TABLE [dbo].[tTransaction] ADD  CONSTRAINT [DF_tTransactions_guidTransaction]  DEFAULT (newid()) FOR [guidTransaction]
GO
ALTER TABLE [dbo].[tTransaction] ADD  CONSTRAINT [DF_tTransactions_value]  DEFAULT ((0)) FOR [value]
GO
ALTER TABLE [dbo].[tTransaction] ADD  CONSTRAINT [DF_tTransactions_date]  DEFAULT (getdate()) FOR [date]
GO
ALTER TABLE [dbo].[tTransaction] ADD  CONSTRAINT [DF_tTransaction_description]  DEFAULT ('') FOR [description]
GO
ALTER TABLE [dbo].[tUser] ADD  CONSTRAINT [DF_tUser_guidUser]  DEFAULT (newid()) FOR [guidUser]
GO
ALTER TABLE [dbo].[tTracker]  WITH CHECK ADD  CONSTRAINT [FK_tTrackers_tUser] FOREIGN KEY([guidUser])
REFERENCES [dbo].[tUser] ([guidUser])
GO
ALTER TABLE [dbo].[tTracker] CHECK CONSTRAINT [FK_tTrackers_tUser]
GO
ALTER TABLE [dbo].[tTransaction]  WITH CHECK ADD  CONSTRAINT [FK_tTransactions_tTrackers] FOREIGN KEY([guidTracker])
REFERENCES [dbo].[tTracker] ([guidTracker])
GO
ALTER TABLE [dbo].[tTransaction] CHECK CONSTRAINT [FK_tTransactions_tTrackers]
GO
ALTER TABLE [dbo].[tTransaction]  WITH CHECK ADD  CONSTRAINT [FK_tTransactions_tTransactionType] FOREIGN KEY([idTransactionType])
REFERENCES [dbo].[tTransactionType] ([idTransactionType])
GO
ALTER TABLE [dbo].[tTransaction] CHECK CONSTRAINT [FK_tTransactions_tTransactionType]
GO
ALTER TABLE [dbo].[tUser]  WITH CHECK ADD  CONSTRAINT [FK_tUser_tRole] FOREIGN KEY([idRole])
REFERENCES [dbo].[tRole] ([idRole])
GO
ALTER TABLE [dbo].[tUser] CHECK CONSTRAINT [FK_tUser_tRole]
GO
/****** Object:  StoredProcedure [dbo].[QTracker_Create]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTracker_Create - Create new tracker
-- =============================================
CREATE PROCEDURE [dbo].[QTracker_Create]
    @Date DATE,
    @GuidUser UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO tTracker ([date], guidUser)
    VALUES (@Date, @GuidUser);
END
GO
/****** Object:  StoredProcedure [dbo].[QTracker_Delete]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTracker_Delete - Delete tracker
-- =============================================
CREATE PROCEDURE [dbo].[QTracker_Delete]
    @Guid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Delete related transactions first (if cascade is not set)
    DELETE FROM tTransaction WHERE guidTracker = @Guid;
    
    -- Delete the tracker
    DELETE FROM tTracker WHERE guidTracker = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QTracker_Get]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTracker_Get - Get tracker by GUID
-- =============================================
CREATE PROCEDURE [dbo].[QTracker_Get]
    @Guid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        guidTracker,
        [date],
        guidUser
    FROM tTracker
    WHERE guidTracker = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QTracker_GetCount]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTracker_GetCount - Get total count of trackers
-- =============================================
CREATE PROCEDURE [dbo].[QTracker_GetCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM tTracker;
END
GO
/****** Object:  StoredProcedure [dbo].[QTracker_GetCountByUser]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTracker_GetCountByUser - Get count of trackers by user
-- =============================================
CREATE PROCEDURE [dbo].[QTracker_GetCountByUser]
    @GuidUser UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) 
    FROM tTracker 
    WHERE guidUser = @GuidUser;
END
GO
/****** Object:  StoredProcedure [dbo].[QTracker_List]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[QTracker_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        guidTracker,
        [date],
        guidUser
    FROM tTracker
    ORDER BY [date] DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[QTracker_ListByUser]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTracker_ListByUser - List trackers by user
-- =============================================
CREATE PROCEDURE [dbo].[QTracker_ListByUser]
    @GuidUser UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        guidTracker,
        [date],
        guidUser
    FROM tTracker
    WHERE guidUser = @GuidUser
    ORDER BY [date] DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[QTracker_Update]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTracker_Update - Update existing tracker
-- =============================================
CREATE PROCEDURE [dbo].[QTracker_Update]
    @Guid UNIQUEIDENTIFIER,
    @Date DATE,
    @GuidUser UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE tTracker
    SET [date] = @Date,
        guidUser = @GuidUser
    WHERE guidTracker = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_Create]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTransaction_Create - Create new transaction
-- =============================================
CREATE PROCEDURE [dbo].[QTransaction_Create]
    @Value DECIMAL(18,3),
    @IdTransactionType TINYINT,
    @GuidTracker UNIQUEIDENTIFIER,
    @Date DATETIME,
    @Description VARCHAR(255) = ''
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO tTransaction ([value], idTransactionType, guidTracker, [date], [description])
    VALUES (@Value, @IdTransactionType, @GuidTracker, @Date, @Description);
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_Delete]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[QTransaction_Delete]
    @Guid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM tTransaction WHERE guidTransaction = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_Get]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTransaction_Get - Get transaction by GUID
-- =============================================
CREATE PROCEDURE [dbo].[QTransaction_Get]
    @Guid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        guidTransaction,
        [value],
        idTransactionType,
        guidTracker,
        [date],
        [description]
    FROM tTransaction
    WHERE guidTransaction = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_GetCount]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[QTransaction_GetCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM tTransaction;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_GetCountByTracker]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[QTransaction_GetCountByTracker]
    @GuidTracker UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) 
    FROM tTransaction 
    WHERE guidTracker = @GuidTracker;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_GetTotalValueByTracker]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[QTransaction_GetTotalValueByTracker]
    @GuidTracker UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ISNULL(SUM([value]), 0) 
    FROM tTransaction 
    WHERE guidTracker = @GuidTracker;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_GetTotalValueByTrackerAndType]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[QTransaction_GetTotalValueByTrackerAndType]
    @GuidTracker UNIQUEIDENTIFIER,
    @IdTransactionType TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ISNULL(SUM([value]), 0) 
    FROM tTransaction 
    WHERE guidTracker = @GuidTracker 
    AND idTransactionType = @IdTransactionType;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_GetTotalValueByType]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[QTransaction_GetTotalValueByType]
    @IdTransactionType TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ISNULL(SUM([value]), 0) 
    FROM tTransaction 
    WHERE idTransactionType = @IdTransactionType;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_List]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTransaction_List - List all transactions
-- =============================================
CREATE PROCEDURE [dbo].[QTransaction_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        guidTransaction,
        [value],
        idTransactionType,
        guidTracker,
        [date],
        [description]
    FROM tTransaction
    ORDER BY [date] DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_ListByTracker]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTransaction_ListByTracker - List transactions by tracker
-- =============================================
CREATE PROCEDURE [dbo].[QTransaction_ListByTracker]
    @GuidTracker UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        guidTransaction,
        [value],
        idTransactionType,
        guidTracker,
        [date],
        [description]
    FROM tTransaction
    WHERE guidTracker = @GuidTracker
    ORDER BY [date] DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_ListByType]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTransaction_ListByType - List transactions by type
-- =============================================
CREATE PROCEDURE [dbo].[QTransaction_ListByType]
    @IdTransactionType TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        guidTransaction,
        [value],
        idTransactionType,
        guidTracker,
        [date],
        [description]
    FROM tTransaction
    WHERE idTransactionType = @IdTransactionType
    ORDER BY [date] DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[QTransaction_Update]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- QTransaction_Update - Update existing transaction
-- =============================================
CREATE PROCEDURE [dbo].[QTransaction_Update]
    @Guid UNIQUEIDENTIFIER,
    @Value DECIMAL(18,3),
    @IdTransactionType TINYINT,
    @GuidTracker UNIQUEIDENTIFIER,
    @Date DATETIME,
    @Description VARCHAR(255) = ''
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE tTransaction
    SET [value] = @Value,
        idTransactionType = @IdTransactionType,
        guidTracker = @GuidTracker,
        [date] = @Date,
        [description] = @Description
    WHERE guidTransaction = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QUser_Create]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QUser_Create]
    @Name VARCHAR(100),
    @Email VARCHAR(100),
    @Password VARCHAR(100),
    @IdRole TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO tUser (name, email, password, idRole)
    VALUES (@Name, @Email, @Password, @IdRole);
END
GO
/****** Object:  StoredProcedure [dbo].[QUser_Delete]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QUser_Delete]
    @Guid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM tUser
    WHERE guidUser = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QUser_Get]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QUser_Get]
    @Guid UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT guidUser, name, email, password, idRole
    FROM tUser
    WHERE guidUser = @Guid;
END
GO
/****** Object:  StoredProcedure [dbo].[QUser_GetByEmail]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QUser_GetByEmail]
    @Email VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT guidUser, name, email, password, idRole
    FROM tUser
    WHERE email = @Email;
END
GO
/****** Object:  StoredProcedure [dbo].[QUser_GetCount]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QUser_GetCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) FROM tUser;
END
GO
/****** Object:  StoredProcedure [dbo].[QUser_List]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QUser_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT guidUser, name, email, idRole
    FROM tUser
    ORDER BY name;
END
GO
/****** Object:  StoredProcedure [dbo].[QUser_Update]    Script Date: 27/06/2025 17:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QUser_Update]
    @Guid UNIQUEIDENTIFIER,
    @Name VARCHAR(100),
    @Email VARCHAR(100),
    @Password VARCHAR(100),
    @IdRole TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE tUser
    SET name = @Name,
        email = @Email,
        password = @Password,
        idRole = @IdRole
    WHERE guidUser = @Guid;
END
GO
