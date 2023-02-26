using Esourcing.Sourcing.Entities;

namespace Esourcing.Sourcing.Repositories.Interfaces
{
    public interface IBidRepository
    {
        Task SendBid(Bid bid);
        Task<List<Bid>> GetBidsByAuctionId(string id);
        Task<List<Bid>> GetAllBidsByAuctionId(string id);
        Task<Bid> GetWinnerBid(string id);
    }
}
