using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservasColegio.Context;
using ReservasColegio.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReservasColegio.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        public ClaimsPrincipal GerarClaimsPrincipal(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Diretiva?.Nome ?? "Funcionario")
            };

            foreach (var perm in usuario.Diretiva?.DiretivaPermissoes ?? new List<DiretivaPermissao>())
            {
                claims.Add(new Claim("permissao", perm.Permissao.Nome));
            }

            var identity = new ClaimsIdentity(claims, "login");
            return new ClaimsPrincipal(identity);
        }



        public string GerarToken(Usuario usuario)
        {
            var usuarioCompleto = _context.Usuarios
                .Include(u => u.Diretiva)
                    .ThenInclude(d => d.DiretivaPermissoes)
                        .ThenInclude(p => p.Permissao)
                .FirstOrDefault(u => u.Id == usuario.Id);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            foreach (var diretiva in usuarioCompleto.Diretiva.DiretivaPermissoes)
            {
                claims.Add(new Claim(ClaimTypes.Role, diretiva.Permissao.Nome));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
