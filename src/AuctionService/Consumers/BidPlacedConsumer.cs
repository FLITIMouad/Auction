using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _auctionDbContext;

    public BidPlacedConsumer(AuctionDbContext auctionDbContext)
    {
        _auctionDbContext = auctionDbContext;
    }
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("---> consuming auction bid placed");
        var auction = await _auctionDbContext.Auctions.FindAsync(context.Message.AuctionId);
        if ( context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _auctionDbContext.SaveChangesAsync();
        }

    }
}
