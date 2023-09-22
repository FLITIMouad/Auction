using AutoMapper;
using Contracts;
using MassTransit;
using MassTransit.Transports;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;


public class AuctionUpdatedCnsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedCnsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> consuming Auction Updated:" + context.Message.Id+" Updated At"+ context.Message.UpdatedAt);

        var item = await DB.Find<Item>().OneAsync(context.Message.Id);

        _mapper.Map<AuctionUpdated, Item>(context.Message, item);

    try{
        await item.SaveAsync();
      }
      catch (Exception e) {

            throw new MessageException(typeof(AuctionUpdated), "Probler updating mongodb");
      }
      


    }
}
