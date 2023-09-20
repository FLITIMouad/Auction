using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.Utils;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDTO>().IncludeMembers(it => it.Item);
        CreateMap<Item, AuctionDTO>();
        CreateMap<CreateAuctionDTO, Auction>()
        .ForMember(it => it.Item, it => it.MapFrom(x => x));
        CreateMap<CreateAuctionDTO, Item>();
        CreateMap<UpdateAuctionDTO, Item>()
           .ForAllMembers(opt =>
           {
               opt.Condition((src, dest, srcMember, destMember) =>
               {
                   if (srcMember?.GetType() == typeof(int))
                       return int.Parse(srcMember + "") != 0;
                   else if (srcMember?.GetType() == typeof(string))
                       return !string.IsNullOrWhiteSpace(srcMember + "");
                   else
                       return srcMember != null;
               });
           });

        CreateMap<AuctionDTO, AuctionCreated>();
         CreateMap<Auction, AuctionUpdated>().IncludeMembers(x=>x.Item);
         CreateMap<Item, AuctionUpdated>();
    }
}

