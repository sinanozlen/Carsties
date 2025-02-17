using System;
using AutoMapper;
using Contracts;
using SearchService.Model;

namespace SearchService.RequestHelpers;

public class MappingProfiles : Profile
{
    protected MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
    }
}
