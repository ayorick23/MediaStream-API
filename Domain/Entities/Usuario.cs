using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto.Domain.Entities
{
    public class Usuario
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public Guid Id { get; set; }

        public string Tel { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
