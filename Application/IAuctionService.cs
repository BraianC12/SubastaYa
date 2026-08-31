namespace Application.DTOs
{
    public interface IAuctionService
    {
        Task<IEnumerable<AuctionDto>> GetAllAuctionsAsync();
        Task<int> CreateAuctionAsync(CreateAuctionDto dto);
    }
}