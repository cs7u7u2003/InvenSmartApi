1. Abre InvenSmartApi.sln en Visual Studio 2022.
2. Restaura paquetes NuGet.
3. Verifica que SQL Server acepte SQL Authentication.
4. Crea el login/db inicial una sola vez con un usuario admin si tu usuario de app no puede crear DB.
5. Ejecuta el perfil: InvenSmartApi (https).
6. Swagger abrirá en: https://localhost:7189/swagger
7. Login por defecto:
   usuario: admin
   password: P@ssw0rd!

Notas:
- El initializer crea tablas, SPs, rol Admin, permisos y asigna todos los permisos al usuario admin.
- Si la base ya existe, el initializer aplica cambios idempotentes.
