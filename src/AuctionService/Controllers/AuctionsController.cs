using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;


[ApiController]
[Route("/api/[Controller]")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }
    [HttpGet]
    public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date = "")
    {
        var query = _context.Auctions.OrderBy(a => a.Item.Make).AsQueryable();
        if (!string.IsNullOrWhiteSpace(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDTO>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
    {
        var auction = await this._context.Auctions
        .Include(i => i.Item)
        .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
            return NotFound();
        return Ok(_mapper.Map<AuctionDTO>(auction));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO)
    {

        var auction = _mapper.Map<Auction>(auctionDTO);

        auction.Seller = User.Identity.Name;

        _context.Auctions.Add(auction);

        var newauction = _mapper.Map<AuctionDTO>(auction);
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newauction));

        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Could not save changes to the auction");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDTO>(auction));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDTO)
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(i => i.Id == id);
        if (auction == null) return NotFound();

        if (!auction.Seller.ToLower().Equals(User.Identity.Name.ToLower())) return Forbid();

        _mapper.Map<UpdateAuctionDTO, Item>(updateAuctionDTO, auction.Item);

        auction.UpdatedAt = DateTime.UtcNow;

        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;
        if (result) return Ok();

        return BadRequest("Problem saving changes");
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null) return NotFound();

        if (!auction.Seller.ToLower().Equals(User.Identity.Name.ToLower())) return Forbid();

        _context.Auctions.Remove(auction);

        _publishEndpoint.Publish(new AuctionDeleted { Id = id.ToString() });

        var result = await _context.SaveChangesAsync() > 0;
        if (result) return Ok();

        return BadRequest("Could not delete auction");
    }
}
