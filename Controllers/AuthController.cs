using Dapper;
using InvenSmartApi.Infrastructure.Security;
using InvenSmartApi.Models;
using InvenSmartApi.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InvenSmartApi.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IDbConnection _db;
    private readonly IConfiguration _cfg;

    public AuthController(IDbConnection db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] Credenciales req)
    {
        if (req is null) return BadRequest("Body requerido.");
        if (string.IsNullOrWhiteSpace(req.Usuario) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Usuario y Password son requeridos.");

        // 1) Traer usuario
        var user = await _db.QueryFirstOrDefaultAsync<UsuarioDto>(
            "[dbo].[sp_GetUsuarioByUserId]",
            new { UserId = req.Usuario.Trim() },
            commandType: CommandType.StoredProcedure);

        if (user is null) return Unauthorized("Credenciales inválidas.");

        // 2) Verificar password
        var ok = LegacyPasswordHasher.VerifyPasswordHash(req.Password, user.PasswordHash, user.PasswordSalt);

        if (!ok) return Unauthorized("Credenciales inválidas.");

        // 3) Roles (ajusta según tus tablas/SP reales)
        // Recomendado: SP que devuelva lista de roles del usuario.
        // Si todavía no lo tienes, te dejo un SP al final.
        var roles = (await _db.QueryAsync<string>(
            "[dbo].[spGetRolesByUsuario]",
            new { UserId = user.Id },
            commandType: CommandType.StoredProcedure)).ToArray();

        // 4) Permisos (tu PermisoRepository ya llama spGetPermisosByUsuario)
        var permisosRaw = (await _db.QueryAsync<PermisoFormularioDto>(
            "[dbo].[spGetPermisosByUsuario]",
            new { UserId = user.Id },
            commandType: CommandType.StoredProcedure)).ToList();

        // Convertir a "FORMULARIO.PERMISO"
        var perms = permisosRaw
            .Where(x => !string.IsNullOrWhiteSpace(x.FormularioName) && !string.IsNullOrWhiteSpace(x.PermisoName))
            .Select(x => $"{x.FormularioName}.{x.PermisoName}".ToUpperInvariant())
            .Distinct()
            .ToArray();

        // 5) Crear token
        var (token, expiresAtUtc) = CreateJwtToken(user, roles, perms);

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            UserName = user.UserId,
            Roles = roles,
            Permissions = perms
        });
    }

    private (string token, DateTime expiresAtUtc) CreateJwtToken(UsuarioDto user, string[] roles, string[] perms)
    {
        var key = _cfg["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");
        var issuer = _cfg["Jwt:Issuer"];
        var audience = _cfg["Jwt:Audience"];
        var expiresMinutes = int.TryParse(_cfg["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("sub", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserId ?? "")
        };

        foreach (var r in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            claims.Add(new Claim(ClaimTypes.Role, r));

        foreach (var p in perms.Where(p => !string.IsNullOrWhiteSpace(p)))
            claims.Add(new Claim("perm", p));

        var jwt = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
            audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: creds);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expiresAtUtc);
    }
}
