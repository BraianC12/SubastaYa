namespace Application.DTOs
{
    public class DepositoDto
    {
        public int TransaccionId { get; set; }
        public decimal MontoDepositado { get; set; }
        public decimal SaldoTotal { get; set; }
        public decimal SaldoDisponible { get; set; }
        public DateTime Fecha { get; set; }
    }
}
