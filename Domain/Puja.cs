using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Puja
    {
        public int Id { get; set; }

        public decimal Monto { get; set; }

        public DateTime Fecha_Puja { get; set; }

        public int Comprador_Id { get; set; }
        public Usuario Comprador { get; set; }

        public int Subasta_Id { get; set; }
        public Subasta Subasta { get; set; }

    }
}
