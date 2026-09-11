
using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Billeteras.Commands
{
    public class DepositCommand
    {
        public int Usuario_Id { get; set; }
        public decimal Monto { get; set; }
    }
}