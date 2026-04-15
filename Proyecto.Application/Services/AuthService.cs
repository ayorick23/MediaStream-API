using Proyecto.Domain.Entities;
using Proyecto.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;

        public AuthService(IUserRepository userRepository, IJWTService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }
        public async Task<Usuario> RegisterUser(Usuario usuario)
        {
            var result = await _userRepository.CreateUser(usuario);

            return result;
        }

        public async Task<string> Login(string email, string password, bool rememberMe)
        {
            var usuario = await _userRepository.GetUserByEmail(email);
            if (usuario == null)
            {
                return "Credenciales invalidas";
            }
            var credencialesValidas = await _userRepository.CheckPasswordAsync(usuario.Id.ToString(), password);

            if (!credencialesValidas)
            {
                return "Credenciales invalidas";
            }

            var roles = await _userRepository.GetUserRoles(email);

            return _jwtService.GenerateToken(usuario, roles);
        }
    }
}
