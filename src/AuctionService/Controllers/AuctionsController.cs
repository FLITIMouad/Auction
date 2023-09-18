using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;


[ApiController]
[Route("/api/[Controller]")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date="")
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

    [HttpPost]
    public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO)
    {

        var auction = _mapper.Map<Auction>(auctionDTO);
        //Todo: add current user As seller

        _context.Auctions.Add(auction);
        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Could not save changes to the auction");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDTO>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDTO)
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(i => i.Id == id);
        if (auction == null) return NotFound();
        //TODO : check SELLER == CurrentUser
        _mapper.Map<UpdateAuctionDTO, Item>(updateAuctionDTO, auction.Item);
        var result = await _context.SaveChangesAsync() > 0;
        if (result) return Ok();

        return BadRequest("Problem saving changes");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null) return NotFound();
        //TODO : check SELLER == CurrentUser
        _context.Auctions.Remove(auction);
        var result = await _context.SaveChangesAsync() > 0;
        if (result) return Ok();

        return BadRequest("Could not delete auction");
    }
}
