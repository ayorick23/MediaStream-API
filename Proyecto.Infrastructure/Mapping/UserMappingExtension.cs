using Proyecto.Domain.Entities;
using Proyecto.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.Infrastructure.Mapping
{
    public static class UserMappingExtension
    {
        /// <summary>
        /// Pasa un usuario de dominio a la base de datos (AppIdentityUser)
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public static AppIdentityUser ToAppIdentityUser(this Usuario usuario)
        {
            return new AppIdentityUser
            {
                UserName = usuario.Email,
                Email = usuario.Email,
                PhoneNumber = usuario.Tel
            };
        }

        /// <summary>
        /// Pasa un usuario de la base a Dominio
        /// </summary>
        /// <param name="identityUser"></param>
        /// <returns></returns>
        public static Usuario ToDomainUser(this AppIdentityUser identityUser)
        {
            return new Usuario
            {
                Id = Guid.Parse(identityUser.Id),
                Email = identityUser.Email,
                Tel = identityUser.PhoneNumber,
                FirstName = string.Empty,
                LastName = string.Empty
            };
        }
    }
}
