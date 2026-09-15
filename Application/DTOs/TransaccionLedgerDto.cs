using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TransaccionLedgerDto
    {
        public int Id { get; set; }
        public int Billetera_Id { get; set; }
        public int? Subasta_Id { get; set; }
        public string? Subasta_Titulo { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
