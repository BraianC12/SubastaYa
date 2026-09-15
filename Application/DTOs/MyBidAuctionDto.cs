using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MyBidAuctionDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Url_Imagen { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public decimal Precio_Base { get; set; }
        public decimal Incremento_Minimo { get; set; }
        public decimal TuPuja { get; set; }
        public decimal PujaLiderActual { get; set; }
        public bool EsLider { get; set; }
        public bool EsGanador { get; set; }
    }
}