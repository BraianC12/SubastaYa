using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Billetera
    {
       
        public int Id { get; set; }

        public decimal Saldo_Total { get; set; } = 0;

        public decimal Saldo_Retenido { get; set; } = 0;

        public decimal Saldo_Disponible { get; set; } = 0;

        public int Version { get; set; } = 1;
   
        public int Usuario_Id { get; set; }
        public Usuario Usuario { get; set; }

        public ICollection<Transaccion_Ledger> Transacciones { get; set; }

    }
}
