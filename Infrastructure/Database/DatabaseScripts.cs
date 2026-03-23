namespace InvenSmartApi.Infrastructure.Database;

public sealed record DbScript(string Name, string Sql);

public static class DatabaseScripts
{
    public static IReadOnlyList<DbScript> GetAll() => new List<DbScript>
    {
        new("SchemaAndSecurity", SchemaAndSecuritySql),
        new("StoredProcedures", StoredProceduresSql)
    };

    private const string SchemaAndSecuritySql = @"
IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Apellido NVARCHAR(100) NOT NULL,
        UserId NVARCHAR(100) NOT NULL,
        PasswordHash VARBINARY(MAX) NOT NULL,
        PasswordSalt VARBINARY(MAX) NOT NULL,
        Cedula NVARCHAR(50) NULL,
        PermissionId INT NULL,
        Comment NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Usuarios_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Usuarios_CreatedAt DEFAULT (SYSUTCDATETIME())
    );

    CREATE UNIQUE INDEX UX_Usuarios_UserId ON dbo.Usuarios(UserId);
END
GO

IF COL_LENGTH('dbo.Usuarios', 'PermissionId') IS NOT NULL
AND EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Usuarios')
      AND name = 'PermissionId'
      AND is_nullable = 0
)
BEGIN
    ALTER TABLE dbo.Usuarios ALTER COLUMN PermissionId INT NULL;
END
GO

IF COL_LENGTH('dbo.Usuarios', 'IsActive') IS NULL
BEGIN
    ALTER TABLE dbo.Usuarios
    ADD IsActive BIT NOT NULL CONSTRAINT DF_Usuarios_IsActive_2 DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.Usuarios', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Usuarios
    ADD CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Usuarios_CreatedAt_2 DEFAULT (SYSUTCDATETIME());
END
GO

IF OBJECT_ID(N'dbo.ErrorLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ErrorLog
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ErrorLog PRIMARY KEY,
        ErrorMessage NVARCHAR(4000) NOT NULL,
        StackTrace NVARCHAR(MAX) NULL,
        ClassName NVARCHAR(300) NOT NULL,
        MethodName NVARCHAR(300) NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ErrorLog_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Roles_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE UNIQUE INDEX UX_Roles_Name ON dbo.Roles(Name);
END
GO

IF OBJECT_ID(N'dbo.Permisos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permisos
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Permisos PRIMARY KEY,
        Code NVARCHAR(150) NOT NULL,
        Description NVARCHAR(250) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Permisos_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Permisos_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
    CREATE UNIQUE INDEX UX_Permisos_Code ON dbo.Permisos(Code);
END
GO

IF OBJECT_ID(N'dbo.UsuarioRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioRoles
    (
        UsuarioId INT NOT NULL,
        RoleId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_UsuarioRoles_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UsuarioRoles PRIMARY KEY (UsuarioId, RoleId),
        CONSTRAINT FK_UsuarioRoles_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
        CONSTRAINT FK_UsuarioRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.RolPermisos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RolPermisos
    (
        RoleId INT NOT NULL,
        PermisoId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_RolPermisos_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_RolPermisos PRIMARY KEY (RoleId, PermisoId),
        CONSTRAINT FK_RolPermisos_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
        CONSTRAINT FK_RolPermisos_Permisos FOREIGN KEY (PermisoId) REFERENCES dbo.Permisos(Id)
    );
END
GO

IF TYPE_ID(N'dbo.IntList') IS NULL
    EXEC('CREATE TYPE dbo.IntList AS TABLE (Id INT NOT NULL)');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'Admin')
    INSERT INTO dbo.Roles(Name) VALUES ('Admin');
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'Operator')
    INSERT INTO dbo.Roles(Name) VALUES ('Operator');
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'Viewer')
    INSERT INTO dbo.Roles(Name) VALUES ('Viewer');
GO

DECLARE @PermSeed TABLE(Code NVARCHAR(150), Description NVARCHAR(250));
INSERT INTO @PermSeed(Code, Description)
VALUES
('SCREEN.DASHBOARD.VIEW', 'View dashboard'),
('SCREEN.PRODUCTS.VIEW', 'View products screen'),
('SCREEN.INVENTORY.VIEW', 'View inventory screen'),
('SCREEN.SUPPLIERS.VIEW', 'View suppliers screen'),
('SCREEN.USERS.VIEW', 'View users screen'),
('SCREEN.ROLES.VIEW', 'View roles screen'),
('PRODUCT.CREATE', 'Create product'),
('PRODUCT.EDIT', 'Edit product'),
('PRODUCT.DELETE', 'Delete product'),
('INVENTORY.CREATE', 'Create inventory entry'),
('INVENTORY.EDIT', 'Edit inventory entry'),
('INVENTORY.DELETE', 'Delete inventory entry'),
('SUPPLIER.CREATE', 'Create supplier'),
('SUPPLIER.EDIT', 'Edit supplier'),
('SUPPLIER.DELETE', 'Delete supplier'),
('USER.CREATE', 'Create user'),
('USER.EDIT', 'Edit user'),
('USER.DELETE', 'Delete user'),
('ROLE.CREATE', 'Create role'),
('ROLE.EDIT', 'Edit role'),
('ROLE.DELETE', 'Delete role');

INSERT INTO dbo.Permisos(Code, Description)
SELECT s.Code, s.Description
FROM @PermSeed s
WHERE NOT EXISTS (SELECT 1 FROM dbo.Permisos p WHERE p.Code = s.Code);
GO
";

    private const string StoredProceduresSql = @"
CREATE OR ALTER PROCEDURE dbo.sp_GetUsuarioByUserId
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        Id,
        Nombre,
        Apellido,
        UserId,
        PasswordHash,
        PasswordSalt,
        Cedula,
        PermissionId,
        Comment,
        IsActive
    FROM dbo.Usuarios
    WHERE UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertarUsuario
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @UserId NVARCHAR(100),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX),
    @Cedula NVARCHAR(50) = NULL,
    @PermissionId INT = NULL,
    @Comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Usuarios
    (
        Nombre, Apellido, UserId,
        PasswordHash, PasswordSalt,
        Cedula, PermissionId, Comment, IsActive
    )
    VALUES
    (
        @Nombre, @Apellido, @UserId,
        @PasswordHash, @PasswordSalt,
        @Cedula, @PermissionId, @Comment, 1
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE dbo.spInsertErrorLog
    @ErrorMessage NVARCHAR(4000),
    @StackTrace NVARCHAR(MAX) = NULL,
    @ClassName NVARCHAR(300),
    @MethodName NVARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ErrorLog(ErrorMessage, StackTrace, ClassName, MethodName)
    VALUES (@ErrorMessage, @StackTrace, @ClassName, @MethodName);
END
GO

CREATE OR ALTER PROCEDURE dbo.spGetRolesByUsuario
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Name
    FROM dbo.UsuarioRoles ur
    INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
    WHERE ur.UsuarioId = @UserId
      AND r.IsActive = 1
    ORDER BY r.Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.spGetPermisosByUsuario
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        CASE 
            WHEN CHARINDEX('.', p.Code) > 0 THEN LEFT(p.Code, CHARINDEX('.', p.Code) - 1)
            ELSE p.Code
        END AS FormularioName,
        CASE 
            WHEN CHARINDEX('.', p.Code) > 0 THEN SUBSTRING(p.Code, CHARINDEX('.', p.Code) + 1, LEN(p.Code))
            ELSE 'VIEW'
        END AS PermisoName
    FROM dbo.UsuarioRoles ur
    INNER JOIN dbo.RolPermisos rp ON rp.RoleId = ur.RoleId
    INNER JOIN dbo.Permisos p ON p.Id = rp.PermisoId
    WHERE ur.UsuarioId = @UserId
      AND p.IsActive = 1
    ORDER BY FormularioName, PermisoName;
END
GO

CREATE OR ALTER PROCEDURE dbo.spRoles_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, IsActive, CreatedAt
    FROM dbo.Roles
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.spRoles_Create
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = @Name)
        THROW 50001, 'Role already exists', 1;

    INSERT INTO dbo.Roles(Name) VALUES (@Name);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END
