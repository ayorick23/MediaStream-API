using Microsoft.AspNetCore.Identity;
using Proyecto.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            this._roleManager = roleManager;
        }

        public async Task<bool> CreateRole(string roleName)
        {
            if (!await RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                return result.Succeeded;
            }

            return false;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}
