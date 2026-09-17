using Application.DTOs;
using Application.Interfaces; 
using Application.UseCases.Subastas.Queries;


namespace Application.UseCases.Subastas.Handlers
{
    public class GetHistorialPujasQueryHandler : IGetHistorialPujasQueryHandler
    {
        private readonly ISubastaRepository _subastaRepository;

        public GetHistorialPujasQueryHandler(ISubastaRepository subastaRepository)
        {
            _subastaRepository = subastaRepository;
        }

        public async Task<List<PujaHistorialDto>> Handle(GetHistorialPujasQuery query)
        {
            var subasta = await _subastaRepository.GetByIdAsync(query.SubastaId);

            if (subasta == null || subasta.Pujas == null)
            {
                return new List<PujaHistorialDto>();
            }
   
            var historial = subasta.Pujas
                .OrderByDescending(p => p.Fecha_Puja)
                .Select(p => new PujaHistorialDto
                {
                    Id = p.Id,
                    Monto = p.Monto,
                    FechaPuja = p.Fecha_Puja,
                    UsuarioAnonimo = $"Postor #{p.Comprador_Id.ToString().Substring(0, 1)}***"
                })
                .ToList();

            return historial;
        }
    }
}