GO

CREATE OR ALTER PROCEDURE dbo.spRoles_Update
    @Id INT,
    @Name NVARCHAR(100),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @Id)
        THROW 50002, 'Role not found', 1;

    IF EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = @Name AND Id <> @Id)
        THROW 50003, 'Role name already used', 1;

    UPDATE dbo.Roles
    SET Name = @Name,
        IsActive = @IsActive
    WHERE Id = @Id;

    SELECT @Id AS UpdatedId;
END
GO

CREATE OR ALTER PROCEDURE dbo.spUserRoles_GetByUser
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id, r.Name, r.IsActive
    FROM dbo.UsuarioRoles ur
    INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
    WHERE ur.UsuarioId = @UsuarioId
    ORDER BY r.Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.spUserRoles_Set
    @UsuarioId INT,
    @RoleIds dbo.IntList READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.UsuarioRoles WHERE UsuarioId = @UsuarioId;

    INSERT INTO dbo.UsuarioRoles(UsuarioId, RoleId)
    SELECT @UsuarioId, rl.Id
    FROM @RoleIds rl;
END
GO

CREATE OR ALTER PROCEDURE dbo.spPermisos_List
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Code, Description, IsActive
    FROM dbo.Permisos
    ORDER BY Code;
END
GO

CREATE OR ALTER PROCEDURE dbo.spRolePermisos_Get
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.Id, p.Code, p.Description, p.IsActive
    FROM dbo.RolPermisos rp
    INNER JOIN dbo.Permisos p ON p.Id = rp.PermisoId
    WHERE rp.RoleId = @RoleId
    ORDER BY p.Code;
END
GO

CREATE OR ALTER PROCEDURE dbo.spRolePermisos_Set
    @RoleId INT,
    @PermisoIds dbo.IntList READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.RolPermisos WHERE RoleId = @RoleId;

    INSERT INTO dbo.RolPermisos(RoleId, PermisoId)
    SELECT @RoleId, p.Id
    FROM @PermisoIds p;
END
GO
";
}
