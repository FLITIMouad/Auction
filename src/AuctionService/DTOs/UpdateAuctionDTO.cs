﻿namespace AuctionService.DTOs;

public class UpdateAuctionDTO
{
    public string? Make { get; set; }=null;
    public string? Model { get; set; }=null;
    public int? Year { get; set; }=null;
    public string? Color { get; set; }=null;
    public int? Mileage { get; set; }=null;
}