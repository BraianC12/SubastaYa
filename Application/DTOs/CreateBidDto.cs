using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateBidDto
    {
        public int PujaId { get; set; }
        public int SubastaId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public decimal SaldoDisponibleRestante { get; set; }
    }
}
