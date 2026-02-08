/* ==========================
   ROLES CRUD
========================== */

IF OBJECT_ID('[dbo].[spRoles_List]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spRoles_List] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spRoles_List]
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Id, Name, IsActive, CreatedAt
  FROM dbo.Roles
  ORDER BY Name;
END
GO

IF OBJECT_ID('[dbo].[spRoles_Create]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spRoles_Create] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spRoles_Create]
  @Name NVARCHAR(50)
AS
BEGIN
  SET NOCOUNT ON;

  IF EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = @Name)
    THROW 50001, 'Role already exists', 1;

  INSERT INTO dbo.Roles(Name) VALUES (@Name);
  SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

IF OBJECT_ID('[dbo].[spRoles_Update]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spRoles_Update] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spRoles_Update]
  @Id INT,
  @Name NVARCHAR(50),
  @IsActive BIT
AS
BEGIN
  SET NOCOUNT ON;

  IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id=@Id)
    THROW 50002, 'Role not found', 1;

  IF EXISTS (SELECT 1 FROM dbo.Roles WHERE Name=@Name AND Id<>@Id)
    THROW 50003, 'Role name already used', 1;

  UPDATE dbo.Roles
  SET Name=@Name, IsActive=@IsActive
  WHERE Id=@Id;

  SELECT @Id AS UpdatedId;
END
GO


/* ==========================
   USER <-> ROLES
========================== */

IF OBJECT_ID('[dbo].[spUserRoles_GetByUser]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spUserRoles_GetByUser] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spUserRoles_GetByUser]
  @UsuarioId INT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT r.Id, r.Name
  FROM dbo.UsuarioRoles ur
  JOIN dbo.Roles r ON r.Id = ur.RoleId
  WHERE ur.UsuarioId = @UsuarioId
  ORDER BY r.Name;
END
GO

IF OBJECT_ID('[dbo].[spUserRoles_Set]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spUserRoles_Set] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spUserRoles_Set]
  @UsuarioId INT,
  @RoleIds dbo.IntList READONLY
AS
BEGIN
  SET NOCOUNT ON;

  -- Reemplaza roles: delete + insert (simple y consistente)
  DELETE FROM dbo.UsuarioRoles WHERE UsuarioId=@UsuarioId;

  INSERT INTO dbo.UsuarioRoles(UsuarioId, RoleId)
  SELECT @UsuarioId, Id FROM @RoleIds;
END
GO


/* ==========================
   PERMISSIONS
========================== */

IF OBJECT_ID('[dbo].[spPermisos_List]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spPermisos_List] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spPermisos_List]
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Id, Code, Description, IsActive
  FROM dbo.Permisos
  ORDER BY Code;
END
GO

IF OBJECT_ID('[dbo].[spRolePermisos_Get]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spRolePermisos_Get] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spRolePermisos_Get]
  @RoleId INT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT p.Id, p.Code, p.Description
  FROM dbo.RolPermisos rp
  JOIN dbo.Permisos p ON p.Id = rp.PermisoId
  WHERE rp.RoleId=@RoleId
  ORDER BY p.Code;
END
GO

IF OBJECT_ID('[dbo].[spRolePermisos_Set]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spRolePermisos_Set] AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE [dbo].[spRolePermisos_Set]
  @RoleId INT,
  @PermisoIds dbo.IntList READONLY
AS
BEGIN
  SET NOCOUNT ON;

  DELETE FROM dbo.RolPermisos WHERE RoleId=@RoleId;

  INSERT INTO dbo.RolPermisos(RoleId, PermisoId)
  SELECT @RoleId, Id FROM @PermisoIds;
END
GO
