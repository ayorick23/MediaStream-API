using Microsoft.AspNetCore.Identity;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Interfaces;
using Proyecto.Infrastructure.Identity;
using Proyecto.Infrastructure.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly UserManager<AppIdentityUser> _userManager;

        public UserRepository(UserManager<AppIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Usuario> AddToRoleAsync(Usuario usuario, string roleName)
        {
            var userDb = await _userManager.FindByEmailAsync(usuario.Email);
            var result = await _userManager.AddToRoleAsync(userDb, roleName);
            return usuario;
        }

        public async Task<Usuario> CreateUser(Usuario usuario)
        {
            var appIdentityUser = usuario.ToAppIdentityUser();
            var result = await _userManager.CreateAsync(appIdentityUser, usuario.Password);

            if (result.Succeeded)
            {
                var newUser = await _userManager.FindByEmailAsync(usuario.Email);
                usuario.Id = new Guid(newUser.Id);

                return usuario;
            }

            return null;
        }

        public async Task<Usuario> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user?.ToDomainUser();
        }

        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<List<string>> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }
    }
}
