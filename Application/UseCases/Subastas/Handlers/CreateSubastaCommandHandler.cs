using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Subastas.Handlers
{
    public class CreateSubastaCommandHandler : IRequestHandler<CreateSubastaCommand, int>
    {
        private readonly ISubastaRepository _subastaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubastaCommandHandler(ISubastaRepository subastaRepository, IUnitOfWork unitOfWork)
        {
            _subastaRepository = subastaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateSubastaCommand request, CancellationToken cancellationToken)
        {
            
            var subasta = new Subasta
            {
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                Url_Imagen = request.Url_Imagen,
                Precio_Base = request.Precio_Base,
                Incremento_Minimo = request.Incremento_Minimo,
                Fecha_Inicio = request.Fecha_Inicio,
                Fecha_Fin = request.Fecha_Fin,
                Categoria_Id = request.Categoria_Id,
                Vendedor_Id = request.Vendedor_Id,
                Estado = "ACTIVA" 
            };

            
            await _subastaRepository.AddAsync(subasta);

            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

          
            return subasta.Id;
        }
    }
}