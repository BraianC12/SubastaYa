using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Domain;
using Domain.Exceptions;

namespace Application.UseCases.Subastas.Handlers
{
    public class CreateSubastaCommandHandler: ICreateSubastaCommandHandler
    {
        private readonly ISubastaRepository _subastaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubastaCommandHandler(ISubastaRepository subastaRepository, IUnitOfWork unitOfWork)
        {
            _subastaRepository = subastaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateAuctionDto> Handle(CreateSubastaCommand request)
        {
            if (request.Fecha_Inicio > request.Fecha_Fin)
            {
                throw new DomainException("La fecha de inicio no puede ser posterior a la fecha de finalizacion");
            }

            if (request.Precio_Base < (decimal)0.01)
            {
                throw new DomainException("El precio debe ser mayor a 0");
            }

            if (request.Incremento_Minimo < (decimal)0.01)
            {
                throw new DomainException("El incremento minimo debe ser mayor a 0");
            }

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

            
            await _unitOfWork.SaveChangesAsync();


            return new CreateAuctionDto
            {
                Id = subasta.Id,
                Titulo = subasta.Titulo,
                Descripcion = subasta.Descripcion,
                Url_Imagen = subasta.Url_Imagen,
                Precio_Base = subasta.Precio_Base,
                Incremento_Minimo = subasta.Incremento_Minimo,
                Fecha_Inicio = subasta.Fecha_Inicio,
                Fecha_Fin = subasta.Fecha_Fin,
                Categoria_Id = subasta.Categoria_Id,
                Vendedor_Id = subasta.Vendedor_Id
            };
        }
    }
}