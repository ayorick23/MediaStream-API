using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.Domain.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Para revisar si un rol existe
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> RoleExistsAsync(string roleName);

        /// <summary>
        /// Para crear roles
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> CreateRole(string roleName);
    }
}
