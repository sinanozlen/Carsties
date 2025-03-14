using System;
using System.Security.Cryptography.X509Certificates;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]

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

public async Task<ActionResult<List<AuctionDto>>> GelAllAuctions(string date)
{

    var query = _context.Auctions.OrderBy(X => X.Item.Make).AsQueryable();

    if(!string.IsNullOrEmpty(date)){
        query = query.Where(x => x.UpdateAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
    }

    return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();

    }

    [HttpGet("{id}")]

    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
        .Include(x=>x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction); 
    }
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDTO auctionDto){
        var auction = _mapper.Map<Auction>(auctionDto);
        // TODO :add current user as seller
        auction.Seller = "test";

        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() >0;

        if(!result) return BadRequest("Couldn't not save changes to DB");

        return CreatedAtAction(nameof(GetAuctionById),
        new{auction.Id}, _mapper.Map<AuctionDto>(auction));
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id==id);

        if (auction == null) return NotFound();

        // TODO: check seller == username

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;

        if(result) return Ok();

        return BadRequest("Problem saving changes");

    }
    [HttpDelete("{id}")]

    public async Task<ActionResult> DeleteAuction(Guid id){
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null) return NotFound();

        // TODO :check seller == username

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() >0;

        if(!result) return BadRequest("Could not upgrade DB");

        return Ok(); 
    }

}