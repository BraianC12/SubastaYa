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
        public decimal Monto { get; set; }

        public int Comprador_Id { get; set; }

    }
}
