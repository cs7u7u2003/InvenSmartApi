IF OBJECT_ID('[dbo].[spGetRolesByUsuario]','P') IS NULL
EXEC('CREATE PROCEDURE [dbo].[spGetRolesByUsuario] AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE [dbo].[spGetRolesByUsuario]
  @UserId INT
AS
BEGIN
  SET NOCOUNT ON;

  -- TODO: reemplazar cuando tengas tablas reales UsuarioRoles/Roles
  -- Placeholder: retorna "ADMIN" a todos, para probar JWT en Swagger
  SELECT 'ADMIN' AS RoleName;
END
GO
