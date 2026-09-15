namespace Application.DTOs
{
    public class WalletBalanceDto
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }  
        public decimal Saldo_Total { get; set; }
        public decimal Saldo_Retenido { get; set; }
        public decimal Saldo_Disponible { get; set; }
    }
}
