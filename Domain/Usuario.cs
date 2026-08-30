using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Nombre { get; set; }

        public string Password_Hash { get; set; }

        public DateTime Fecha_Registro { get; set; }

        public Billetera Billetera { get; set; }

        public ICollection<Subasta> Subastas { get; set; }

        public ICollection<Puja> Pujas { get; set; }

        public ICollection<Auditoria_Log> Auditorias { get; set; }

    }
}
