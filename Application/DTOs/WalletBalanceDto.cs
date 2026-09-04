namespace Application.DTOs
{
    public class WalletBalanceDto
    {
        public string Usuario { get; set; }  
        public decimal Saldo_Total { get; set; }
        public decimal Saldo_Retenido { get; set; }
        public decimal Saldo_Disponible { get; set; }
    }
}
