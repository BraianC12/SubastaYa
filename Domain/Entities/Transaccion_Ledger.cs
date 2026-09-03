using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Transaccion_Ledger
    {
        public int Id { get; set; }

        public string Tipo { get; set; }

        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        public int? Subasta_Id { get; set; }
        public Subasta Subasta { get; set; }

        public int Billetera_Id { get; set; }
        public Billetera Billetera { get; set; }
    }
}
