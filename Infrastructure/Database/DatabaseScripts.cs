namespace InvenSmartApi.Infrastructure.Database;

public sealed record DbScript(string Name, string Sql);

public static class DatabaseScripts
{
    public static IReadOnlyList<DbScript> GetAll() => new List<DbScript>
    {
        new("Tables", TablesSql),
        new("StoredProcedures", StoredProceduresSql)
    };

    // Ajusta tamaños/nombres si tu DB real ya existe con otro diseño.
    private const string TablesSql = @"
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
        PermissionId INT NOT NULL,
        Comment NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Usuarios_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Usuarios_CreatedAt DEFAULT (SYSUTCDATETIME())
    );

    CREATE UNIQUE INDEX UX_Usuarios_UserId ON dbo.Usuarios(UserId);
END
GO

IF OBJECT_ID(N'dbo.Formulario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Formulario
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Formulario PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL
    );

    CREATE UNIQUE INDEX UX_Formulario_Name ON dbo.Formulario(Name);
END
GO

IF OBJECT_ID(N'dbo.Permiso', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permiso
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Permiso PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL
    );

    CREATE UNIQUE INDEX UX_Permiso_Name ON dbo.Permiso(Name);
END
GO

IF OBJECT_ID(N'dbo.UsuarioPermiso', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioPermiso
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UsuarioPermiso PRIMARY KEY,
        UserId INT NOT NULL,
        FormularioId INT NOT NULL,
        PermisoId INT NOT NULL,
        CONSTRAINT FK_UsuarioPermiso_Usuarios FOREIGN KEY (UserId) REFERENCES dbo.Usuarios(Id),
        CONSTRAINT FK_UsuarioPermiso_Formulario FOREIGN KEY (FormularioId) REFERENCES dbo.Formulario(Id),
        CONSTRAINT FK_UsuarioPermiso_Permiso FOREIGN KEY (PermisoId) REFERENCES dbo.Permiso(Id)
    );

    CREATE INDEX IX_UsuarioPermiso_UserId ON dbo.UsuarioPermiso(UserId);
END
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
        Comment
    FROM dbo.Usuarios
    WHERE UserId = @UserId
      AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertarUsuario
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @UserId NVARCHAR(100),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX),
    @Cedula NVARCHAR(50) = NULL,
    @PermissionId INT,
    @Comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Usuarios
    (
        Nombre, Apellido, UserId,
        PasswordHash, PasswordSalt,
        Cedula, PermissionId, Comment
    )
    VALUES
    (
        @Nombre, @Apellido, @UserId,
        @PasswordHash, @PasswordSalt,
        @Cedula, @PermissionId, @Comment
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.spGetPermisosByUsuario
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        f.Name AS FormularioName,
        p.Name AS PermisoName
    FROM dbo.UsuarioPermiso up
    INNER JOIN dbo.Formulario f ON f.Id = up.FormularioId
    INNER JOIN dbo.Permiso p ON p.Id = up.PermisoId
    WHERE up.UserId = @UserId;
END
GO
";
}
