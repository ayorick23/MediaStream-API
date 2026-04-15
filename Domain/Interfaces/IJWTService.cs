using Proyecto.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.Domain.Interfaces
{
    public interface IJWTService
    {
        /// <summary>
        /// Para generar un token de acceso a recursos
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        string GenerateToken(Usuario usuario, IList<string> roles);
    }
}
