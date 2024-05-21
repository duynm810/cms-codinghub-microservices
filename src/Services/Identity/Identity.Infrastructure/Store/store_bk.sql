USE [identity_service_db]
GO
/****** Object:  StoredProcedure [dbo].[Create_Permission]    Script Date: 5/21/2024 1:39:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Create_Permission]
	@roleId VARCHAR(50) NULL,
	@function VARCHAR(50) NULL,
	@command VARCHAR(50) NULL,
	@newID BIGINT OUTPUT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

BEGIN TRANSACTION;
BEGIN TRY
IF NOT EXISTS (
            SELECT *
            FROM [Identity].[Permissions]
            WHERE [RoleId] = @roleId
              AND [Function] = @function
              AND [Command] = @command
        )
BEGIN
INSERT INTO [Identity].[Permissions] ([RoleId], [Function], [Command])
VALUES (@roleId, @function, @command);

SET @newID = SCOPE_IDENTITY();
END;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = 'ERROR: ' + ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[Delete_Permission]    Script Date: 5/21/2024 1:39:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Delete_Permission]
    @roleId VARCHAR(50),
    @function VARCHAR(50),
    @command VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE
    FROM [Identity].[Permissions]
    WHERE [RoleId] = @roleId
      AND [Function] = @function
      AND [Command] = @command;
END
GO
/****** Object:  StoredProcedure [dbo].[Get_Permissions]    Script Date: 5/21/2024 1:39:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Permissions]
	@roleId VARCHAR(50) NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT *
    FROM [Identity].[Permissions]
    WHERE [RoleId] = @roleId;
END
GO
/****** Object:  StoredProcedure [dbo].[Update_Permissions]    Script Date: 5/21/2024 1:39:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Permissions]
	@roleId VARCHAR(50) NULL,
	@permissions Permission READONLY
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE FROM [Identity].[Permissions]
        WHERE [RoleId] = @roleId;

        INSERT INTO [Identity].[Permissions] ([RoleId], [Function], [Command])
        SELECT [RoleId], [Function], [Command]
        FROM @permissions;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = 'ERROR: ' + ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH;
END
GO
